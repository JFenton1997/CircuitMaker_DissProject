using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Wire))]
public class DrawWire : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Wire wire;
    public GameObject hit;
    private BoxCollider2D boxCollider;
    public CircuitComponentBlueprint buildWire;
    private bool drawingLine;
    // Start is called before the first frame update
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

    // Update is called once per frame
    private void Update()
    {
        if (drawingLine)
        {
            lineRenderer.SetPosition(1, createWireSegment());
            RaycastHit2D raycast = createRaycast();
            if (isValid(raycast))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    drawingLine = false;
                    Debug.Log(drawingLine);
                    if (raycast)
                    {
                        Debug.Log(hit);
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
                    }
                    else
                    {
                        GameObject newWire = (GameObject)Instantiate(buildWire.prefab, lineRenderer.GetPosition(1), Quaternion.identity, transform.parent);
                        newWire.name = buildWire.name.ToString();
                        newWire.GetComponent<Wire>().createdFromUnconnectedWire();
                        newWire.GetComponent<Wire>().addConnection(wire);
                        wire.addConnection(newWire.GetComponent<Wire>());
                    }
                    updateBoxCollider();
                    SendMessage("drawingEnded");

                }
            }


            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("delete wire");
                Destroy(gameObject);
            }
        }
    }






    private Vector3 createWireSegment()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        Vector3 lastPos = lineRenderer.GetPosition(0);
        Vector3 newPos;
        float deltaX = Mathf.Abs(curPosition.x - lastPos.x);
        float deltaY = Mathf.Abs(curPosition.y - lastPos.y);
        if (deltaX > deltaY)
        {
            newPos = new Vector3(Mathf.RoundToInt(curPosition.x), lastPos.y, 0);
        }
        else
        {
            newPos = new Vector3(lastPos.x, Mathf.RoundToInt(curPosition.y), 0);
        }
        return newPos;
    }



    private RaycastHit2D createRaycast()
    {
        Vector2 checkStart = lineRenderer.GetPosition(0);
        Vector2 checkEnd = lineRenderer.GetPosition(1);
        RaycastHit2D hitInfo = Physics2D.Raycast(checkStart, checkEnd - checkStart, Vector3.Magnitude(checkEnd - checkStart));
        return hitInfo;
    }

    private bool isValid(RaycastHit2D raycast)
    {
        if (raycast)
        {
            hit = raycast.transform.gameObject;
            string objectTag = hit.tag;
            if (objectTag == "CircuitComponent")
            {
                lineRenderer.SetColors(Color.red, Color.red);
                lineRenderer.SetPosition(1,raycast.point);
                return false;
            }
            else
            {
                lineRenderer.SetColors(Color.black, Color.black);
                lineRenderer.SetPosition(1,raycast.point);
                return true;
            }
        }
        lineRenderer.SetColors(Color.black, Color.black);
        return true;


    }


    private void updateBoxCollider()
    {
        Debug.Log(boxCollider.size.ToString());
        Vector3 lineStart = lineRenderer.GetPosition(0);
        Vector3 lineEnd = lineRenderer.GetPosition(1);
        float deltaX = lineStart.x - lineEnd.x;
        float deltaY = lineStart.y - lineEnd.y;
        Debug.Log(deltaX + "  " + deltaY);
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

