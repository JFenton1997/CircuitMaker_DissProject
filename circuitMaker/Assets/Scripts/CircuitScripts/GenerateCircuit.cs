﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GenerateCircuit : MonoBehaviour
{
    public CircuitManager circuitTest;
    public GameObject toCellLocation, toNextLayerLocation, emptyLocation;
    Dictionary<int, List<DiagramComponent>> diagramData;
    private int VertComponentGap = 4, HorizontalComponentGap = 3;
    public Vector3 CellLocation, ComponentOrigin;
    public GameObject cellPrefab;
    public GameObject wirePrefab;
    public GameObject resistorPrefab;
    [SerializeField]
    public List<List<CircuitComponent>> generatorMatrix;
    private List<CircuitComponent> createdComponents;
    public CircuitComponent cell;

    private float cellBottom;
    private DiagramComponent tempD;
    private List<DiagramComponent> connectionsA;
    private List<DiagramComponent> connectionsB;


    private void Start()
    {
        CellLocation = transform.position;
        if(GlobalValues.selectedDiagram.diagramData != null){
            GenerateCircuitObject(GlobalValues.selectedDiagram.diagramData);
        }else{
            GenerateCircuitObject(transform.Find("/ProgramMaster").GetComponent<CsvManager>().testRead().diagramData);

        }
        
    }


    public void GenerateCircuitObject(Dictionary<int, List<DiagramComponent>> diagramData)
    {
        DeletePrevGen();
        createdComponents = new List<CircuitComponent>();
        this.diagramData = diagramData;
        initialiseMatrix();
        generateValues();
        foreach (var data in diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join("",
data.Value
.ConvertAll(i => i.name.ToString())
.ToArray()));
        }

        foreach (var layerComponents in diagramData)
        {
            foreach (DiagramComponent diagramComponent in layerComponents.Value)
            {
                if (layerComponents.Key > 0) spaceCheck(layerComponents.Key - 1);
                createComponent(layerComponents.Key, diagramComponent);
                if (layerComponents.Key > 0)
                {
                    tempD = new DiagramComponent();
                    connectionsA = new List<DiagramComponent>();
                    connectionsB = new List<DiagramComponent>();

                    foreach (DiagramComponent output in getConnectionsOfDirection(diagramComponent, true).ToArray())
                    {
                        if (layerComponents.Key + 1 <= generatorMatrix.Count - 1)
                        {
                            if (diagramData[layerComponents.Key + 1].Contains(output) || output.type == ComponentType.CELL)
                            {
                                createComponent(layerComponents.Key + 1, output);

                            }
                            else
                            {


                                connectionsB.Add(output);


                            }
                        }


                    }
                    if (connectionsB.Count > 0)
                    {
                        connectionsA.Add(diagramComponent);
                        tempD.direction = Direction.A_to_B;
                        tempD.Aconnections = connectionsA;
                        tempD.Bconnections = connectionsB;
                        tempD.name = "toLayer";
                        createComponent(layerComponents.Key + 1, tempD);
                    }

                }

            }
            tempD = new DiagramComponent();
            connectionsA = new List<DiagramComponent>();
            connectionsB = new List<DiagramComponent>();

            if (layerComponents.Key - 1 > 0 && layerComponents.Key < generatorMatrix.Count && diagramData.ContainsKey(layerComponents.Key + 1))
            {
                foreach (DiagramComponent toLayerComp in generatorMatrix[layerComponents.Key - 1].ConvertAll(i => i.conponent).ToArray())
                {
                    if (toLayerComp.name == "toLayer")
                    {
                        if (!diagramData[layerComponents.Key + 1].Contains(getConnectionsOfDirection(toLayerComp, true)[0]))
                        {
                            tempD = new DiagramComponent();
                            tempD.direction = Direction.A_to_B;
                            tempD.Aconnections = toLayerComp.Aconnections;
                            tempD.Bconnections = toLayerComp.Bconnections;
                            tempD.name = "toLayer";
                            spaceCheck(layerComponents.Key, generatorMatrix[layerComponents.Key + 1].ConvertAll(x => x.conponent).IndexOf(toLayerComp));
                            createComponent(layerComponents.Key + 1, tempD);
                        }

                    }

                }
            }
        }


        removeLastLayer();
        int layerValue = -1;
        addCellConnection();
        foreach (List<CircuitComponent> circuitComponentLayer in generatorMatrix)
        {
            layerValue++;
            foreach (CircuitComponent c in circuitComponentLayer)
            {

                addComponentEnds(c);
                connectConponents(c, layerValue);
            }
        }

        convertWiresToLocal();


        for (int i = 0; i < generatorMatrix.Count; i++)
        {

            Debug.Log("layer " + i.ToString() + " : " + string.Join(" ",
generatorMatrix[i]
.ConvertAll(j => j.name.ToString())
.ToArray()));
        }

        //getSizeAndMidPoint
    
        RectTransform rt = GetComponent<RectTransform>();
        List<RectTransform> allChildren = new List<RectTransform> (GetComponentsInChildren<RectTransform>());
        Vector2 tl = CellLocation;

        







    }

    private void removeLastLayer()
    {
        List<CircuitComponent> lastLayer = generatorMatrix[generatorMatrix.Count - 1];
        generatorMatrix.Remove(lastLayer);
        foreach (CircuitComponent c in lastLayer)
        {
            DestroyImmediate(c.transform);
        }

    }
    private void addComponentEnds(CircuitComponent c)
    {

        if (c.name == "toCell" && c.conponent.type == ComponentType.UNTYPED)
        {
            Vector2 wireAEnd = new Vector2(c.transform.position.x, c.transform.position.y + 2);
            Vector2 wireBEnd = new Vector2(c.transform.position.x, cellBottom);
            GameObject wireA = (GameObject)Instantiate(wirePrefab, wireAEnd, Quaternion.identity, transform);
            wireA.GetComponent<Wire>().createdFromCicuitGen(wireAEnd, wireBEnd);
            return;
        }
        else if (c.name == "toLayer" && c.conponent.type == ComponentType.UNTYPED)
        {
            Vector2 wireAEnd = new Vector2(c.transform.position.x, c.transform.position.y + 2);
            Vector2 wireBEnd = new Vector2(c.transform.position.x, c.transform.position.y - 2);
            GameObject wireA = (GameObject)Instantiate(wirePrefab, wireAEnd, Quaternion.identity, transform);
            wireA.GetComponent<Wire>().createdFromCicuitGen(wireAEnd, wireBEnd);
            return;
        }
        else if (c.name == "empty" || c.name == "" && c.conponent.type == ComponentType.UNTYPED)
        {
            return;

        }
        else
        {
            Debug.Log(c);
            Debug.Log(c.nodeA + " " + c.nodeB);
            Vector2 wireAEnd = new Vector2(c.nodeA.transform.position.x, c.nodeA.transform.position.y + 1);
            Vector2 wireBEnd = new Vector2(c.nodeB.transform.position.x, c.nodeB.transform.position.y - 1);
            GameObject wireA = (GameObject)Instantiate(wirePrefab, c.nodeA.transform.position, Quaternion.identity, transform);
            wireA.GetComponent<Wire>().createdFromCicuitGen(c.nodeA.transform.position, wireAEnd);
            GameObject wireB = (GameObject)Instantiate(wirePrefab, c.nodeB.transform.position, Quaternion.identity, transform);
            wireB.GetComponent<Wire>().createdFromCicuitGen(c.nodeB.transform.position, wireBEnd);
        }
    }

    private void addCellConnection()
    {
        Vector2 wireAAEnd = new Vector2(cell.nodeA.transform.position.x,
            cell.nodeA.transform.position.y + 1);
        Vector2 wireABEnd = new Vector2(wireAAEnd.x - 3 + (generatorMatrix[0].Count + 1) * HorizontalComponentGap, wireAAEnd.y);
        Vector2 wireBAEnd = new Vector2(cell.nodeB.transform.position.x, cell.nodeB.transform.position.y - 1
            - ((generatorMatrix.Count - 1) * VertComponentGap));
        int largestRow = 0;
        foreach (List<CircuitComponent> c in generatorMatrix)
        {
            foreach (CircuitComponent d in c)
            {
                if (d.name == "toCell" && c.IndexOf(d) > largestRow)
                {
                    largestRow = c.IndexOf(d);
                }
            }
        }
        if (generatorMatrix[generatorMatrix.Count - 1].Count - 1 > largestRow)
        {
            largestRow = generatorMatrix[generatorMatrix.Count - 1].Count - 1;
        }
        Vector2 wireBBEnd = new Vector2(wireBAEnd.x + ((largestRow + 1) * HorizontalComponentGap), wireBAEnd.y);
        cellBottom = wireBBEnd.y;

        Debug.Log(wirePrefab);
        GameObject wireAA = (GameObject)Instantiate(wirePrefab, cell.nodeA.transform.position, Quaternion.identity, transform);
        Debug.Log(wireAA);
        wireAA.GetComponent<Wire>().createdFromCicuitGen(cell.nodeA.transform.position, wireAAEnd);
        GameObject wireAB = (GameObject)Instantiate(wirePrefab, wireAAEnd, Quaternion.identity, transform);
        wireAB.GetComponent<Wire>().createdFromCicuitGen(wireAAEnd, wireABEnd);
        GameObject wireBA = (GameObject)Instantiate(wirePrefab, cell.nodeB.transform.position, Quaternion.identity, transform);
        wireBA.GetComponent<Wire>().createdFromCicuitGen(cell.nodeB.transform.position, wireBAEnd);
        GameObject wireBB = (GameObject)Instantiate(wirePrefab, wireBAEnd, Quaternion.identity, transform);
        wireBB.GetComponent<Wire>().createdFromCicuitGen(wireBAEnd, wireBBEnd);


    }

    private void spaceCheck(int layer)
    {
        GameObject temp;
        CircuitComponent circuitComponent;
        for (int i = generatorMatrix[layer + 1].Count - generatorMatrix[layer].Count; i > 0; i--)
        {


            temp = (GameObject)Instantiate(emptyLocation, generateLocation(layer + 1), Quaternion.identity, transform);
            circuitComponent = temp.GetComponent<CircuitComponent>();
            circuitComponent.gameObject.name = "empty" + layer;
            circuitComponent.name = "empty";
            circuitComponent.conponent.name = circuitComponent.name;
            generatorMatrix[layer].Add(circuitComponent);
        }
    }


    private void spaceCheck(int layer, int index)
    {
        GameObject temp;
        CircuitComponent circuitComponent;
        for (int i = generatorMatrix[layer + 1].Count - index; i > 0; i--)
        {

            Debug.Log("Empty");
            temp = (GameObject)Instantiate(emptyLocation, generateLocation(layer + 1), Quaternion.identity, transform);
            circuitComponent = temp.GetComponent<CircuitComponent>();
            circuitComponent.gameObject.name = "empty" + layer;
            circuitComponent.name = "empty";
            circuitComponent.conponent.name = circuitComponent.name;
            generatorMatrix[layer].Add(circuitComponent);
        }
    }



    private void createComponent(int layerValue, DiagramComponent d)
    {
        GameObject temp;
        CircuitComponent circuitComponent;
        if (createdComponents.ConvertAll(i => i.conponent).Contains(d) && d.type != ComponentType.CELL)
        {
            return;
        }
        if (layerValue == 0)
        {
            temp = (GameObject)Instantiate(cellPrefab, CellLocation, Quaternion.identity, transform);

        }
        else if (layerValue > 0 && d.type == ComponentType.CELL)
        {
            temp = (GameObject)Instantiate(toCellLocation, generateLocation(layerValue), Quaternion.identity, transform);
            circuitComponent = temp.GetComponent<CircuitComponent>();
            circuitComponent.gameObject.name = "toCell" + layerValue;
            circuitComponent.name = "toCell";
            circuitComponent.conponent.name = circuitComponent.name;
            generatorMatrix[layerValue - 1].Add(circuitComponent);
            return;
        }
        else if (layerValue > 0 && d.type != ComponentType.CELL && d.name == "toLayer")
        {
            temp = (GameObject)Instantiate(toNextLayerLocation, generateLocation(layerValue), Quaternion.identity, transform);
            circuitComponent = temp.GetComponent<CircuitComponent>();
            circuitComponent.conponent = d;
            circuitComponent.gameObject.name = "toLayer" + layerValue;
            circuitComponent.name = "toLayer";
            circuitComponent.conponent.name = circuitComponent.name;
            generatorMatrix[layerValue - 1].Add(circuitComponent);
            return;
        }
        else
        {
            switch (d.type)
            {
                case ComponentType.RESISTOR:
                    temp = (GameObject)Instantiate(resistorPrefab, generateLocation(layerValue), Quaternion.identity, transform);
                    break;
                default:
                    Debug.LogError("unknown");
                    temp = new GameObject("ERROR");
                    break;

            }

        }

        circuitComponent = temp.GetComponent<CircuitComponent>();
        circuitComponent.conponent = d;
        circuitComponent.gameObject.name = d.name;
        circuitComponent.name = d.name;
        createdComponents.Add(circuitComponent);
        if (layerValue == 0)
        {
            cell = circuitComponent;
        }
        else
        {
            generatorMatrix[layerValue - 1].Add(circuitComponent);


        }


    }

    private void initialiseMatrix()
    {
        generatorMatrix = new List<List<CircuitComponent>>();

        for (int i = 0; i < diagramData.Count; i++)
        {
            generatorMatrix.Add(new List<CircuitComponent>());
        }
    }

    private Vector3 generateLocation(int layerValue)
    {
        //x
        int x = Mathf.RoundToInt(ComponentOrigin.x + ((generatorMatrix[layerValue - 1].Count) * HorizontalComponentGap));
        //y
        int y = Mathf.RoundToInt(ComponentOrigin.y - ((layerValue - 1) * VertComponentGap));
        return new Vector3Int(x, y, 0);
    }

    private void generateValues()
    {
        int numberOfLayers = diagramData.Count;
        ComponentOrigin = new Vector2(CellLocation.x + HorizontalComponentGap, CellLocation.y);





    }



    private void connectConponents(CircuitComponent a, int layerValue)
    {
        int aIndex = generatorMatrix[layerValue].IndexOf(a);
        Pair<CircuitComponent, int> lowestDiff = new Pair<CircuitComponent, int>(a, aIndex);
        Pair<CircuitComponent, int> HighestDiff = new Pair<CircuitComponent, int>(a, aIndex);
        int itemIndex;
        foreach (DiagramComponent diagramComponent in getConnectionsOfDirection(a.conponent, true))
        {

            if (diagramComponent.type != ComponentType.CELL)
            {
                itemIndex = generatorMatrix[layerValue + 1].ConvertAll(i => i.conponent).IndexOf(diagramComponent);
                if (itemIndex >= 0)
                {
                    if (itemIndex < lowestDiff.b)
                    {
                        lowestDiff = new Pair<CircuitComponent, int>(generatorMatrix[layerValue + 1][itemIndex], itemIndex);
                    }
                    else if (itemIndex > HighestDiff.b)
                    {
                        HighestDiff = new Pair<CircuitComponent, int>(generatorMatrix[layerValue + 1][itemIndex], itemIndex);
                    }
                }
            }
        }
        if (lowestDiff.b != HighestDiff.b)
        {

            Vector2 WireA = new Vector2(ComponentOrigin.x + (lowestDiff.b * HorizontalComponentGap), ComponentOrigin.y - 2 - (VertComponentGap * layerValue));
            Vector2 WireB = new Vector2(ComponentOrigin.x + (HighestDiff.b * HorizontalComponentGap), ComponentOrigin.y - 2 - (VertComponentGap * layerValue));


            GameObject wireAB = (GameObject)Instantiate(wirePrefab, WireA, Quaternion.identity, transform);
            wireAB.name = "Wire" + a.name + " connection between" + lowestDiff.a + " " + lowestDiff.b + "       " + HighestDiff.a + " " + HighestDiff.b;
            wireAB.GetComponent<Wire>().createdFromCicuitGen(WireA, WireB);
        }
    }



    public void setData()
    {
        this.diagramData = circuitTest.diagramData;
    }

    private void DeletePrevGen()
    {
        foreach (RectTransform g in transform.GetComponentInChildren<RectTransform>())
        {
            if (g.parent == transform) ;
            DestroyImmediate(g.gameObject);


        }
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

    private void convertWiresToLocal()
    {
        foreach (LineRenderer wireLines in (GetComponentsInChildren<LineRenderer>()))
        {
            wireLines.SetPosition(0, wireLines.transform.InverseTransformPoint(wireLines.GetPosition(0)));
            wireLines.SetPosition(1, wireLines.transform.InverseTransformPoint(wireLines.GetPosition(1)));
            wireLines.useWorldSpace = false;

        }

    }

}
