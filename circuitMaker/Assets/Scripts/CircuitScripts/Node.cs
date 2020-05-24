using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private CircuitComponent circuitComponent;
    private RectTransform rect;
    public Wire ConnectedWire;
    private void Awake() {
        rect = GetComponent<RectTransform>();
        circuitComponent = transform.GetComponentInParent<CircuitComponent>();
    }

    public void updateWire(Wire w){
        ConnectedWire = w;
    }

    // private void Update() {
    //     // List<Collider2D> hits = new List<Collider2D>(Physics2D.OverlapCircleAll(new Vector2(transform.position.x,transform.position.y), (rect.sizeDelta.x/4)*rect.localScale.x));
    //     // if(hits.Find(x => x.name == "wire")){
    //     //     ConnectedWire = hits.Find(x => x.name == "wire").GetComponent<Wire>();
    //     //     if(!ConnectedWire.connectedNode.Contains(this))
    //     //     ConnectedWire.connectedNode.Add(this);
    //     //}
        
        
    // }
}

