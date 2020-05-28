﻿using System;
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
    public List<CircuitComponent> allcomponents;
    public HashSet<DiagramError> foundErrors;


    public bool isBuilder = true;



    private char ConpName = 'A';

    private void Start()
    {
        foundErrors = new HashSet<DiagramError>();
        allcomponents = new List<CircuitComponent>();
    }

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



    public void buildCircuitWire()
    {
        GameObject circuitComponentOBJ = (GameObject)Instantiate(circuitWirePrefab, Vector3.zero, Quaternion.identity, transform);
        Wire newcomponent = circuitComponentOBJ.GetComponent<Wire>();
        newcomponent.gameObject.name = "wire";
        newcomponent.createdFromButton();


    }




    public void GenerateDiagramData()
    {
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
            updateAllWireConnections();
            SetCircuitConnections();
        }
        catch
        {
            foundErrors.Add(new DiagramError("NO CONNECTION ERROR  ", "Fail to set up connections, Make sure the Diagram has connections"));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;


        }






        //component with no connections
        foreach (CircuitComponent c in allcomponents.FindAll(x => x.component.Aconnections.Count == 0 || x.component.Bconnections.Count == 0))
        {
            foundErrors.Add(new DiagramError("NO CONNECTION ERROR   ",
            "component " + c.name + " is not fully connected to circuit\nA Connections Found: " + string.Join(" , ", c.component.Aconnections.ConvertAll(x => x.name)) +
            "\nB Connections Found: " + string.Join(" , ", c.component.Bconnections.ConvertAll(x => x.name)) + "\n(no name means no connections.\n"+
            "make sure connections are going into the component and not along the component", c.component, allcomponents));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;

        }
        List<Wire> unconnectedWires = new List<Wire>(GetComponentsInChildren<Wire>());
        if (unconnectedWires.Find(x => x.connectedWires.Count == 0) == unconnectedWires.Find(x => x.connectedNode.Count == 0))
        {
            foundErrors.Add(new DiagramError("UNCONNECTED WIRE ERROR  ", "there are wire or wires with no connections, please delete the wire before saving."));
        }




        //component with incorrect values
        if(isBuilder)
        foreach (CircuitComponent c in allcomponents.FindAll(x => Math.Round(x.component.Values[ComponentParameter.VOLTAGE].value / x.component.Values[ComponentParameter.CURRENT].value
        ,2) != Math.Round(x.component.Values[ComponentParameter.RESISTANCE].value,2)))
        {
            
            if (c.component.type != ComponentType.CELL && c.component.type != ComponentType.UNTYPED)
            {
                Debug.Log(Math.Round(c.component.Values[ComponentParameter.VOLTAGE].value / c.component.Values[ComponentParameter.CURRENT].value
        ,2));
             
                foundErrors.Add(new DiagramError("component VALUE ERROR    ", "the values for " + c.gameObject.name + " dont add up correctly to ohm's law, fix values or use auto feature", c.component, allcomponents));
            }


        }




        diagramData = new Dictionary<int, List<DiagramComponent>>();
        foreach (CircuitComponent d in transform.GetComponentsInChildren<CircuitComponent>())
        {
            DiagramComponent c = d.component;
            c.Aconnections.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));
            c.Bconnections.Sort((x1, x2) => getCircuitComponent(x1).transform.position.x.CompareTo(getCircuitComponent(x2).transform.position.x));

        }
        foreach (CircuitComponent circuitComponent in transform.GetComponentsInChildren<CircuitComponent>())
        {
            DiagramComponent c = circuitComponent.component;

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
                 foundErrors.Add(new DiagramError("INFINITE LOOP CREATED  ", "following connections made an infinite loop, this is caused by having a component connected to itself,"+
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

        if(isBuilder)
        if (diagramData[0][0].Values[ComponentParameter.VOLTAGE].value != voltage || diagramData[0][0].Values[ComponentParameter.CURRENT].value != current)
        {
            foundErrors.Add(new DiagramError("CELL VALUE ERROR ",
            "the cells values dont add up to the rest of the circuit using a basic check, using top for current and leftmost path for voltage, the values should be " +
            Math.Round(voltage,2) + "V    " + Math.Round(current,2)+ "I"));

        }
        // diagramData[0][0].Values[ComponentParameter.VOLTAGE].value = voltage;
        // diagramData[0][0].Values[ComponentParameter.CURRENT].value = current;
        // diagramData[0][0].Values[ComponentParameter.RESISTANCE].value = 0f;

        //box check
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

        //set scale
        float scale =1;
        float smallest = Mathf.Infinity;
        foreach(float lowestVal in allcomponents.ConvertAll(x=>x.component.Values[ComponentParameter.CURRENT].value.CompareTo(
        x.component.Values[ComponentParameter.VOLTAGE].value))){
            if(smallest  <lowestVal){
                smallest = lowestVal;
            }
        }
        scale = 0.001f;
        if(smallest > 0.1){
            scale = 0.1f;
        }
        if(smallest> 1){
            scale = 1f;
        }
        if(smallest >10){
            scale = 10f;
        }
        if(smallest >100){
            scale = 100f;
        }
        if(smallest >1000){
            scale = 1000f;
        }

















        if(isBuilder)
        transform.Find("/UI/SaveDiagram").GetComponent<SaveFileWindow>().intialiseSaveWindow(diagramData,scale);
        else{
            transform.Find("/UI/SolverPanel").GetComponent<SolverScript>().compareAnswers(diagramData);

        }


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
            if (circuitComponent.component == d)
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
        correctDirection();

    }

    private void SetComponentConnections(CircuitComponent circuitComponent)
    {
        DiagramComponent component = circuitComponent.component;
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
                if (!connectedComponents.Contains(n.GetComponentInParent<CircuitComponent>().component) && n != node)
                {
                    if (isCorrectNode(n, isOutput))
                    {
                        connectedComponents.Add(n.GetComponentInParent<CircuitComponent>().component);
                    }

                            

                    
                }
                                    if(!vistedWires.Contains(n.ConnectedWire)){
                        wiresToTest.Add(n.ConnectedWire);
                        vistedWires.Add(n.ConnectedWire);
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
            DiagramComponent c = circuitComponent.component;
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
