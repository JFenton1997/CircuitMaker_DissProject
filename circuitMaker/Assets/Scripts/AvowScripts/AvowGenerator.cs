using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class AvowGenerator : AvowManager
{

    public void testGenerate()
    {
        GenerateAvowDiagram(csv.avowTestRead(), 1);
    }

    public void GenerateAvowDiagram(Dictionary<int, List<DiagramComponent>> diagramData, float scale)
    {
        Debug.Log("GEN");
        Stack<DiagramComponent> conponentsToProcess = new Stack<DiagramComponent>();
        List<AvowConponent> builtAvows = new List<AvowConponent>();


        int prevAvowInRow = 0;
        DiagramComponent firstAvow = diagramData[1][0];
        builtAvows.Add(BuildAvow(firstAvow, Vector2.zero));
        foreach (DiagramComponent layerConponent in diagramData[1])
        {
            Debug.Log(builtAvows.Count + " , " + prevAvowInRow);
            if (layerConponent != firstAvow)
            {
                builtAvows.Add(BuildAvow(builtAvows[prevAvowInRow], 'R', layerConponent, scale));
                prevAvowInRow++;
            }
            foreach (DiagramComponent output in outConponents(layerConponent))
            {
                if (output.type != ComponentType.CELL)
                    conponentsToProcess.Push(output);
            }



        }

        //reverse stack
        Stack<DiagramComponent> tempStack = new Stack<DiagramComponent>();
        foreach (DiagramComponent diagram in conponentsToProcess.ToArray())
        {
            tempStack.Push(conponentsToProcess.Pop());
        }
        conponentsToProcess = tempStack;
        tempStack = new Stack<DiagramComponent>();

        foreach (DiagramComponent d in conponentsToProcess)
        {
            Debug.Log(d.name);
        }

        while (conponentsToProcess.Count > 0)
        {
            DiagramComponent currentAvow = conponentsToProcess.Pop();
            if (!builtAvows.ConvertAll(x => x.component).Contains(currentAvow) && currentAvow.type != ComponentType.CELL)
            {
                AvowConponent inputOfCurrentAvow = builtAvows.Find(x => outConponents(x.component).Contains(currentAvow));
                AvowConponent builtAvow = BuildAvow(inputOfCurrentAvow, 'D', currentAvow, scale);
                builtAvows.Add(builtAvow);
                foreach(AvowConponent a in builtAvows.FindAll(x => outConponents(x.component).Contains(currentAvow))){
                    a.BotConnections.Add(builtAvow);

                }
                tempStack.Clear();
                foreach (DiagramComponent output in outConponents(currentAvow))
                {
                    tempStack.Push(output);

                }
                foreach (DiagramComponent d in tempStack)
                {
                    Debug.Log(d.name);
                }


                foreach (DiagramComponent conponent in tempStack.ToArray())
                {
                    
                    conponentsToProcess.Push(conponent);

                    
                    

                }
            }







        }


    }

    public List<DiagramComponent> outConponents(DiagramComponent diagram)
    {
        if (diagram.direction == Direction.A_to_B)
        {
            return diagram.Bconnections;
        }
        else
        {
            return diagram.Aconnections;
        }
    }




    private AvowConponent BuildAvow(DiagramComponent conponent, Vector2 locationToBuild)
    {
        Vector3 buildPos = new Vector3(locationToBuild.x, locationToBuild.y, 0);
        GameObject AvowObject = (GameObject)Instantiate(avowPrefab, buildPos, Quaternion.identity, transform);
        AvowConponent avow = AvowObject.GetComponent<AvowConponent>();
        avow.component = conponent;
        avow.name = conponent.name;
        AvowObject.name = conponent.name;
        avow.voltage = conponent.Values[ComponentParameter.VOLTAGE].value;
        avow.current = conponent.Values[ComponentParameter.CURRENT].value;
        avow.updateSize(avow.voltage, avow.current);
        return avow;
    }

    private AvowConponent BuildAvow(AvowConponent Original, char direction, DiagramComponent newConponent, float scale)
    {
        Vector2 buildLocation = Vector2.zero;
        Vector2 newAvowSize = new Vector2((float)newConponent.Values[ComponentParameter.CURRENT].value * scale, (float)newConponent.Values[ComponentParameter.VOLTAGE].value * scale);
        switch (direction)
        {
            case 'U':
                buildLocation = new Vector2(Original.nextFreeSlotInSpaceInDirection(direction) + (newAvowSize.x / 2), Original.nextSpaceInDirection(direction) + (newAvowSize.y));
                break;
            case 'D':
                buildLocation = new Vector2(Original.nextFreeSlotInSpaceInDirection(direction) + (newAvowSize.x / 2), Original.nextSpaceInDirection(direction) - (newAvowSize.y / 2));
                break;
            case 'R':
                Debug.Log("\nX :" + Original.nextSpaceInDirection(direction) + "  " + newAvowSize.x + " \nY: " + Original.nextFreeSlotInSpaceInDirection(direction) + "  " + (newAvowSize.y / 2));
                buildLocation = new Vector2(Original.rectTransform.sizeDelta.x / 2 + Original.transform.position.x + (newAvowSize.x / 2), Original.nextFreeSlotInSpaceInDirection(direction) - (newAvowSize.y / 2));
                break;
            case 'L':
                buildLocation = new Vector2(Original.nextSpaceInDirection(direction) + (newAvowSize.x), Original.nextFreeSlotInSpaceInDirection(direction) + (newAvowSize.y / 2));
                break;
            default:
                Debug.LogError("UNKOWN Direction    original:" + Original.name + "  new conponent:" + newConponent.name);
                break;
        }
        //Debug.Log("BUILD NEW AVOW: " + newConponent.name + "   using: " + Original.name + "   new size: " + newAvowSize.ToString() + "  new location: " + buildLocation.ToString());
        return BuildAvow(newConponent, buildLocation);

    }
}
