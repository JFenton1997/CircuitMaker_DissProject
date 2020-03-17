using UnityEngine;
using System.Collections;

[System.Serializable]
public class CircuitComponentBlueprint {

    public enum CircuitComponentName{
        Cell = 0,
        Light = 1,
        Resister = 2,
        Switch = 3,

        Wire = 4

    }


	public GameObject prefab;
    public CircuitComponentName name;



}
