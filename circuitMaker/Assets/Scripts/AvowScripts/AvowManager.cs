using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class AvowManager : MonoBehaviour
{

    private List<AvowConponent> allAvow;
    public GameObject avowPrefab;
    public char currentName;

    public CsvManager csv;


    // Start is called before the first frame update
    private void Awake()
    {
        allAvow = new List<AvowConponent>();
        currentName = 'A';
        GlobalValues.AvowScaler = 1f;

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
            avow.component.Values[ComponentParameter.VOLTAGE].value = avow.voltage;
            avow.component.Values[ComponentParameter.CURRENT].value = avow.current;
            avow.component.Values[ComponentParameter.RESISTANCE].value = avow.voltage / avow.current;
            avow.component.Aconnections = avow.TopConnections.ConvertAll(x => x.component);
            avow.component.Bconnections = avow.BotConnections.ConvertAll(x => x.component);
            avow.component.name = avow.name;
        }
    }

    public void GenerateDiagramData()
    {
        updateAllValue();
        List<Pair<AvowConponent, string>> ErrorAvow = new List<Pair<AvowConponent, string>>();
        Dictionary<int, List<DiagramComponent>> diagramData = new Dictionary<int, List<DiagramComponent>>();
        List<DiagramComponent> listOfConponents = new List<DiagramComponent>();
        List<DiagramComponent> visitedConponents = new List<DiagramComponent>();
        int layerNumber = 0;
        listOfConponents.Add(new DiagramComponent());
        listOfConponents[0].name = "CELL";
        listOfConponents[0].type = ComponentType.CELL;
        listOfConponents[0].direction = Direction.B_to_A;
        diagramData.Add(layerNumber, listOfConponents);
        layerNumber++;
        listOfConponents = new List<DiagramComponent>();
        listOfConponents = allAvow.ConvertAll(x => x.component).FindAll(x => x.Aconnections.Count == 0);
        diagramData.Add(layerNumber, listOfConponents);
        visitedConponents.AddRange(listOfConponents);
        while (diagramData[layerNumber].Count != 0)
        {
            listOfConponents = new List<DiagramComponent>();
            foreach (DiagramComponent diagramComponent in diagramData[layerNumber].ToArray())
            {
                foreach (DiagramComponent downConnected in diagramComponent.Bconnections.ToArray())
                {
                    if (visitedConponents.Contains(downConnected))
                    {
                        foreach (var keyValuePair in diagramData)
                        {
                            keyValuePair.Value.Remove(downConnected);
                        }
                    }
                    visitedConponents.Add(downConnected);
                    if(!listOfConponents.Contains(downConnected))listOfConponents.Add(downConnected);

                }

            }
            layerNumber++;
            diagramData.Add(layerNumber, listOfConponents);
            Debug.Log("Layer " + layerNumber + " size: " + diagramData[layerNumber].Count);



        }
        foreach (var data in diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join("",
data.Value
.ConvertAll(i => i.name.ToString())
.ToArray()));

        }
        diagramData.Remove(layerNumber);
        List<AvowConponent> unconnected = allAvow.FindAll(x => x.TopConnections.Count == 0 && x.BotConnections.Count == 0 &&
        x.LeftConnections.Count == 0 && x.RightConnections.Count == 0);
        if (unconnected.Count > 0 || (unconnected.Count == 1 && diagramData[1].Count > 1))
        {
            foreach (AvowConponent avow in unconnected)
            {
                avow.ColorToParam(Color.red);
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
            Debug.Log(csv.WriteDigram(diagramData, "JamesTest", "GAY"));

        }



    }



}
