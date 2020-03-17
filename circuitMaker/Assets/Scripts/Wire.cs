using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public List<Wire> connectedWires;
    public List<Node> connectedNode;
    private DrawWire drawWire;
    private SpriteRenderer spriteRenderer;
    private GridMove gridMove;



    private void Awake() {
        connectedWires = new List<Wire>();
        connectedNode = new List<Node>();
        drawWire = this.GetComponent<DrawWire>();
        gridMove = this.GetComponent<GridMove>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void createdFromButton(){
        Debug.Log("triggered");
        spriteRenderer.enabled= true;
        bool isMoving = true;
        gridMove.enabled = true;
        gridMove.MoveStart();

    }

    public void createdFromUnconnectedWire(){
        spriteRenderer.enabled= false;
        drawWire.enabled=true;
        drawWire.StartDrawingLine();

    }

    public void undo(){

    }
    public void addConnection(Node n){
        connectedNode.Add(n);
    }

    public void addConnection(Wire w){
        connectedWires.Add(w);
    }

    public void removeConnection(Wire w){
        connectedWires.Remove(w);
    }

    public void removeConnection(Node n){
        connectedNode.Remove(n);
    }
    public void gridMoveEnded(){
        gridMove.enabled = false;
        createdFromUnconnectedWire();
    }

    public void drawingEnded(){
        drawWire.enabled = false;
    }

    private void OnDestroy() {
        Debug.Log("woot");
        foreach(Wire w in connectedWires){
            w.removeConnection(this);
        }
        foreach(Node n in connectedNode){
            n.updateWire(null);
        }
    }
}
