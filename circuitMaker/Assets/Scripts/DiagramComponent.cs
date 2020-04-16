using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[System.Serializable]
public class DiagramComponent
{
    public string name;
    public ComponentType type;
    public List<DiagramComponent> Aconnections, Bconnections;
    public Dictionary<ComponentParameter,Pair> Values;
    public Direction direction;

    public DiagramComponent() {
        Aconnections = new List<DiagramComponent>();
        Bconnections = new List<DiagramComponent>();
        Values = new Dictionary<ComponentParameter, Pair>();
        Values.Add(ComponentParameter.VOLTAGE,new Pair(1,false));
        Values.Add(ComponentParameter.CURRENT,new Pair(1,false));
        Values.Add(ComponentParameter.RESISTANCE,new Pair(1,false)); 
        
        
    }


    
}
