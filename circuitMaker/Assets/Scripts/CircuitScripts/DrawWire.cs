using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Wire))]

/// <summary>
/// class for controlling the drawing and connections of wires
/// </summary>
public class DrawWire : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Wire wire;
    private GameObject hit;
    private BoxCollider2D boxCollider;
    public CircuitComponentBlueprint buildWire;
    private bool drawingLine;
    // Start is called before the first frame update


    /// <summary>
    /// invoke method starts drawing, set value ready for linerender for drawing
    /// </summary>
    public void StartDrawingLine()
    {
        wire = this.GetComponent<Wire>();
        boxCollider = GetComponent<BoxCollider2D>();
        drawingLine = true;
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
        lineRenderer.SetPositions(new Vector3[2] { transform.position, transform.position });

    }

    /// <summary>
    /// used when drawing wire, running each frame
    /// </summary>
    [System.Obsolete]
    private void Update()
    {
        if (drawingLine) // if drawing
        {
            lineRenderer.SetPosition(1, createWireSegment()); // set second position of the line renderer
            RaycastHit2D raycast = createRaycast(); // raycast, a line to match line rendered
            if (isValid(raycast))//check if raycast hits only valid
            {
                if (Input.GetMouseButtonDown(0)) //on placement left click
                {
                    drawingLine = false; //stop drawing
                    if (raycast) //if raycast hit
                    {
                        //if hit is a wire, connections between wires, if hit node, add connections between node and wire
                        if (hit.GetComponent<Wire>())
                        {
                            wire.addConnection(hit.GetComponent<Wire>());
                            hit.GetComponent<Wire>().addConnection(wire);
                        }
                        if (hit.GetComponent<Node>())
                        {
                           wire.addConnection(hit.GetComponent<Node>());
                           hit.GetComponent<Node>().updateWire(wire);
                        }
                         transform.parent.GetComponent<CircuitManager>().buildCircuitWire();
                    }
                    else
                    {// if nothing is hit in raycast but is valid, start building a new wire from ending position
                        GameObject newWire = (GameObject)Instantiate(buildWire.prefab, lineRenderer.GetPosition(1), Quaternion.identity, transform.parent);
                        newWire.GetComponent<Wire>().createdFromUnconnectedWire();
                        newWire.GetComponent<Wire>().addConnection(wire);
                        wire.addConnection(newWire.GetComponent<Wire>());
                    }
                    //update box collide to match created linerenderer, used for future raycasts
                    updateBoxCollider();
                    

                }
            }

            //on right click, stop drawing and destroy gameobject
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.visible = true;
                SendMessageUpwards("EndWireDraw");
                Destroy(gameObject);
            }
        }
    }





/// <summary>
/// set second linerenderer position from cursor position, enforce vertical and horizontal lines only rounded
/// </summary>
/// <returns> position of second position following rules</returns>
    private Vector3 createWireSegment()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        Vector3 lastPos = lineRenderer.GetPosition(0);
        Vector3 newPos;
        //checking change in x and y
        float deltaX = Mathf.Abs(curPosition.x - lastPos.x);
        float deltaY = Mathf.Abs(curPosition.y - lastPos.y);
        if (deltaX > deltaY) //if more change in x than y
        {
            newPos = new Vector3(Mathf.RoundToInt(curPosition.x), lastPos.y, 0); //horizontal line
        }
        else
        {
            newPos = new Vector3(lastPos.x, Mathf.RoundToInt(curPosition.y), 0);//vertical line
        }
        return newPos;
    }


    /// <summary>
    /// creating a raycast to check if second position
    /// </summary>
    /// <returns> raycast2D of valid hit</returns>
    private RaycastHit2D createRaycast()
    {
        Vector2 checkStart = lineRenderer.GetPosition(0);
        Vector2 checkEnd = lineRenderer.GetPosition(1);
        //creating raycast
        List <RaycastHit2D> hit = new List<RaycastHit2D> (Physics2D.RaycastAll(checkStart, checkEnd - checkStart, Vector3.Magnitude(checkEnd - checkStart)));
        //if contains a node
        if(hit.Find(x => x.transform.tag == "Node" )){
            return hit.Find(x => x.transform.tag == "Node" );
        }
        //if nothing
        else if (hit.Count == 0){
            return new RaycastHit2D();
        }
        //return normal hit (either wire or invalid hit)
        else{
            return hit[0];

        }
    }

    [System.Obsolete]
    /// <summary>
    /// check if raycast hit is valid
    /// </summary>
    /// <param name="raycast">raycast result from create raycast</param>
    /// <returns>if the hit is valid</returns>
    private bool isValid(RaycastHit2D raycast)
    {
        //if raycast had a hit
        if (raycast)
        {
            hit = raycast.transform.gameObject;
            string objectTag = hit.tag;
            //if the gameobject with tag, circuitComponent had a hit, show isnt valid
            if (objectTag == "CircuitComponent")
            {
                lineRenderer.SetColors(Color.red, Color.red);
                lineRenderer.SetPosition(1, raycast.point);
                return false;
            }
            else
            {   // hit is valid
                lineRenderer.SetColors(Color.black, Color.black);
                lineRenderer.SetPosition(1, raycast.point);
                return true;
            }
        }
        lineRenderer.SetColors(Color.black, Color.black);
        return true;


    }

/// <summary>
/// set new box collider size to match linerenderer
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


    
}

