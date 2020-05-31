using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


/// <summary>
///  sub class of Avow manager, used to create a avow from a diagramData from stored selected in the global values static class
/// </summary>
public class AvowGenerator : AvowManager
{
    public Vector2 startLocation;


/// <summary>
/// runs on first frame active
/// 
/// gets selected file from the global values to generate, if not found load up a test file
/// </summary>
     private void Start()
    {
        startLocation = transform.position;
        if(GlobalValues.selectedDiagram.diagramData != null){
            GenerateAvowDiagram(GlobalValues.selectedDiagram.diagramData,  GlobalValues.selectedDiagram.scale);
        }else{
            GenerateAvowDiagram(transform.Find("/ProgramMaster").GetComponent<CsvManager>().testRead().diagramData,transform.Find("/ProgramMaster").GetComponent<CsvManager>().testRead().scale);

        }
        
    }

/// <summary>
/// main method taking a diagram data to generate
/// </summary>
/// <param name="diagramData"> the data storing the diagram data obtained from CSV</param>
/// <param name="scale"> used to set size of avow object, using the component values, obtained via diagram instance data</param>
    public void GenerateAvowDiagram(Dictionary<int, List<DiagramComponent>> diagramData, float scale)
    {
        //using a modifier depth first method using a stack
        Stack<DiagramComponent> componentsToProcess = new Stack<DiagramComponent>();
        List<AvowComponent> builtAvows = new List<AvowComponent>();
        // setting scale text in problem viewer
        transform.Find("/UI/ProblemDisplayer/ProblemView").GetComponent<ProblemViewer>().showScaleText(scale);

        //building first avow at current position
        int prevAvowInRow = 0;
        DiagramComponent firstAvow = diagramData[1][0];
        //building first avow
        builtAvows.Add(BuildAvow(firstAvow, startLocation,scale));
        // building entire first layer off of the first built avow
        foreach (DiagramComponent layerComponent in diagramData[1])
        {
            Debug.Log(builtAvows.Count + " , " + prevAvowInRow);
            if (layerComponent != firstAvow)
            {
                builtAvows.Add(BuildAvow(builtAvows[prevAvowInRow], 'R', layerComponent, scale));
                prevAvowInRow++;
            }
            //adding bot connections to stack
            foreach (DiagramComponent output in outComponents(layerComponent))
            {
                if (output.type != ComponentType.CELL)
                    componentsToProcess.Push(output);
            }



        }

        //reverse stack to process all b connections from left to right
        Stack<DiagramComponent> tempStack = new Stack<DiagramComponent>();
        foreach (DiagramComponent diagram in componentsToProcess.ToArray())
        {
            tempStack.Push(componentsToProcess.Pop());
        }
        componentsToProcess = tempStack;
        tempStack = new Stack<DiagramComponent>();


        // while Avows still to process in stack
        while (componentsToProcess.Count > 0)
        {
            // pop
            DiagramComponent currentAvow = componentsToProcess.Pop();
            // if not built and not cell
            if (!builtAvows.ConvertAll(x => x.component).Contains(currentAvow) && currentAvow.type != ComponentType.CELL)
            {
                // sort built avows by x position, used to prevent overlaps
                builtAvows.Sort((x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x));
                
                //get the input of the current avow from built components
                AvowComponent inputOfCurrentAvow = builtAvows.Find(x => outComponents(x.component).Contains(currentAvow));
                //build avow below inputOfCurrent
                AvowComponent builtAvow = BuildAvow(inputOfCurrentAvow, 'D', currentAvow, scale);
                //add newly builder to built avows
                builtAvows.Add(builtAvow);
                //add connections to all built avows to the newly built avow which has the newly built avow as a output
                foreach(AvowComponent a in builtAvows.FindAll(x => outComponents(x.component).Contains(currentAvow))){
                    a.BotConnections.Add(builtAvow);

                }
                //clear temp stack, used to get all output connections of the newly built avow
                tempStack.Clear();

                // add each out of newly built to temp stack
                foreach (DiagramComponent output in outComponents(currentAvow))
                {
                    tempStack.Push(output);

                }
                //push contents of tempt stack to top of components to procces. temp stack used to reverse the order
                foreach (DiagramComponent component in tempStack.ToArray())
                {
                    
                    componentsToProcess.Push(component);

                    
                    

                }
            }







        }


    }

/// <summary>
/// return a list of out component of a given diagramComponent
/// </summary>
/// <param name="diagram"> diagram component to get outputs of </param>
/// <returns> list of diagram components which are outputs of diagram</returns>
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



/// <summary>
/// build a avow from a given location, and intialising the values of the new AvowComponent object
/// </summary>
/// <param name="component">diagramComponent to be built as a avow</param>
/// <param name="locationToBuild"> location to build the new avow</param>
/// <param name="scale">used to set the voltage and current and thus the size of the new avow to fit into the correct scale to prevent really small or huge avows</param>
/// <returns>newly built avowComponent</returns>
    private AvowComponent BuildAvow(DiagramComponent component, Vector2 locationToBuild, float scale)
    {
        Vector3 buildPos = new Vector3(locationToBuild.x, locationToBuild.y, 0); //instatiation requires a vector3
        GameObject AvowObject = (GameObject)Instantiate(avowPrefab, buildPos, Quaternion.identity, transform); // build prefab, and cast to a new gameobject to be stored
        AvowComponent avow = AvowObject.GetComponent<AvowComponent>(); // get avow of new gameobject
        
        //filling values out 
        avow.component = component;
        avow.name = component.name;
        AvowObject.name = component.name;
        avow.voltage = (float)Math.Round(component.Values[ComponentParameter.VOLTAGE].value/scale,2);
        avow.current = (float)Math.Round(component.Values[ComponentParameter.CURRENT].value/scale,2);
        avow.updateSize(avow.voltage, avow.current);
        return avow;
    }

/// <summary>
/// gets the correct location to build avow and sends info to other polymorphic method
/// </summary>
/// <param name="Original">built avow object which the new avow will connect to</param>
/// <param name="direction">the direction the new avow will connect to the original </param>
/// <param name="newComponent">the diagram component of the avow to built</param>
/// <param name="scale"> scale as mention in the other polymorphic method</param>
/// <returns></returns>
    private AvowComponent BuildAvow(AvowComponent Original, char direction, DiagramComponent newComponent, float scale)
    {
        Vector2 buildLocation = Vector2.zero;
        // calculate size of new avow to built
        Vector2 newAvowSize = new Vector2((float)newComponent.Values[ComponentParameter.CURRENT].value / scale, (float)newComponent.Values[ComponentParameter.VOLTAGE].value / scale);
        // check direction, runs nextFreeSlotInSpaceInDirection of original to next free space. 
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
        return BuildAvow(newComponent, buildLocation, scale);

    }
}
