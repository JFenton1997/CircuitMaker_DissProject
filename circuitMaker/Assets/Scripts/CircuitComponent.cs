using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CircuitComponentBlueprint;

public class CircuitComponent : MonoBehaviour
{

    public new CircuitComponentName name;
    public float current;
    public float voltage;
    public float resistance;

    private Node inNode;
    private Node outNode;


    private void Awake() {
        try{
            outNode = transform.GetChild(0).gameObject.GetComponent<Node>();
            inNode = transform.GetChild(1).gameObject.GetComponent<Node>();
        }catch{
            Debug.LogError(this.name + " failed to find nodes");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}