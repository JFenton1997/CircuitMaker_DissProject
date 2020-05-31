using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// the core class for Wire in the diagram
/// </summary>
public class Wire : MonoBehaviour
{
    [SerializeField]
    public HashSet<Wire> connectedWires; //storing all unique connected wires
    //public List<Wire> wires; //used for debugging as hashSets cant be seen in inspector
public List<Wire> wires;
    [SerializeField]
    public HashSet<Node> connectedNode; //unique connected nodes

    private DrawWire drawWire; //draw wire class
    private Image image; //image component to show when placing with linerender
    private CircuitClickAndDrag circuitClickAndDrag; //for initial placement
    private LineRenderer lineRenderer; //line rendered to line of the wire
    private BoxCollider2D boxCollider; //box collider to check for connections
    bool EndWireDrawBool; //if not drawing
    private Color normColor; //normal colour


/// <summary>
/// initialize values
/// </summary>
    private void Awake()
    {
        
        gameObject.name = "wire";
        EndWireDrawBool = false;
        wires = new List<Wire>();
        connectedWires = new HashSet<Wire>();
        connectedNode = new HashSet<Node>();
        drawWire = this.GetComponent<DrawWire>();
        circuitClickAndDrag = this.GetComponent<CircuitClickAndDrag>();
        image = this.GetComponent<Image>();
        normColor = image.color;
    }


/// <summary>
/// invoke if created from a button, start movestart for initial placement
/// </summary>
    public void createdFromButton()
    {
        image.enabled = true;
        circuitClickAndDrag.enabled = true;
        circuitClickAndDrag.MoveStart();

    }


/// <summary>
/// if created from a unconnected wire, start wireDraw from last wire
/// </summary>
    public void createdFromUnconnectedWire()
    {
        image.enabled = false;
        drawWire.enabled = true;
        gameObject.name = "wire";
        drawWire.StartDrawingLine();

    }

/// <summary>
/// update box collider size, used for other methods other than wireDraw
/// </summary>
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


/// <summary>
/// UI colouring methods
/// </summary>
    public void toNormalColor() => image.color = normColor;
    public void toErrorColor() => image.color =Color.red;
    public void toConnectedColor() => image.color =Color.green;


/// <summary>
/// if created from box gen
/// </summary>
/// <param name="a">position 0 of the linerender vector2</param>
/// <param name="b">position 1 of the linerenderer vector2</param>
    public void createdFromCicuitGen(Vector2 a, Vector2 b)
    {
        this.GetComponent<Image>().enabled = false; //hide image
        boxCollider = gameObject.GetComponent<BoxCollider2D>();//get values
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        Vector3[] linePositions = { new Vector3(a.x, a.y, 0), new Vector3(b.x, b.y, 0) }; // creating a vector3[] to set positions in line renderer
        lineRenderer.SetPositions(linePositions); //set positions
        lineRenderer.enabled = true;//enable line renderer  to show
        updateBoxCollider();//update box collider

        //using overlap boxes to generate connections in gen
        Collider2D[] aHits = Physics2D.OverlapBoxAll(a, new Vector2(0.1f, 0.1f), 0f);
        Collider2D[] bHits = Physics2D.OverlapBoxAll(b, new Vector2(0.1f, 0.1f), 0f);
        Collider2D[] allHits = new Collider2D[aHits.Length + bHits.Length];
        aHits.CopyTo(allHits, 0);
        bHits.CopyTo(allHits, aHits.Length);
        foreach (Collider2D collider in allHits)
        {
            if (collider.transform.TryGetComponent<Node>(out Node foundNode))
            {
                foundNode.ConnectedWire = this;
                addConnection(foundNode);
            }
            else if (collider.transform.TryGetComponent<Wire>(out Wire foundWire))
            {
                foundWire.addConnection(this);
                addConnection(foundWire);
            }
        }

    }


/// <summary>
/// update connections by processing all hit in the wires box collider
/// </summary>
    public void updateWireConnections(){
        Vector2 a = GetComponent<LineRenderer>().GetPosition(0);
        Vector2 b = GetComponent<LineRenderer>().GetPosition(1);
        Collider2D[] allHits = Physics2D.OverlapBoxAll(new Vector2((a.x+b.x)/2 ,(a.y+b.y)/2),GetComponent<BoxCollider2D>().size,0f );
        foreach (Collider2D collider in allHits)
        {
             if (collider.transform.TryGetComponent<Node>(out Node foundNode))
             {
                 foundNode.ConnectedWire = this;
                 addConnection(foundNode);
             }
            if (collider.transform.TryGetComponent<Wire>(out Wire foundWire))
            {
                foundWire.addConnection(this);
                addConnection(foundWire);
            }
        }
    }


/// <summary>
/// adding a node to connected node
/// </summary>
/// <param name="n">node n</param>
    public void addConnection(Node n)
    {
        connectedNode.Add(n);
    }

/// <summary>
/// adding a wire to connections if it aint already connected
/// </summary>
/// <param name="w"></param>
    public void addConnection(Wire w)
    {
        if (!connectedWires.Contains(w) && w != this) connectedWires.Add(w);
    }

/// <summary>
/// remove wire from connection
/// </summary>
/// <param name="w">wire W to be removed</param>
    public void removeConnection(Wire w)
    {
        connectedWires.Remove(w);
    }

/// <summary>
/// remove a given node from connected nodes
/// </summary>
/// <param name="n">given node n</param>
    public void removeConnection(Node n)
    {
        connectedNode.Remove(n);
    }

    /// <summary>
    /// invoked when intial placement set, start drawWire, sets circuitClickAndDraw to false
    /// </summary>
    public void GridMoveEnded()
    {
        circuitClickAndDrag.enabled = false;
        createdFromUnconnectedWire();
    }

/// <summary>
/// called when finished drawWire, sets drawWire to false
/// </summary>
    public void EndWireDraw()
    {
        drawWire.enabled = false;
        EndWireDrawBool = true;
    }

/// <summary>
/// invoked on object Destory
/// removes self form all nodes and wires connected to this wire
/// </summary>
    private void OnDestroy()
    {
        if(connectedWires.Count>0)
        foreach (Wire w in connectedWires)
        {
            w.removeConnection(this);
        }
         if(connectedNode.Count>0)
        foreach (Node n in connectedNode)
        {
            n.updateWire(null);
        }
        Cursor.visible = true;
    }


/// <summary>
/// used to highlight wire to be deleted by the user on mouse over
/// </summary>
    [System.Obsolete]
    private void OnMouseEnter()
    {


        GetComponent<LineRenderer>().SetColors(Color.gray, Color.gray);
  foreach(Wire w in connectedWires){
            if(!wires.Contains(w))
            wires.Add(w);
        }
        
    }


/// <summary>
/// if mouse if over wire collider and mouse is down, delete gameobject
/// </summary>
    private void OnMouseOver()
    {



        if (Input.GetMouseButton(1))
        {
            GameObject.Destroy(gameObject);
        }

    }



/// <summary>
/// on mouse exit, change colours back to normal
/// </summary>
    [System.Obsolete]
    private void OnMouseExit()


    {
        GetComponent<LineRenderer>().SetColors(Color.black, Color.black);

    }




}
