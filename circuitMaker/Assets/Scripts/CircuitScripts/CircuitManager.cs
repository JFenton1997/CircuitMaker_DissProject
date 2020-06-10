using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

/// <summary>
/// key class handling setting of connections of a circuit diagram, generating diagram data and other circuit component functions
/// </summary>
public class CircuitManager : MonoBehaviour
{
    [SerializeField]
    public Dictionary<int, List<DiagramComponent>> diagramData; // for fiagram data
    private DiagramComponent cellComponent = null;
    public GameObject circuitComponentPrefab, circuitWirePrefab;
    public List<CircuitComponent> allcomponents;
    public HashSet<DiagramError> foundErrors;


    // if belongs to problem builder scene
    public bool isBuilder = true;


    // used for names
    private char ConpName = 'A';


    /// <summary>
    /// initialize lists
    /// </summary>
    private void Start()
    {
        foundErrors = new HashSet<DiagramError>();
        allcomponents = new List<CircuitComponent>();
    }


    /// <summary>
    /// call on build component button press, buildes component of CELL if no cell present, else builds a RESISTOR
    /// </summary>
    public void buildCircuitcomponent()
    {
        GameObject circuitComponentOBJ = (GameObject)Instantiate(circuitComponentPrefab, new Vector3(0f, 0f, 1000f), Quaternion.identity, transform);
        CircuitComponent newcomponent = circuitComponentOBJ.GetComponent<CircuitComponent>();

        newcomponent.name = "CELL";
        circuitComponentOBJ.name = "CELL";
        newcomponent.name = "CELL";
        allcomponents.Add(newcomponent);
        if (allcomponents.FindAll(x => x.component.type == ComponentType.CELL).Count > 1)
        {
            newcomponent.component.type = ComponentType.RESISTOR;
            newcomponent.component.Values[ComponentParameter.RESISTANCE].value = 1f;
            newcomponent.component.direction = Direction.A_to_B;
            circuitComponentOBJ.name = ConpName.ToString();
            newcomponent.name = ConpName.ToString();
            ConpName++;
        }

        newcomponent.clickAndDrag.enabled = true;
        newcomponent.clickAndDrag.MoveStart();


    }


    /// <summary>
    /// called on WIRE build tool button press, adds a wire to the scene and call create from button
    /// </summary>
    public void buildCircuitWire()
    {
        GameObject circuitComponentOBJ = (GameObject)Instantiate(circuitWirePrefab, Vector3.zero, Quaternion.identity, transform);
        Wire newcomponent = circuitComponentOBJ.GetComponent<Wire>();
        newcomponent.gameObject.name = "wire";
        newcomponent.createdFromButton();


    }



    /// <summary>
    /// used to create DiagramData, if errors found, run error window, if no errors and isbuilder, run save, else run solver
    /// </summary>
    public void GenerateDiagramData()
    {
        //empty error and get all circuit to allComponents
        foundErrors.Clear();
        foreach (CircuitComponent c in transform.GetComponentsInChildren<CircuitComponent>())
        {
            if (c.component.type != ComponentType.UNTYPED)
            {
                if (!allcomponents.Contains(c))
                {
                    allcomponents.Add(c);
                }
            }
        }



        // no components
        if (allcomponents.Count == 0)
        {
            foundErrors.Add(new DiagramError("No component Error    ", "There are no components to make a diagram from, please create a diagram before trying to save."));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }
        //only just 1 component
        if (allcomponents.Count == 1)
        {
            foundErrors.Add(new DiagramError("NO component ERROR", "There are no components to make a diagram from, please use at least 2 components before saving."));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }

        // more or less than 1 battery
        if (allcomponents.FindAll(x => x.component.type == ComponentType.CELL).Count != 1)
        {
            foundErrors.Add(new DiagramError("CELL NUMBER ERROR  ", "has be one cell in the diagram, and only one. please remove/add the required cells."));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }


        try
        {
            //update wire connections and circuit connections
            updateAllWireConnections();
            SetCircuitConnections();
        }
        catch
        {
            //if error found, the not connected correctly, show error to user
            foundErrors.Add(new DiagramError("NO CONNECTION ERROR  ", "Fail to set up connections, Make sure the Diagram has connections"));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;


        }






        //component with no connections
        foreach (CircuitComponent c in allcomponents.FindAll(x => x.component.Aconnections.Count == 0 || x.component.Bconnections.Count == 0))
        {
            foundErrors.Add(new DiagramError("NO CONNECTION ERROR   ",
            "component " + c.name + " is not fully connected to circuit\nA Connections Found: " + string.Join(" , ", c.component.Aconnections.ConvertAll(x => x.name)) +
            "\nB Connections Found: " + string.Join(" , ", c.component.Bconnections.ConvertAll(x => x.name)) + "\n(no name means no connections.\n" +
            "make sure connections are going into the component and not along the component", c.component, allcomponents));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }
        //check for any wires completely not connected
        List<Wire> unconnectedWires = new List<Wire>(GetComponentsInChildren<Wire>());
        if (unconnectedWires.Find(x => x.connectedWires.Count == 0) == unconnectedWires.Find(x => x.connectedNode.Count == 0))
        {
            foundErrors.Add(new DiagramError("UNCONNECTED WIRE ERROR  ", "there are wire or wires with no connections, please delete the wire before saving."));
        }




        //component with incorrect values check using ohms law
        if (isBuilder)
            foreach (CircuitComponent c in allcomponents.FindAll(x => Math.Round(x.component.Values[ComponentParameter.VOLTAGE].value / x.component.Values[ComponentParameter.CURRENT].value
            , 2) != Math.Round(x.component.Values[ComponentParameter.RESISTANCE].value, 2)))
            {

                if (c.component.type != ComponentType.CELL && c.component.type != ComponentType.UNTYPED)
                {
                    Debug.Log(Math.Round(c.component.Values[ComponentParameter.VOLTAGE].value / c.component.Values[ComponentParameter.CURRENT].value
            , 2));

                    foundErrors.Add(new DiagramError("component VALUE ERROR    ", "the values for " + c.gameObject.name + " dont add up correctly to ohm's law, fix values or use auto feature", c.component, allcomponents));
                }


            }



        //most major errors been checked, start diagramData generation
        diagramData = new Dictionary<int, List<DiagramComponent>>();
        //for each circuit component DiagramComponent in allAll components, set A and B connections of each
        foreach (CircuitComponent d in transform.GetComponentsInChildren<CircuitComponent>())
        {
            DiagramComponent c = d.component;
            c.Aconnections.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));
            c.Bconnections.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));

        }

        // find cell to use as a starting off point to generate diagramData
        foreach (CircuitComponent circuitComponent in transform.GetComponentsInChildren<CircuitComponent>())
        {
            DiagramComponent c = circuitComponent.component;

            if (c.type == ComponentType.CELL)
            {

                cellComponent = c;
                break;
            }
        }


        int layerValue = 0;
        List<DiagramComponent> vistedComponents = new List<DiagramComponent>(); //store visited
        vistedComponents.Add(cellComponent);
        List<DiagramComponent> layerList = new List<DiagramComponent>(); //used to insert into diagramData
        List<DiagramComponent> outputConnections; //used to out connections 
        layerList.Add(cellComponent);
        diagramData.Add(layerValue, layerList); //inserting layer 0
        //Component currentComponent = cellComponent;
        while (diagramData[layerValue].Count > 0)
        {
            // prevent infinte loops 
            if (layerValue > 1000)
            {
                Debug.LogError("infinteLoop");
                foundErrors.Add(new DiagramError("INFINITE LOOP CREATED  ", "following connections made an infinite loop, this is caused by having a component connected to itself," +
                " Please keep to standard DC circuit, avoid any recursion"));
                transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
                return;
            }
            layerList = new List<DiagramComponent>();
            //for each component in next layer
            foreach (DiagramComponent currentComponent in diagramData[layerValue].ToArray())
            {
                //itialise new search
                //get outputs
                outputConnections = getConnectionsOfDirection(currentComponent, true);
                //for each output connection of current component
                foreach (DiagramComponent c in outputConnections)
                {

                    //if not cell
                    if (c.type != ComponentType.CELL)
                    {
                        //if in visited
                        if (vistedComponents.Contains(c) && !layerList.Contains(c))
                        {
                            //remove from prev layers and add to this layer
                            removeComponentOnPreviousLayer(c, diagramData);
                        }
                        //if not already add to this layer
                        if (!layerList.Contains(c))
                        {
                            layerList.Add(c);
                            vistedComponents.Add(c);
                        }
                    }

                }


            }
            //sort by x cord left to right for later generator use
            layerList.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));
            layerValue++;
            //insert layer
            diagramData.Add(layerValue, layerList);


        }
        diagramData.Remove(layerValue);





        //calculate Cell Values to check correct values
        float voltage = 0;
        float current = 0;
        foreach (DiagramComponent d in diagramData[1])
        {
            current += d.Values[ComponentParameter.CURRENT].value;
        }
        DiagramComponent voltageDiagramCheck = diagramData[1][0];
        while (true)
        {
            voltage += voltageDiagramCheck.Values[ComponentParameter.VOLTAGE].value;
            if (voltageDiagramCheck.Bconnections.Count != 0)
            {
                if (voltageDiagramCheck.Bconnections[0].type != ComponentType.CELL)
                {
                    voltageDiagramCheck = voltageDiagramCheck.Bconnections[0];
                    continue;
                }
            }
            break;


        }
        //if in problem builder scene
        if (isBuilder)
            if (diagramData[0][0].Values[ComponentParameter.VOLTAGE].value != voltage || diagramData[0][0].Values[ComponentParameter.CURRENT].value != current)
            {
                foundErrors.Add(new DiagramError("CELL VALUE ERROR ",
                "the cells values dont add up to the rest of the circuit using a basic check, using top for current and leftmost path for voltage, the values should be " +
                Math.Round(voltage, 2) + "V    " + Math.Round(current, 2) + "I"));

            }
        // diagramData[0][0].Values[ComponentParameter.VOLTAGE].value = voltage;
        // diagramData[0][0].Values[ComponentParameter.CURRENT].value = current;
        // diagramData[0][0].Values[ComponentParameter.RESISTANCE].value = 0f;

        //box check, checking if values are correct in a basic check
        float Cvoltage = 0;
        float Ccurrent = 0;
        foreach (DiagramComponent d in diagramData[0][0].Bconnections)
        {
            Ccurrent += d.Values[ComponentParameter.CURRENT].value;
        }
        voltageDiagramCheck = diagramData[1][diagramData[1].Count - 1];
        while (true)
        {
            Cvoltage += voltageDiagramCheck.Values[ComponentParameter.VOLTAGE].value;
            if (voltageDiagramCheck.Bconnections.Count != 0)
            {
                if (voltageDiagramCheck.Bconnections[0].type != ComponentType.CELL)
                {
                    voltageDiagramCheck = voltageDiagramCheck.Bconnections[0];
                    continue;
                }
            }
            break;


        }

        Debug.Log(Ccurrent + " " + current + "  " + voltage + "  " + Cvoltage);
        if (Cvoltage != voltage && isBuilder)
        {
            foundErrors.Add(new DiagramError("VALUES ERROR   ", "Using a basic check, the values of the circuit are Incorrect, the voltage down the leftmost path is " +
            voltage + "Vand on the leftMost path is " + Cvoltage + "V, all paths should add up to the same, this error check isnt conprohensive, please float check the values"));
        }
        if (Ccurrent != current && isBuilder)
        {
            foundErrors.Add(new DiagramError("VALUES ERROR   ", "Using a basic check, the values of the circuit are Incorrect, the current on the top path is " +
            current + "I and on the bottom path is " + Ccurrent + "I, all paths should add up to the same, this error check isnt conprohensive, please float check the values"));
        }

        if (foundErrors.Count != 0)
        {
            Debug.Log(transform.Find("/UI/ErrorsPanel").name);
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;
        }

        //calculating a scale value to be used when generated to a avow, finding the largest scale smaller than smallest value in diagram
        float scale = 1;
        float smallest = Mathf.Infinity;
        Debug.Log(allcomponents.Count);
        foreach (var values in allcomponents.ConvertAll(x => x.component.Values))
        {
            foreach (var lowestVal in values)
            {
                if(lowestVal.Key != ComponentParameter.RESISTANCE)
                if (smallest > lowestVal.Value.value)
                {
                    smallest = lowestVal.Value.value;
                }
            }
        }
        Debug.Log(smallest);
        scale = 0.001f;
        if (smallest >= 0.1)
        {
            scale = 0.1f;
        }
        if (smallest >= 1)
        {
            scale = 1f;
        }
        if (smallest >= 10)
        {
            scale = 10f;
        }
        if (smallest >= 100)
        {
            scale = 100f;
        }
        if (smallest >= 1000)
        {
            scale = 1000f;
        }
        Debug.Log(scale);

        //if in problem builder, run save window, else send to solver
        if (isBuilder)
            transform.Find("/UI/SaveDiagram").GetComponent<SaveFileWindow>().intialiseSaveWindow(diagramData, scale);
        else
        {
            transform.Find("/UI/SolverPanel").GetComponent<SolverScript>().compareAnswers(diagramData);

        }

        // for debugging
        foreach (var data in diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join("",
data.Value
.ConvertAll(i => i.name.ToString())
.ToArray()));

        }
    }


    /// <summary>
    /// used to remove DiagramComponent from a previous layer
    /// </summary>
    /// <param name="c"> component to remove</param>
    /// <param name="d">diagramData so far</param>
    private void removeComponentOnPreviousLayer(DiagramComponent c, Dictionary<int, List<DiagramComponent>> d)
    {
        foreach (var layerValue in d)
        {
            foreach (DiagramComponent layerComponent in layerValue.Value)
            {
                if (c == layerComponent)
                {
                    d[layerValue.Key].Remove(layerComponent);
                    return;
                }
            }
        }
        Debug.LogError(c + " was not found to be deleted on prev layer");
        return;
    }


    /// <summary>
    /// gets the circuit component of a diagram component
    /// </summary>
    /// <param name="d">diagram component</param>
    /// <returns>circuit component storing the given diagram component</returns>
    private CircuitComponent getCircuitComponent(DiagramComponent d)
    {
        foreach (CircuitComponent circuitComponent in transform.GetComponentsInChildren<CircuitComponent>())
        {
            if (circuitComponent.component == d)
            {
                return circuitComponent;
            }
        }
        return null;
    }


    /// <summary>
    /// gets the lists of inputs or outputs of a given diagram component
    /// </summary>
    /// <param name="c"> Diagram component to get input or output from</param>
    /// <param name="output">bool if output, if true return outputs ,else return intputs </param>
    /// <returns></returns>
    private List<DiagramComponent> getConnectionsOfDirection(DiagramComponent c, bool output)
    {
        List<DiagramComponent> connections;
        if (output)
        {
            if (c.direction == Direction.A_to_B)
            {
                connections = c.Bconnections;
            }
            else
            {
                connections = c.Aconnections;
            }
        }
        else
        {
            if (c.direction == Direction.A_to_B)
            {
                connections = c.Aconnections;
            }
            else
            {
                connections = c.Bconnections;
            }

        }
        return connections;
    }

    /// <summary>
    /// returns true if the node is the inverse of the require connection type
    /// only returns true if the node and isoutput is diffrent
    /// thus true if node is output and isoutput is false (intput to output)
    /// or node is a input and isoutput is true (output to input)
    /// </summary>
    /// <param name="n">node to check</param>
    /// <param name="isOutput">if output,</param>
    /// <returns></returns>
    private bool isCorrectNode(Node n, bool isOutput)
    {
        if (isNodeOutput(n) == isOutput)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// gets weather a given node is a output connector
    /// </summary>
    /// <param name="n"> node given</param>
    /// <returns>true if node is output, else false</returns>
    private bool isNodeOutput(Node n)
    {
        CircuitComponent c = n.GetComponentInParent<CircuitComponent>();
        DiagramComponent d = c.component;
        if (d.direction == Direction.A_to_B)
        {
            if (n.name == "nodeA")
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        else
        {
            if (n.name == "nodeA")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// runs updatesWireConnections on all wires, used mainly to fix middle wireconnections using collisions
    /// </summary>
    private void updateAllWireConnections()
    {
        List<Wire> allWire = new List<Wire>(transform.GetComponentsInChildren<Wire>());
        foreach (Wire w in allWire)
        {
            w.updateWireConnections();
        }
    }


    /// <summary>
    /// initial method run to set up connections of all components in a circuit diagram
    /// </summary>
    private void SetCircuitConnections()
    {
        foreach (CircuitComponent circuitComponent in gameObject.GetComponentsInChildren<CircuitComponent>())
        {
            foreach (Node n in new Node[2] { circuitComponent.nodeA, circuitComponent.nodeB }) // another check to make sure all nodes with wire are connrected
            {
                foreach (Collider2D collider in Physics2D.OverlapCircleAll(n.transform.position, (n.GetComponent<RectTransform>().sizeDelta.x / 4) * n.GetComponent<RectTransform>().localScale.x))
                {
                    if (collider.TryGetComponent<Wire>(out Wire w))
                    {
                        w.addConnection(n);
                        n.ConnectedWire = w;
                    }
                }
            }
            //set coonnections of each circuit component with debugging information
            SetComponentConnections(circuitComponent);
            DiagramComponent component = circuitComponent.component;
            Debug.Log(component.name + "     " + component.type.ToString());
            Debug.Log("Aconnections = " + String.Join("",
            component.Aconnections
            .ConvertAll(i => i.name.ToString())
            .ToArray()));
            Debug.Log("Bconnections = " + String.Join("",
             component.Bconnections
             .ConvertAll(i => i.name.ToString())
             .ToArray()));

        }
        //correctDirection();

    }

    /// <summary>
    /// for a given circuit component run getConnectedComponents to set connections for list of connections in DiagramComponent
    /// </summary>
    /// <param name="circuitComponent">a given circuit component to set connections to</param>
    private void SetComponentConnections(CircuitComponent circuitComponent)
    {
        DiagramComponent component = circuitComponent.component;
        Debug.Log(circuitComponent.gameObject);
        component.Aconnections = GetConnectedComponents(circuitComponent.nodeA, isNodeOutput(circuitComponent.nodeA));
        component.Bconnections = GetConnectedComponents(circuitComponent.nodeB, isNodeOutput(circuitComponent.nodeB));


    }

    /// <summary>
    /// returns all components which are connected the correct way (input to output / output to input)
    /// gets all connected wires and nodes from a singular node
    /// </summary>
    /// <param name="node"> given node</param>
    /// <param name="isOutput"> if node is a input or output node</param>
    /// <returns></returns>
    private List<DiagramComponent> GetConnectedComponents(Node node, bool isOutput)
    {
        List<DiagramComponent> connectedComponents = new List<DiagramComponent>();
        List<Wire> wiresToTest = new List<Wire>();
        List<Wire> vistedWires = new List<Wire>();
        wiresToTest.Add(node.ConnectedWire);
        while (wiresToTest.Count > 0) //loop till all connected wires are explored
        {

            Wire w = wiresToTest[0];
            //for each node found, check if valid if so add to list
            foreach (Node n in w.connectedNode)
            {
                if (!connectedComponents.Contains(n.GetComponentInParent<CircuitComponent>().component) && n != node)
                {
                    if (isCorrectNode(n, isOutput))
                    {
                        connectedComponents.Add(n.GetComponentInParent<CircuitComponent>().component);
                    }

                }
                //if wire found to node isnt in visted, add to be tested, prevent infinite loops
                if (!vistedWires.Contains(n.ConnectedWire))
                {
                    wiresToTest.Add(n.ConnectedWire);
                    vistedWires.Add(n.ConnectedWire);
                }
            }
            //adding all wire to each wire connected to w
            foreach (Wire u in w.connectedWires)
            {
                if (!vistedWires.Contains(u))
                {
                    wiresToTest.Add(u);
                    vistedWires.Add(u);
                }
            }
            wiresToTest.Remove(w);
        }


        return connectedComponents;




    }
}




