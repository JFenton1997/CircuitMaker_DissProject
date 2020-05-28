﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class AvowManager : MonoBehaviour
{

    public List<AvowComponent> allAvow;
    public GameObject avowPrefab;
    public char currentName;
    public float scale = 1;

    public CsvManager csv;
    public bool isBuilder;

    private HashSet<DiagramError> foundErrors;


    // Start is called before the first frame update
    private void Awake()
    {
        allAvow = new List<AvowComponent>();
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
                if (!allAvow.Contains(t.GetComponent<AvowComponent>()))
                {
                    allAvow.Add(t.GetComponent<AvowComponent>());
                }

            }
        }
        foreach (AvowComponent avow in allAvow)
        {
            avow.clearConnections();
        }

        foreach (AvowComponent avow in allAvow)
        {
            avow.updateConnections();
        }
        foreach (AvowComponent avow in allAvow)
        {
            avow.updateSameLayerConnections();
        }
    }

    public void buildAvow()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        GameObject avowObject = (GameObject)Instantiate(avowPrefab, new Vector3(curPosition.x, curPosition.y + 5f, 0), Quaternion.identity, transform);
        AvowComponent avowComponent = avowObject.GetComponent<AvowComponent>();
        avowObject.name = currentName.ToString();
        avowComponent.component.type = ComponentType.RESISTOR;
        avowComponent.component.Values[ComponentParameter.VOLTAGE].hidden = false;
        avowComponent.component.Values[ComponentParameter.CURRENT].hidden = false;
        avowComponent.component.Values[ComponentParameter.RESISTANCE].hidden = false;
        currentName++;
        allAvow.Clear();

        foreach (Transform t in transform.GetComponentInChildren<Transform>())
        {
            if (t.parent == transform)
            {
                if (!allAvow.Contains(t.GetComponent<AvowComponent>()))
                {
                    allAvow.Add(t.GetComponent<AvowComponent>());
                }
            }
        }
    }

    public void removeAvow(AvowComponent avowToDestroy)
    {
        allAvow.Clear();
        foreach (Transform t in transform.GetComponentInChildren<Transform>())
        {
            if (t.parent == transform && t == avowToDestroy.transform)
            {
                if (!allAvow.Contains(t.GetComponent<AvowComponent>()))
                {
                    allAvow.Add(t.GetComponent<AvowComponent>());
                    t.GetComponent<AvowComponent>().removeAvowConnection(avowToDestroy);
                }

            }
        }

    }



    public void updateAllValue()
    {
        foreach (AvowComponent avow in allAvow)
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
        foundErrors = new HashSet<DiagramError>();

        if (allAvow.Count == 0)
        {
            foundErrors.Add(new DiagramError("NO componentS FOUND", "theres no components to use to create a diagram"));
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;


        }
        updateConnections();
        // update Diagram component of the Avows
        updateAllValue();
        //initialise Lists values
        Dictionary<int, List<DiagramComponent>> diagramData = new Dictionary<int, List<DiagramComponent>>(); //diagram datatype
        List<DiagramComponent> listOfcomponents = new List<DiagramComponent>(); // used to insert data into diagram data
        List<DiagramComponent> visitedcomponents = new List<DiagramComponent>(); // keep track of prev
        //adding CELL into layer 0
        int layerNumber = 0;
        listOfcomponents.Add(new DiagramComponent());
        listOfcomponents[0].name = "CELL";
        listOfcomponents[0].type = ComponentType.CELL;
        listOfcomponents[0].direction = Direction.B_to_A;
        diagramData.Add(layerNumber, listOfcomponents);
        layerNumber++;
        listOfcomponents = new List<DiagramComponent>();
        //getting layer 1 = all AVOW with no top connections
        listOfcomponents = allAvow.ConvertAll(x => x.component).FindAll(x => x.Aconnections.Count == 0);
        diagramData.Add(layerNumber, listOfcomponents);
        visitedcomponents.AddRange(listOfcomponents);
        //keep looping until next layer has nothing i.e. end of diagram
        int j = 0;
        while (diagramData[layerNumber].Count != 0)
        {
            j++;


            listOfcomponents = new List<DiagramComponent>();
            //for each Avow on layer 
            foreach (DiagramComponent diagramComponent in diagramData[layerNumber].ToArray())
            {
                //for each bot connection
                foreach (DiagramComponent downConnected in diagramComponent.Bconnections.ToArray())
                {
                    //if visited before, remove visisted from prev layers
                    if (visitedcomponents.Contains(downConnected))
                    {
                        Debug.Log("VISITED BEFORE: " + downConnected.name);
                        foreach (var keyValuePair in diagramData)
                        {
                            keyValuePair.Value.Remove(downConnected);
                        }
                    }

                    //add component into visisted
                    visitedcomponents.Add(downConnected);

                    // if not currenlty visisted this layer, add to next layer
                    if (!listOfcomponents.Contains(downConnected)) listOfcomponents.Add(downConnected);


                }

            }
            // incase of infinite loop
            if (j > 1000)
            {
                Debug.LogError("infi while");
                break;
            }
            //insert into dictionary
            layerNumber++;
            diagramData.Add(layerNumber, listOfcomponents);

        }


        // remove last layer, due to it being empty

        diagramData.Remove(layerNumber);

        //debug info for generated data
        foreach (var data in diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join("",
data.Value
.ConvertAll(i => i.name.ToString())
.ToArray()));

        }


        //Final Checks
        //if  2 AVOWs exist with no connections, or 1 Avow with a large diagramData (more than just 1 element), then a avow is unconnected
        List<AvowComponent> unconnected = allAvow.FindAll(x => x.TopConnections.Count == 0 && x.BotConnections.Count == 0 &&
        x.LeftConnections.Count == 0 && x.RightConnections.Count == 0);
        if (unconnected.Count > 1 || (unconnected.Count == 1 && diagramData[1].Count > 1))
        {
            if(isBuilder)
            foreach (AvowComponent avow in unconnected)
            {
                foundErrors.Add(new DiagramError("   UNCONNECTED   ", "The Avow " + avow.gameObject.name + " is seen as unconnected, please either delete the avow or make sure it is connected", avow.component, allAvow));

            }


        }

        foreach (AvowComponent avow in allAvow.FindAll(x => x.isBlocked == true))
        {
            foundErrors.Add(new DiagramError("   BLOCKED   ", "The Avow " + avow.gameObject.name + " is seen as Blocked, please either delete the avow or Move it", avow.component, allAvow));

        }
        //check for box;
        // Vector3 tl = allAvow.Find(x => x.component == diagramData[1][0]).transform.position;
        // Vector3 tr = allAvow.Find(x => x.component == diagramData[1][diagramData[1].Count-1]).transform.position;
        // Vector3 bl = allAvow.Find(x => x.component == diagramData[diagramData.Count-1][0]).transform.position;
        // Vector3 br = allAvow.Find(x => x.component == diagramData[diagramData.Count-1][diagramData[diagramData.Count-1].Count-1]).transform.position;







        //ERRORS DISPLAY IF ANY







        //adding cell connectionions
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
        float voltage = 0;
        float current = 0;
        foreach (DiagramComponent d in diagramData[1])
        {
            current += d.Values[ComponentParameter.CURRENT].value;
        }
        bool endOfDiagram = false; ;
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



        diagramData[0][0].Values[ComponentParameter.VOLTAGE].value = voltage;
        diagramData[0][0].Values[ComponentParameter.CURRENT].value = current;
        diagramData[0][0].Values[ComponentParameter.RESISTANCE].value = 0f;

        //box check
        float Cvoltage = 0;
        float Ccurrent = 0;
        foreach (DiagramComponent d in diagramData[0][0].Bconnections)
        {
            Ccurrent += d.Values[ComponentParameter.CURRENT].value;
        }
        endOfDiagram = false;
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

        Debug.Log("CELL GEN = "+Ccurrent + " " + current + "  " + voltage + "  " + Cvoltage+ "  s:"+ scale);
        if (Cvoltage != voltage || Ccurrent != current)
        {
            foundErrors.Add(new DiagramError("  Layout Error   ", "A Avow must be a rectangle in shape. e.g. must be a complete box with no gaps and exactly 4 sides"));
        }




        if (foundErrors.Count != 0)
        {
            Debug.Log(transform.Find("/UI/ErrorsPanel").name);
            transform.Find("/UI/ErrorsPanel").GetComponent<ErrorPanel>().displayErrors(foundErrors);
            return;
        }


        diagramData[0][0].Values[ComponentParameter.VOLTAGE].value = voltage;
        diagramData[0][0].Values[ComponentParameter.CURRENT].value = current;
        diagramData[0][0].Values[ComponentParameter.RESISTANCE].value = 0f;








        // submit avow diagram to save window
        if(isBuilder)
        transform.Find("/UI/SaveDiagram").GetComponent<SaveFileWindow>().intialiseSaveWindow(diagramData,scale);
        else{
            transform.Find("/UI/SolverPanel").GetComponent<SolverScript>().compareAnswers(diagramData);
        }





    }

    public void updateScale(Dropdown dropdown)
    {
        switch (dropdown.value)
        {

            case 0:
                scale = 0.01f;
                break;
            case 1:
                scale = 0.1f;
                break;
            case 2:
                scale = 1f;
                break;
            case 3:
                scale = 10f;
                break;
            case 4:
                scale = 100f;
                break;
            case 5:
                scale = 1000f;
                break;
            default:
                Debug.Log("invalid scale value");
                break;

        }
    }


}




