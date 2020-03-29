using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Component : MonoBehaviour
{
    public ComponentType type;
    public List<Component> Aconnections, Bconnections;
    public Dictionary<ComponentParameter,Pair> Values;
    public Direction direction;

    public  void Awake() {
        Aconnections = new List<Component>();
        Bconnections = new List<Component>();
        Values = new Dictionary<ComponentParameter, Pair>();
        Values.Add(ComponentParameter.VOLTAGE,new Pair(1,false));
        Values.Add(ComponentParameter.CURRENT,new Pair(1,false));
        Values.Add(ComponentParameter.RESISTANCE,new Pair(1,false)); 
        
    }


    
}
