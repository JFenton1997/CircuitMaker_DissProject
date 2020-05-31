using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// node class for all operation dealing with nodes
/// </summary>
public class Node : MonoBehaviour
{
    private CircuitComponent circuitComponent; //perant

    public Wire ConnectedWire;//the wire connected to the node if there is one
/// <summary>
/// get perant
/// </summary>
    private void Awake() {
        circuitComponent = transform.GetComponentInParent<CircuitComponent>();
    }

//update wireConnected
    public void updateWire(Wire w){
        ConnectedWire = w;
    }


}

