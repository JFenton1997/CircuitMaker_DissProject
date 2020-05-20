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
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;



    private void Awake()
    {
        connectedWires = new List<Wire>();
        connectedNode = new List<Node>();
        drawWire = this.GetComponent<DrawWire>();
        gridMove = this.GetComponent<GridMove>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void createdFromButton()
    {
        Debug.Log("triggered");
        spriteRenderer.enabled = true;
        gridMove.enabled = true;
        gridMove.MoveStart();

    }

    public void createdFromUnconnectedWire()
    {
        spriteRenderer.enabled = false;
        drawWire.enabled = true;
        drawWire.StartDrawingLine();

    }

    private void updateBoxCollider()
    {
        Vector3 lineStart = lineRenderer.GetPosition(0);
        Vector3 lineEnd = lineRenderer.GetPosition(1);
        float deltaX = lineStart.x - lineEnd.x;
        float deltaY = lineStart.y - lineEnd.y;
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            boxCollider.offset = new Vector2(deltaX / -2, 0);
            boxCollider.size = new Vector2(Mathf.Abs(deltaX), 0.1f);
        }
        else
        {
            boxCollider.offset = new Vector2(0, deltaY / -2);
            boxCollider.size = new Vector2(0.1f, Mathf.Abs(deltaY));

        }
        
    }



    public void createdFromCicuitGen(Vector2 a, Vector2 b)
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        Vector3 [] linePositions = { new Vector3(a.x,a.y,0), new Vector3(b.x,b.y,0)}; 
        lineRenderer.SetPositions(linePositions);
        lineRenderer.enabled= true;
        updateBoxCollider();
        Collider2D[] aHits =Physics2D.OverlapBoxAll(a,new Vector2(0.1f,0.1f),0f);
        Collider2D[] bHits =Physics2D.OverlapBoxAll(b,new Vector2(0.1f,0.1f),0f);
        Collider2D[] allHits= new Collider2D[aHits.Length+bHits.Length];
        aHits.CopyTo(allHits,0);
        bHits.CopyTo(allHits,aHits.Length);
        foreach(Collider2D collider in allHits){
            if(collider.transform.TryGetComponent<Node>(out Node foundNode)){
                foundNode.ConnectedWire = this;
                addConnection(foundNode);
            }
            else if (collider.transform.TryGetComponent<Wire>(out Wire foundWire)){
                foundWire.addConnection(this);
                addConnection(foundWire);
            }
        }

    }


    public void undo()
    {

    }
    public void addConnection(Node n)
    {
        connectedNode.Add(n);
    }

    public void addConnection(Wire w)
    {
        if(!connectedWires.Contains(w) && w != this) connectedWires.Add(w);
    }

    public void removeConnection(Wire w)
    {
        connectedWires.Remove(w);
    }

    public void removeConnection(Node n)
    {
        connectedNode.Remove(n);
    }
    public void GridMoveEnded()
    {
        gridMove.enabled = false;
        createdFromUnconnectedWire();
    }

    public void EndWireDraw()
    {
        drawWire.enabled = false;
    }

    private void OnDestroy()
    {
        foreach (Wire w in connectedWires)
        {
            w.removeConnection(this);
        }
        foreach (Node n in connectedNode)
        {
            n.updateWire(null);
        }
    }

}
