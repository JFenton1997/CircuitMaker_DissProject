using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class AvowGenerator : AvowManager
{
    public Vector2 startLocation;

    public void testGenerate()
    {
        GenerateAvowDiagram(csv.avowTestRead(), 1);
    }

//create a Avow diagram
    public void GenerateAvowDiagram(Dictionary<int, List<DiagramComponent>> diagramData, float scale)
    {
        //using a depth first methode using a stack
        Stack<DiagramComponent> componentsToProcess = new Stack<DiagramComponent>();
        List<AvowComponent> builtAvows = new List<AvowComponent>();


        int prevAvowInRow = 0;
        DiagramComponent firstAvow = diagramData[1][0];
        builtAvows.Add(BuildAvow(firstAvow, startLocation));
        foreach (DiagramComponent layerComponent in diagramData[1])
        {
            Debug.Log(builtAvows.Count + " , " + prevAvowInRow);
            if (layerComponent != firstAvow)
            {
                builtAvows.Add(BuildAvow(builtAvows[prevAvowInRow], 'R', layerComponent, scale));
                prevAvowInRow++;
            }
            foreach (DiagramComponent output in outComponents(layerComponent))
            {
                if (output.type != ComponentType.CELL)
                    componentsToProcess.Push(output);
            }



        }

        //reverse stack
        Stack<DiagramComponent> tempStack = new Stack<DiagramComponent>();
        foreach (DiagramComponent diagram in componentsToProcess.ToArray())
        {
            tempStack.Push(componentsToProcess.Pop());
        }
        componentsToProcess = tempStack;
        tempStack = new Stack<DiagramComponent>();

        foreach (DiagramComponent d in componentsToProcess)
        {
            Debug.Log(d.name);
        }

        while (componentsToProcess.Count > 0)
        {
            DiagramComponent currentAvow = componentsToProcess.Pop();
            // if not built and not cell
            if (!builtAvows.ConvertAll(x => x.component).Contains(currentAvow) && currentAvow.type != ComponentType.CELL)
            {
                builtAvows.Sort((x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x));
                
                AvowComponent inputOfCurrentAvow = builtAvows.Find(x => outComponents(x.component).Contains(currentAvow))
                    ;
                AvowComponent builtAvow = BuildAvow(inputOfCurrentAvow, 'D', currentAvow, scale);
                if( currentAvow.name == "H"){
                   Debug.Log(inputOfCurrentAvow); 
                    
                }
                builtAvows.Add(builtAvow);
                foreach(AvowComponent a in builtAvows.FindAll(x => outComponents(x.component).Contains(currentAvow))){
                    a.BotConnections.Add(builtAvow);

                }
                tempStack.Clear();
                foreach (DiagramComponent output in outComponents(currentAvow))
                {
                    tempStack.Push(output);

                }
                foreach (DiagramComponent d in tempStack)
                {
                    Debug.Log(d.name);
                }


                foreach (DiagramComponent component in tempStack.ToArray())
                {
                    
                    componentsToProcess.Push(component);

                    
                    

                }
            }







        }


    }

    public List<DiagramComponent> outComponents(DiagramComponent diagram)
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




    private AvowComponent BuildAvow(DiagramComponent component, Vector2 locationToBuild)
    {
        Vector3 buildPos = new Vector3(locationToBuild.x, locationToBuild.y, 0);
        GameObject AvowObject = (GameObject)Instantiate(avowPrefab, buildPos, Quaternion.identity, transform);
        AvowComponent avow = AvowObject.GetComponent<AvowComponent>();
        avow.component = component;
        avow.name = component.name;
        AvowObject.name = component.name;
        avow.voltage = component.Values[ComponentParameter.VOLTAGE].value;
        avow.current = component.Values[ComponentParameter.CURRENT].value;
        avow.updateSize(avow.voltage, avow.current);
        return avow;
    }

    private AvowComponent BuildAvow(AvowComponent Original, char direction, DiagramComponent newComponent, float scale)
    {
        Vector2 buildLocation = Vector2.zero;
        Vector2 newAvowSize = new Vector2((float)newComponent.Values[ComponentParameter.CURRENT].value * scale, (float)newComponent.Values[ComponentParameter.VOLTAGE].value * scale);
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
                Debug.LogError("UNKOWN Direction    original:" + Original.name + "  new component:" + newComponent.name);
                break;
        }
        //Debug.Log("BUILD NEW AVOW: " + newComponent.name + "   using: " + Original.name + "   new size: " + newAvowSize.ToString() + "  new location: " + buildLocation.ToString());
        return BuildAvow(newComponent, buildLocation);

    }
}
