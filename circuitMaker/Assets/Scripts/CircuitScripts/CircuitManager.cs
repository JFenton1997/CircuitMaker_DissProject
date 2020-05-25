using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class CircuitManager : MonoBehaviour
{
    [SerializeField]
    public Dictionary<int, List<DiagramComponent>> diagramData;
    private DiagramComponent cellComponent = null;
    public GameObject circuitComponentPrefab, circuitWirePrefab;
    public List<CircuitComponent> allConponents;
    public HashSet<DiagramError> foundErrors;



    private char ConpName = 'A';

    private void Start()
    {
        foundErrors = new HashSet<DiagramError>();
        allConponents = new List<CircuitComponent>();
    }

    public void buildCircuitConponent()
    {
        GameObject circuitComponentOBJ = (GameObject)Instantiate(circuitComponentPrefab, new Vector3(0f, 0f, 1000f), Quaternion.identity, transform);
        CircuitComponent newConponent = circuitComponentOBJ.GetComponent<CircuitComponent>();

        newConponent.name = "CELL";
        circuitComponentOBJ.name = "CELL";
        newConponent.name = "CELL";
        allConponents.Add(newConponent);
        if (allConponents.FindAll(x => x.conponent.type == ComponentType.CELL).Count > 1)
        {
            newConponent.conponent.type = ComponentType.RESISTOR;
            newConponent.conponent.Values[ComponentParameter.RESISTANCE].value = 1f;
            newConponent.conponent.direction = Direction.A_to_B;
            circuitComponentOBJ.name = ConpName.ToString();
            newConponent.name = ConpName.ToString();
            ConpName++;
        }

        newConponent.clickAndDrag.enabled = true;
        newConponent.clickAndDrag.MoveStart();


    }



    public void buildCircuitWire()
    {
        GameObject circuitComponentOBJ = (GameObject)Instantiate(circuitWirePrefab, Vector3.zero, Quaternion.identity, transform);
        Wire newConponent = circuitComponentOBJ.GetComponent<Wire>();
        newConponent.gameObject.name = "wire";
        newConponent.createdFromButton();


    }




    public void GenerateDiagramData()
    {
        foundErrors.Clear();
        foreach (CircuitComponent c in transform.GetComponentsInChildren<CircuitComponent>())
        {
            if (c.conponent.type != ComponentType.UNTYPED)
            {
                if (!allConponents.Contains(c))
                {
                    allConponents.Add(c);
                }
            }
        }



        // no conponents
        if (allConponents.Count == 0)
        {
            foundErrors.Add(new DiagramError("No Conponent Error    ", "There are no conponents to make a diagram from, please create a diagram before trying to save."));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }
        //only just 1 conponent
        if (allConponents.Count == 1)
        {
            foundErrors.Add(new DiagramError("NO CONPONENT ERROR", "There are no conponents to make a diagram from, please use at least 2 conponents before saving."));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }

        // more or less than 1 battery
        if (allConponents.FindAll(x => x.conponent.type == ComponentType.CELL).Count != 1)
        {
            foundErrors.Add(new DiagramError("CELL NUMBER ERROR  ", "has be one cell in the diagram, and only one. please remove/add the required cells."));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }


        try
        {
            updateAllWireConnections();
            SetCircuitConnections();
        }
        catch
        {
            foundErrors.Add(new DiagramError("NO CONNECTION ERROR  ", "Fail to set up connections, Make sure the Diagram has connections"));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;


        }






        //conponent with no connections
        foreach (CircuitComponent c in allConponents.FindAll(x => x.conponent.Aconnections.Count == 0 || x.conponent.Bconnections.Count == 0))
        {
            foundErrors.Add(new DiagramError("NO CONNECTION ERROR   ",
            "Conponent " + c.name + " is not fully connected to circuit\nA Connections Found: " + string.Join(" , ", c.conponent.Aconnections.ConvertAll(x => x.name)) +
            "\nB Connections Found: " + string.Join(" , ", c.conponent.Bconnections.ConvertAll(x => x.name)) + "\n(no name means no connections.", c.conponent, allConponents));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }
        List<Wire> unconnectedWires = new List<Wire>(GetComponentsInChildren<Wire>());
        if (unconnectedWires.Find(x => x.connectedWires.Count == 0) == unconnectedWires.Find(x => x.connectedNode.Count == 0))
        {
            foundErrors.Add(new DiagramError("UNCONNECTED WIRE ERROR  ", "there are wire or wires with no connections, please delete the wire before saving."));
        }




        //conponent with incorrect values
        foreach (CircuitComponent c in allConponents.FindAll(x => x.conponent.Values[ComponentParameter.VOLTAGE].value / x.conponent.Values[ComponentParameter.CURRENT].value
         != x.conponent.Values[ComponentParameter.RESISTANCE].value))
        {
            if (c.conponent.type != ComponentType.CELL && c.conponent.type != ComponentType.UNTYPED)
            {
                foundErrors.Add(new DiagramError("CONPONENT VALUE ERROR    ", "the values for " + c.gameObject.name + " dont add up correctly to ohm's law, fix values or use auto feature", c.conponent, allConponents));
            }


        }




        diagramData = new Dictionary<int, List<DiagramComponent>>();
        foreach (CircuitComponent d in transform.GetComponentsInChildren<CircuitComponent>())
        {
            DiagramComponent c = d.conponent;
            c.Aconnections.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));
            c.Bconnections.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));

        }
        foreach (CircuitComponent circuitComponent in transform.GetComponentsInChildren<CircuitComponent>())
        {
            DiagramComponent c = circuitComponent.conponent;

            if (c.type == ComponentType.CELL)
            {

                cellComponent = c;
                break;
            }
        }
        if (cellComponent == null)
        {
            Debug.LogError("No Cell Detected");
            return;
        }
        int layerValue = 0;
        List<DiagramComponent> vistedComponents = new List<DiagramComponent>();
        vistedComponents.Add(cellComponent);
        List<DiagramComponent> layerList = new List<DiagramComponent>();
        List<DiagramComponent> outputConnections;
        layerList.Add(cellComponent);
        diagramData.Add(layerValue, layerList);
        //Component currentComponent = cellComponent;
        while (diagramData[layerValue].Count > 0)
        {
            if (layerValue > 1000)
            {
                Debug.LogError("infinteLoop");
                 foundErrors.Add(new DiagramError("INFINITE LOOP CREATED  ", "following connections made an infinite loop, this is caused by having a conponent connected to itself,"+
                 " Please keep to standard DC circuit, avoid any recursion"));
                     transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
                return;
            }
            layerList = new List<DiagramComponent>();
            //for each component in next layer
            foreach (DiagramComponent currentComponent in diagramData[layerValue].ToArray())
            {
                //itialise new search

                outputConnections = getConnectionsOfDirection(currentComponent, true);
                //for each connection of current component
                foreach (DiagramComponent c in outputConnections)
                {

                    //Debug.Log("currentComp:" + currentComponent + " layer:" + layerValue + " connection:" + c);
                    if (c.type != ComponentType.CELL)
                    {
                        if (vistedComponents.Contains(c) && !layerList.Contains(c))
                        {
                            //Debug.Log("remove called on:" + c + " layer:" + layerValue + " currentComponent:" + currentComponent);
                            removeComponentOnPreviousLayer(c, diagramData);
                        }
                        if (!layerList.Contains(c))
                        {
                            layerList.Add(c);
                            vistedComponents.Add(c);
                        }
                    }

                }


            }
            layerList.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));
            layerValue++;
            diagramData.Add(layerValue, layerList);


        }
        diagramData.Remove(layerValue);





        //calculate Cell Values
        double voltage = 0;
        double current = 0;
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


        if (diagramData[0][0].Values[ComponentParameter.VOLTAGE].value != voltage || diagramData[0][0].Values[ComponentParameter.CURRENT].value != current)
        {
            foundErrors.Add(new DiagramError("CELL VALUE ERROR ",
            "the cells values dont add up to the rest of the circuit using a basic check, using top for current and leftmost path for voltage, the values should be " +
            voltage + "V    " + current + "I"));

        }
        // diagramData[0][0].Values[ComponentParameter.VOLTAGE].value = voltage;
        // diagramData[0][0].Values[ComponentParameter.CURRENT].value = current;
        // diagramData[0][0].Values[ComponentParameter.RESISTANCE].value = 0f;

        //box check
        double Cvoltage = 0;
        double Ccurrent = 0;
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
        if (Cvoltage != voltage)
        {
            foundErrors.Add(new DiagramError("VALUES ERROR   ", "Using a basic check, the values of the circuit are Incorrect, the voltage down the leftmost path is " +
            voltage + "Vand on the leftMost path is " + Cvoltage + "V, all paths should add up to the same, this error check isnt conprohensive, please double check the values"));
        }
        if (Ccurrent != current)
        {
            foundErrors.Add(new DiagramError("VALUES ERROR   ", "Using a basic check, the values of the circuit are Incorrect, the current on the top path is " +
            current + "I and on the bottom path is " + Ccurrent + "I, all paths should add up to the same, this error check isnt conprohensive, please double check the values"));
        }

        if (foundErrors.Count != 0)
        {
            Debug.Log(transform.Find("/UI/ErrorsPanel").name);
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;
        }


















        transform.Find("/UI/SaveDiagram").GetComponent<SaveFileWindow>().intialiseSaveWindow(diagramData);


        foreach (var data in diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join("",
data.Value
.ConvertAll(i => i.name.ToString())
.ToArray()));





        }
    }

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

    private CircuitComponent getCircuitComponent(DiagramComponent d)
    {
        foreach (CircuitComponent circuitComponent in transform.GetComponentsInChildren<CircuitComponent>())
        {
            if (circuitComponent.conponent == d)
            {
                return circuitComponent;
            }
        }
        return null;
    }



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

    private bool isNodeOutput(Node n)
    {
        CircuitComponent c = n.GetComponentInParent<CircuitComponent>();
        DiagramComponent d = c.conponent;
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

    private void updateAllWireConnections()
    {
        List<Wire> allWire = new List<Wire>(transform.GetComponentsInChildren<Wire>());
        foreach (Wire w in allWire)
        {
            w.updateWireConnections();
        }
    }



    private void SetCircuitConnections()
    {
        foreach (CircuitComponent circuitComponent in gameObject.GetComponentsInChildren<CircuitComponent>())
        {
            foreach (Node n in new Node[2] { circuitComponent.nodeA, circuitComponent.nodeB })
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

            SetComponentConnections(circuitComponent);
            DiagramComponent component = circuitComponent.conponent;
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
        correctDirection();

    }

    private void SetComponentConnections(CircuitComponent circuitComponent)
    {
        DiagramComponent component = circuitComponent.conponent;
        Debug.Log(circuitComponent.gameObject);
        component.Aconnections = GetConnectedComponents(circuitComponent.nodeA, isNodeOutput(circuitComponent.nodeA));
        component.Bconnections = GetConnectedComponents(circuitComponent.nodeB, isNodeOutput(circuitComponent.nodeB));


    }

    private List<DiagramComponent> GetConnectedComponents(Node node, bool isOutput)
    {
        List<DiagramComponent> connectedComponents = new List<DiagramComponent>();
        List<Wire> wiresToTest = new List<Wire>();
        List<Wire> vistedWires = new List<Wire>();
        wiresToTest.Add(node.ConnectedWire);
        while (wiresToTest.Count > 0)
        {

            Wire w = wiresToTest[0];
            foreach (Node n in w.connectedNode)
            {
                if (!connectedComponents.Contains(n.GetComponentInParent<CircuitComponent>().conponent) && n != node)
                {
                    if (isCorrectNode(n, isOutput))
                    {
                        connectedComponents.Add(n.GetComponentInParent<CircuitComponent>().conponent);
                    }
                }
            }

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



    private void correctDirection()
    {
        //for each component
        foreach (CircuitComponent circuitComponent in transform.GetComponentsInChildren<CircuitComponent>())
        {
            DiagramComponent c = circuitComponent.conponent;
            if (c.type != ComponentType.CELL)
            {
                //for each output connected
                foreach (DiagramComponent d in getConnectionsOfDirection(c, true).ToArray())
                {
                    if (d.type != ComponentType.CELL)
                    {
                        if (!getConnectionsOfDirection(d, false).Contains(c))
                        {
                            getConnectionsOfDirection(c, true).Remove(d);

                        }
                    }
                }
                foreach (DiagramComponent d in getConnectionsOfDirection(c, false).ToArray())
                {
                    if (d.type != ComponentType.CELL)
                    {
                        if (!getConnectionsOfDirection(d, true).Contains(c))
                        {
                            getConnectionsOfDirection(c, false).Remove(c);
                        }
                    }
                    // foreach (Component e in getConnectionsOfDirection(d, true).ToArray())
                    // {
                    //     if (!getConnectionsOfDirection(c, false).Contains(d))
                    //     {
                    //         getConnectionsOfDirection(e, false).Remove(d);
                    //     }
                    // }


                }
            }
        }

    }





}
