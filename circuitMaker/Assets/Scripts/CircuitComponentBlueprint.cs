using UnityEngine;
using System.Collections;

[System.Serializable]
public class CircuitComponentBlueprint {

    public enum CircuitComponentName{
        Cell = 0,
        Light = 1,
        Resister = 2,
        Switch = 3

    }


	public GameObject prefab;
    public CircuitComponentName name;
    public float voltage;
    public float current;
    public float resistance;


}
