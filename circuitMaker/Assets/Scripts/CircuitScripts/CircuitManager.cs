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



    private char ConpName = 'A';

    private void Start()
    {
        allConponents = new List<CircuitComponent>();
    }

    public void buildCircuitConponent()
    {
        GameObject circuitComponentOBJ = (GameObject)Instantiate(circuitComponentPrefab, Vector3.zero, Quaternion.identity, transform);
        CircuitComponent newConponent = circuitComponentOBJ.GetComponent<CircuitComponent>();

        newConponent.name = "CELL";
        circuitComponentOBJ.name =  "CELL";
        newConponent.name =  "CELL";
        allConponents.Add(newConponent);
    if(allConponents.FindAll(x=>x.conponent.type == ComponentType.CELL).Count >1){
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
        updateAllWireConnections();
        SetCircuitConnections();
        if(allConponents.FindAll(x=> x.conponent.type == ComponentType.CELL).Count != 1){
            Debug.LogError("ONLY 1 CELL");
            return;

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
            if (layerValue > 100)
            {
                Debug.LogError("infinteLoop");
                break;
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
                            Debug.Log("remove called on:" + c + " layer:" + layerValue + " currentComponent:" + currentComponent);
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

    private void updateAllWireConnections(){
        List<Wire> allWire = new List<Wire>(transform.GetComponentsInChildren<Wire>());
        foreach(Wire w in allWire){
            w.updateWireConnections();
        }
    }



    private void SetCircuitConnections()
    {
        foreach (CircuitComponent circuitComponent in gameObject.GetComponentsInChildren<CircuitComponent>())
        {
            foreach (Node n in new Node[2] { circuitComponent.nodeA, circuitComponent.nodeB })
            {
                foreach (Collider2D collider in Physics2D.OverlapCircleAll(n.transform.position, (n.GetComponent<RectTransform>().sizeDelta.x/4 )*n.GetComponent<RectTransform>().localScale.x ))
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
        Debug.Log(node.name + "  "+ node.transform.parent.name + "  "+ isOutput);
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
