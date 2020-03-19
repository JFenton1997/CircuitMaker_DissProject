using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;


public class CircuitComponent : MonoBehaviour
{

    public values values;
    public new CircuitComponentName name;

    private Node nodeA;
    private Node nodeB;


    private void Awake()
    {
        try
        {
            nodeA = transform.GetChild(0).gameObject.GetComponent<Node>();
            nodeB = transform.GetChild(1).gameObject.GetComponent<Node>();
            values = new values();
        }
        catch
        {
            Debug.LogError(this.name + " failed to find nodes");
        }
    }
    // Start is called before the first frame update
}