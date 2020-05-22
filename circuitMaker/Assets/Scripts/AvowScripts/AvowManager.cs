using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class AvowManager : MonoBehaviour
{

    public List<AvowConponent> allAvow;
    public GameObject avowPrefab;
    public char currentName;
    public float scale = 1;

    public CsvManager csv;


    // Start is called before the first frame update
    private void Awake()
    {
        allAvow = new List<AvowConponent>();
        currentName = 'A';

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void updateConnections()
    {

        allAvow.Clear();
        foreach (Transform t in transform.GetComponentInChildren<Transform>())
        {
            if (t.parent == transform)
            {
                if (!allAvow.Contains(t.GetComponent<AvowConponent>()))
                {
                    allAvow.Add(t.GetComponent<AvowConponent>());
                }

            }
        }

        foreach (AvowConponent avow in allAvow)
        {
            avow.updateConnections();
        }
    }

    public void buildAvow()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        GameObject avowObject = (GameObject)Instantiate(avowPrefab, new Vector3(curPosition.x, curPosition.y + 6f, 0), Quaternion.identity, transform);
        AvowConponent avowConponent = avowObject.GetComponent<AvowConponent>();
        avowObject.name = currentName.ToString();
        avowConponent.component.type = ComponentType.RESISTOR;
        avowConponent.component.Values[ComponentParameter.VOLTAGE].hidden = false;
        avowConponent.component.Values[ComponentParameter.CURRENT].hidden = false;
        avowConponent.component.Values[ComponentParameter.RESISTANCE].hidden = false;
        currentName++;
        allAvow.Clear();

        foreach (Transform t in transform.GetComponentInChildren<Transform>())
        {
            if (t.parent == transform)
            {
                if (!allAvow.Contains(t.GetComponent<AvowConponent>()))
                {
                    allAvow.Add(t.GetComponent<AvowConponent>());
                }
            }
        }
    }

    public void removeAvow(AvowConponent avowToDestroy)
    {
        allAvow.Clear();
        foreach (Transform t in transform.GetComponentInChildren<Transform>())
        {
            if (t.parent == transform && t == avowToDestroy.transform)
            {
                if (!allAvow.Contains(t.GetComponent<AvowConponent>()))
                {
                    allAvow.Add(t.GetComponent<AvowConponent>());
                    t.GetComponent<AvowConponent>().removeAvowConnection(avowToDestroy);
                }

            }
        }

    }



    public void updateAllValue()
    {
        foreach (AvowConponent avow in allAvow)
        {
            avow.component.direction = Direction.A_to_B;
            avow.component.Values[ComponentParameter.VOLTAGE].value = avow.voltage * scale;
            avow.component.Values[ComponentParameter.CURRENT].value = avow.current * scale;
            avow.component.Values[ComponentParameter.RESISTANCE].value =
                avow.component.Values[ComponentParameter.VOLTAGE].value
                / avow.component.Values[ComponentParameter.CURRENT].value;
            avow.component.Aconnections = avow.TopConnections.ConvertAll(x => x.component);
            avow.component.Bconnections = avow.BotConnections.ConvertAll(x => x.component);
            avow.component.name = avow.gameObject.name;
        }
    }

    public void GenerateDiagramData()
    {
        if (allAvow.Count == 0)
        {
            Debug.LogError("NOTHING TO SAVE ");
            return;
        }
        // update Diagram Conponent of the Avows
        updateAllValue();
        //initialise Lists values
        List<Pair<AvowConponent, string>> ErrorAvow = new List<Pair<AvowConponent, string>>(); //errors
        Dictionary<int, List<DiagramComponent>> diagramData = new Dictionary<int, List<DiagramComponent>>(); //diagram datatype
        List<DiagramComponent> listOfConponents = new List<DiagramComponent>(); // used to insert data into diagram data
        List<DiagramComponent> visitedConponents = new List<DiagramComponent>(); // keep track of prev
        //adding CELL into layer 0
        int layerNumber = 0;
        listOfConponents.Add(new DiagramComponent());
        listOfConponents[0].name = "CELL";
        listOfConponents[0].type = ComponentType.CELL;
        listOfConponents[0].direction = Direction.B_to_A;
        diagramData.Add(layerNumber, listOfConponents);
        layerNumber++;
        listOfConponents = new List<DiagramComponent>();
        //getting layer 1 = all AVOW with no top connections
        listOfConponents = allAvow.ConvertAll(x => x.component).FindAll(x => x.Aconnections.Count == 0);
        diagramData.Add(layerNumber, listOfConponents);
        visitedConponents.AddRange(listOfConponents);
        //keep looping until next layer has nothing i.e. end of diagram
        while (diagramData[layerNumber].Count != 0)
        {
            listOfConponents = new List<DiagramComponent>();
            //for each Avow on layer 
            foreach (DiagramComponent diagramComponent in diagramData[layerNumber].ToArray())
            {
                //for each bot connection
                foreach (DiagramComponent downConnected in diagramComponent.Bconnections.ToArray())
                {
                    //if visited before, remove visisted from prev layers
                    if (visitedConponents.Contains(downConnected))
                    {
                        Debug.Log("VISITED BEFORE: " + downConnected.name);
                        foreach (var keyValuePair in diagramData)
                        {
                            keyValuePair.Value.Remove(downConnected);
                        }
                    }

                    // if not currenlty visisted this layer, add to next layer
                    if (!listOfConponents.Contains(downConnected)) listOfConponents.Add(downConnected);
                    //add conponent into visisted
                    visitedConponents.Add(downConnected);

                }

            }
            //insert into dictionary
            layerNumber++;
            diagramData.Add(layerNumber, listOfConponents);

        }
        //debug info for generated data
        foreach (var data in diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join("",
data.Value
.ConvertAll(i => i.name.ToString())
.ToArray()));

        }
        // remove last layer, due to it being empty
        Debug.Log(layerNumber + "   " + diagramData.Count);
        diagramData.Remove(layerNumber);

        //Final Checks
        //if  2 AVOWs exist with no connections, or 1 Avow with a large diagramData (more than just 1 element), then a avow is unconnected
        List<AvowConponent> unconnected = allAvow.FindAll(x => x.TopConnections.Count == 0 && x.BotConnections.Count == 0 &&
        x.LeftConnections.Count == 0 && x.RightConnections.Count == 0);
        if (unconnected.Count > 1 || (unconnected.Count == 1 && diagramData[1].Count > 1))
        {
            //error avow
            foreach (AvowConponent avow in unconnected)
            {
                ErrorAvow.Add(new Pair<AvowConponent, string>(avow, "Avow is seen as Unconnected, please delete or make sure the avow is connected"));

            }


        }
        else
        {
            foreach (DiagramComponent d in allAvow.ConvertAll(x => x.component)
                .FindAll(x => x.Bconnections.Count == 0))
            {
                diagramData[0][0].Bconnections.Add(d);
                d.Bconnections.Add(diagramData[0][0]);
            }
            foreach (DiagramComponent d in allAvow.ConvertAll(x => x.component)
                .FindAll(x => x.Aconnections.Count == 0))
            {
                diagramData[0][0].Aconnections.Add(d);
                d.Aconnections.Add(diagramData[0][0]);
            }

            //calculate Cell Values
            double voltage = 0;
            double current = 0;
            foreach (DiagramComponent d in diagramData[1])
            {
                current += d.Values[ComponentParameter.CURRENT].value;
            }
            bool endOfDiagram = false; ;
            foreach (var d in diagramData)
            {
                if (d.Key > 0 && !endOfDiagram)
                {
                    if (d.Value[0].Bconnections.Count == 1 && d.Value[0].Bconnections[0].type == ComponentType.CELL)
                    {
                        endOfDiagram = true;
                    }
                    Debug.Log("cell check,  Found: " + d.Value[0].name + "   volt: " + d.Value[0].Values[ComponentParameter.VOLTAGE].value);
                    voltage += d.Value[0].Values[ComponentParameter.VOLTAGE].value;
                }
            }
            diagramData[0][0].Values[ComponentParameter.VOLTAGE].value = voltage;
            diagramData[0][0].Values[ComponentParameter.CURRENT].value = current;
            diagramData[0][0].Values[ComponentParameter.RESISTANCE].value = 0f;

            // submit avow diagram to save window
            transform.Find("/UI/SaveDiagram").GetComponent<SaveFileWindow>().intialiseSaveWindow(diagramData);

        }



    }

    public void updateScale(Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                scale = 0.001f;
                break;
            case 1:
                scale = 0.01f;
                break;
            case 2:
                scale = 0.01f;
                break;
            case 3:
                scale = 1f;
                break;
            case 4:
                scale = 10f;
                break;
            case 5:
                scale = 100f;
                break;
            case 6:
                scale = 1000f;
                break;
            default:
                Debug.Log("invalid scale value");
                break;

        }
    }


}




