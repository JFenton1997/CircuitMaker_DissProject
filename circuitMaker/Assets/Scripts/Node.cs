using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private CircuitComponent circuitComponent;
    public Wire ConnectedWire;
    private void Awake() {
        circuitComponent = transform.GetComponentInParent<CircuitComponent>();
    }

    public void updateWire(Wire w){
        ConnectedWire = w;
    }
}

