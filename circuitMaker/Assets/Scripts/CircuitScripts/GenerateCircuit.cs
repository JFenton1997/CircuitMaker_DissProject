using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
/// <summary>
/// class for generating a circuit diagram from a diagramData for viewing problems
/// </summary>
public class GenerateCircuit : MonoBehaviour
{

    public GameObject toCellLocation, toNextLayerLocation, emptyLocation; //gameobjects to build to fill spaces
    Dictionary<int, List<DiagramComponent>> diagramData; //digramData to work with
    private int VertComponentGap = 4, HorizontalComponentGap = 3; // values of have far apart components should be
    public Vector3 CellLocation, ComponentOrigin; // locations to work with
    public GameObject cellPrefab; //prefabs used
    public GameObject wirePrefab;
    public GameObject resistorPrefab;
    [SerializeField]
    public List<List<CircuitComponent>> generatorMatrix; //matrix to store the layout of the entire generated circuit 
    private List<CircuitComponent> createdComponents; //list of call components created so far
    public CircuitComponent cell; //cell component

    private float cellBottom;
    private DiagramComponent tempD;
    private List<DiagramComponent> connectionsA;
    private List<DiagramComponent> connectionsB;

/// <summary>
/// getting digram data from global values, if use debugging file 
/// </summary>
    private void Start()
    {
        CellLocation = transform.position;
        if(GlobalValues.selectedDiagram.diagramData != null){
            GenerateCircuitObject(GlobalValues.selectedDiagram.diagramData);
        }else{
            GenerateCircuitObject(transform.Find("/ProgramMaster").GetComponent<CsvManager>().testRead().diagramData);

        }
        
    }


/// <summary>
/// main method for generating a circuit diagram
/// </summary>
/// <param name="diagramData">diagram data from global Values</param>
    public void GenerateCircuitObject(Dictionary<int, List<DiagramComponent>> diagramData)
    {
        DeletePrevGen(); ///delete any prev generated diagrams, (used for debugging)
        createdComponents = new List<CircuitComponent>(); //initialize list
        this.diagramData = diagramData;
        initialiseMatrix(); //initialize matrix
        generateValues();
        foreach (var data in diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join("",
data.Value
.ConvertAll(i => i.name.ToString())
.ToArray()));
        }

        //for each layer in diagramData
        foreach (var layerComponents in diagramData)
        {
            //for each component going left to right, in the layer
            foreach (DiagramComponent diagramComponent in layerComponents.Value)
            {
                //if not cell and spaceCheck previous layer 
                if (layerComponents.Key > 0) spaceCheck(layerComponents.Key - 1);
                createComponent(layerComponents.Key, diagramComponent); //create component
                if (layerComponents.Key > 0)//if cell
                {
                    tempD = new DiagramComponent();
                    connectionsA = new List<DiagramComponent>();
                    connectionsB = new List<DiagramComponent>();
                    //for each output of the current diagram component
                    foreach (DiagramComponent output in getConnectionsOfDirection(diagramComponent, true).ToArray())
                    {
                        //if not end of diagram
                        if (layerComponents.Key + 1 <= generatorMatrix.Count - 1)
                        {
                            if (diagramData[layerComponents.Key + 1].Contains(output) || output.type == ComponentType.CELL)
                            {
                                //create the output component of current diagram component is a Cell or is on next layer
                                createComponent(layerComponents.Key + 1, output);

                            }
                            else
                            {
                                //add out to bConnections
                                connectionsB.Add(output);
                            }
                        }


                    }
                    //if B connections is more than 0
                    if (connectionsB.Count > 0)
                    {
                        connectionsA.Add(diagramComponent); //add diagram component to input(aconnections)
                        tempD.direction = Direction.A_to_B;//set value
                        tempD.Aconnections = connectionsA;
                        tempD.Bconnections = connectionsB;
                        tempD.name = "toLayer";
                        createComponent(layerComponents.Key + 1, tempD); // create new component with tempD values
                    }

                }

            }
            //create new objects for tempD and connections
            tempD = new DiagramComponent();
            connectionsA = new List<DiagramComponent>();
            connectionsB = new List<DiagramComponent>();
            // if another layer
            if (layerComponents.Key - 1 > 0 && layerComponents.Key < generatorMatrix.Count && diagramData.ContainsKey(layerComponents.Key + 1))
            {
                foreach (DiagramComponent toLayerComp in generatorMatrix[layerComponents.Key - 1].ConvertAll(i => i.component).ToArray()) // for each component on the layer 
                {
                    if (toLayerComp.name == "toLayer") //if component is a toLayer component, create to Layer
                    {
                        if (!diagramData[layerComponents.Key + 1].Contains(getConnectionsOfDirection(toLayerComp, true)[0]))
                        {
                            tempD = new DiagramComponent();
                            tempD.direction = Direction.A_to_B;
                            tempD.Aconnections = toLayerComp.Aconnections;
                            tempD.Bconnections = toLayerComp.Bconnections;
                            tempD.name = "toLayer";
                            spaceCheck(layerComponents.Key, generatorMatrix[layerComponents.Key + 1].ConvertAll(x => x.component).IndexOf(toLayerComp));
                            createComponent(layerComponents.Key + 1, tempD);
                        }

                    }

                }
            }
        }


        removeLastLayer(); //remove last layer as it will be empty
        int layerValue = -1;
        addCellConnection(); //add cell connection wires
        foreach (List<CircuitComponent> circuitComponentLayer in generatorMatrix) //set connections for each component (add wires)
        {
            layerValue++;
            foreach (CircuitComponent c in circuitComponentLayer)
            {

                addComponentEnds(c);
                connectcomponents(c, layerValue);
            }
        }
        //convert wires to local space so the diagram can be moved from origin
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

/// <summary>
/// remove last layer from generator matrix
/// </summary>
    private void removeLastLayer()
    {
        List<CircuitComponent> lastLayer = generatorMatrix[generatorMatrix.Count - 1];
        generatorMatrix.Remove(lastLayer);
        foreach (CircuitComponent c in lastLayer)
        {
            DestroyImmediate(c.transform);
        }

    }


    /// <summary>
    /// creating wires for each type of component in the matrix
    /// </summary>
    /// <param name="c"></param>
    private void addComponentEnds(CircuitComponent c)
    {
        //if to cell, go from top to bottom cell wire
        if (c.name == "toCell" && c.component.type == ComponentType.UNTYPED)
        {
            Vector2 wireAEnd = new Vector2(c.transform.position.x, c.transform.position.y + 2);
            Vector2 wireBEnd = new Vector2(c.transform.position.x, cellBottom);
            GameObject wireA = (GameObject)Instantiate(wirePrefab, wireAEnd, Quaternion.identity, transform);
            wireA.GetComponent<Wire>().createdFromCicuitGen(wireAEnd, wireBEnd);
            return;
        }
        //if to layer, create wire going from though its node locations, acting as a layer skip
        else if (c.name == "toLayer" && c.component.type == ComponentType.UNTYPED)
        {
            Vector2 wireAEnd = new Vector2(c.transform.position.x, c.transform.position.y + 2);
            Vector2 wireBEnd = new Vector2(c.transform.position.x, c.transform.position.y - 2);
            GameObject wireA = (GameObject)Instantiate(wirePrefab, wireAEnd, Quaternion.identity, transform);
            wireA.GetComponent<Wire>().createdFromCicuitGen(wireAEnd, wireBEnd);
            return;
        }

        //if empter space, do nothing
        else if (c.name == "empty" || c.name == "" && c.component.type == ComponentType.UNTYPED)
        {
            return;

        }
        else
        {
            // else, add a wire on top of the node A and node B ready for a vertical or horizontal connection
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

/// <summary>
/// using size and contents of generator matrix, create top and bottom cell connection rails
/// </summary>
    private void addCellConnection()
    {
        Vector2 wireAAEnd = new Vector2(cell.nodeA.transform.position.x,
            cell.nodeA.transform.position.y + 1);
        Vector2 wireABEnd = new Vector2(wireAAEnd.x - 3 + (generatorMatrix[0].Count + 1) * HorizontalComponentGap, wireAAEnd.y);
        Vector2 wireBAEnd = new Vector2(cell.nodeB.transform.position.x, cell.nodeB.transform.position.y - 1
            - ((generatorMatrix.Count - 1) * VertComponentGap));
        int largestRow = 0;
        //find the largest row containing a to cell, else use size of last layer
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

        //build wires using calculated vector2 s
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

/// <summary>
/// fills spaces to match the layer above, row size
/// </summary>
/// <param name="layer">layer to fill up to</param>
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
            circuitComponent.component.name = circuitComponent.name;
            generatorMatrix[layer].Add(circuitComponent);
        }
    }


/// <summary>
/// same as space Check polymorphic method, but take a value of where to fill empty to
/// </summary>
/// <param name="layer"></param>
/// <param name="index"></param>
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
            circuitComponent.component.name = circuitComponent.name;
            generatorMatrix[layer].Add(circuitComponent);
        }
    }



/// <summary>
/// method used to create a new component
/// </summary>
/// <param name="layerValue">int of the current layer</param>
/// <param name="d">the component to build </param>
    private void createComponent(int layerValue, DiagramComponent d)
    {
        GameObject temp;
        CircuitComponent circuitComponent;
        //check the component isnt already built or a cell
        if (createdComponents.ConvertAll(i => i.component).Contains(d) && d.type != ComponentType.CELL)
        {
            return;
        }
        //if layer is 0, build a cell
        if (layerValue == 0)
        {
            temp = (GameObject)Instantiate(cellPrefab, CellLocation, Quaternion.identity, transform);

        }
        //if the d is a cell and not on layer 0, make a toCell component
        else if (layerValue > 0 && d.type == ComponentType.CELL)
        {
            temp = (GameObject)Instantiate(toCellLocation, generateLocation(layerValue), Quaternion.identity, transform);
            circuitComponent = temp.GetComponent<CircuitComponent>();
            circuitComponent.gameObject.name = "toCell" + layerValue;
            circuitComponent.name = "toCell";
            circuitComponent.component.name = circuitComponent.name;
            generatorMatrix[layerValue - 1].Add(circuitComponent);
            return;
        }

        // if d is to layer, make a toLayer, used to add connections over multiple layers
        else if (layerValue > 0 && d.type != ComponentType.CELL && d.name == "toLayer")
        {
            temp = (GameObject)Instantiate(toNextLayerLocation, generateLocation(layerValue), Quaternion.identity, transform);
            circuitComponent = temp.GetComponent<CircuitComponent>();
            circuitComponent.component = d;
            circuitComponent.gameObject.name = "toLayer" + layerValue;
            circuitComponent.name = "toLayer";
            circuitComponent.component.name = circuitComponent.name;
            generatorMatrix[layerValue - 1].Add(circuitComponent);
            return;
        }
        else
        {
            //else build a normal component
            switch (d.type)
            {
                case ComponentType.RESISTOR:
                    temp = (GameObject)Instantiate(resistorPrefab, generateLocation(layerValue), Quaternion.identity, transform);
                    break;
                case ComponentType.LIGHT:
                    temp = (GameObject)Instantiate(resistorPrefab, generateLocation(layerValue), Quaternion.identity, transform);
                    break;
                default:
                    Debug.LogError("unknown");
                    temp = new GameObject("ERROR");
                    break;

            }

        }
        //fill out values from d
        circuitComponent = temp.GetComponent<CircuitComponent>();
        circuitComponent.component = d;
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


/// <summary>
/// initialises genarator matrix
/// </summary>
    private void initialiseMatrix()
    {
        generatorMatrix = new List<List<CircuitComponent>>();

        for (int i = 0; i < diagramData.Count; i++)
        {
            generatorMatrix.Add(new List<CircuitComponent>());
        }
    }

/// <summary>
/// used to calculate location to to build a component
/// </summary>
/// <param name="layerValue">layer to build the component</param>
/// <returns>vector 3 of where to build the new component</returns>
    private Vector3 generateLocation(int layerValue)
    {
        //x
        int x = Mathf.RoundToInt(ComponentOrigin.x + ((generatorMatrix[layerValue - 1].Count) * HorizontalComponentGap));
        //y
        int y = Mathf.RoundToInt(ComponentOrigin.y - ((layerValue - 1) * VertComponentGap));
        return new Vector3Int(x, y, 0);
    }

    /// <summary>
    /// calculate values
    /// </summary>
    private void generateValues()
    {
        int numberOfLayers = diagramData.Count;
        ComponentOrigin = new Vector2(CellLocation.x + HorizontalComponentGap, CellLocation.y);





    }


/// <summary>
/// method used to link component to each other
/// </summary>
/// <param name="a">circuit to link</param>
/// <param name="layerValue">layer the circuit is on</param>
    private void connectcomponents(CircuitComponent a, int layerValue)
    {
        int aIndex = generatorMatrix[layerValue].IndexOf(a);
        Pair<CircuitComponent, int> lowestDiff = new Pair<CircuitComponent, int>(a, aIndex); //go across layer to find the 
        //                                                                                     all components on next layer to connect to in both left and right
        Pair<CircuitComponent, int> HighestDiff = new Pair<CircuitComponent, int>(a, aIndex);
        int itemIndex;
        foreach (DiagramComponent diagramComponent in getConnectionsOfDirection(a.component, true))
        {

            if (diagramComponent.type != ComponentType.CELL)
            {
                itemIndex = generatorMatrix[layerValue + 1].ConvertAll(i => i.component).IndexOf(diagramComponent);
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
            //create wire across all component to connect them

            Vector2 WireA = new Vector2(ComponentOrigin.x + (lowestDiff.b * HorizontalComponentGap), ComponentOrigin.y - 2 - (VertComponentGap * layerValue));
            Vector2 WireB = new Vector2(ComponentOrigin.x + (HighestDiff.b * HorizontalComponentGap), ComponentOrigin.y - 2 - (VertComponentGap * layerValue));


            GameObject wireAB = (GameObject)Instantiate(wirePrefab, WireA, Quaternion.identity, transform);
            wireAB.name = "Wire" + a.name + " connection between" + lowestDiff.a + " " + lowestDiff.b + "       " + HighestDiff.a + " " + HighestDiff.b;
            wireAB.GetComponent<Wire>().createdFromCicuitGen(WireA, WireB);
        }
    }



/// <summary>
/// delete all children of this gen
/// </summary>
    private void DeletePrevGen()
    {
        foreach (RectTransform g in transform.GetComponentInChildren<RectTransform>())
        {
            if (g.parent == transform)
            DestroyImmediate(g.gameObject);


        }
    }

    /// <summary>
    /// gets all desired inputs/outputs of a given component
    /// </summary>
    /// <param name="c">component ot</param>
    /// <param name="output"></param>
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
