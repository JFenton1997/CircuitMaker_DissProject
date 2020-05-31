using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]

/// <summary>
/// method used for both circuit and wire movement in circuit component builders
/// </summary>
public class CircuitClickAndDrag : MonoBehaviour
{




    public Color dragColor = Color.gray; // colour when moving
    public Color blockedColor = Color.red; // colour when blocked
    private Color normColor;// colour for normal
    public bool isMoving; // isMoving bool to check if component is still moving

    [SerializeField]
    public LayerMask circuitLayer, nodeLayer; //layermask to filter boxcasts
    private Vector2 colliderSize; // size of the collider
    private BoxCollider2D collider2D; // the box collider attached to this object
    private Wire w; // wire component if this is a wire 
    private Node n; // used if wire
    private bool attachable; // used if hit is attachable
    private bool isBlocked; // used if blocked


    /// <summary>
    /// run on start moving , initializing values
    /// </summary>
    public void MoveStart()
    {
        isMoving = true;
        collider2D = this.GetComponent<BoxCollider2D>();
        colliderSize = collider2D.size;
        Cursor.visible = false;



    }


/// <summary>
/// update every frame, handle all movement functions of this class
/// </summary>
    private void Update()
    {
        if (isMoving) // if moving
        {
            
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            transform.position = new Vector2(Mathf.RoundToInt(curPosition.x), Mathf.RoundToInt(curPosition.y));// get mouse pos and round as circuit uses a grid base system
            
            //check if gameobject is blocked
            isBlocked = checkToBlock();
            //rotatation for circuit components
            if (Input.GetKeyDown(KeyCode.Q))
            {
                transform.Rotate(0, 0, 90, Space.Self);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                transform.Rotate(0, 0, -90, Space.Self);
            }


            //if placement is attempted
            else if (Input.GetMouseButtonDown(0))
            {
                if (isMoving) //if moving
                {

                    if (!isBlocked)// and not blocked
                    {
                        {
                            
                            isMoving = false; // stop moving and show cursor
                            Cursor.visible = true;
                            if (gameObject.GetComponent<Wire>())//if this is a wire, check if placed on node, if so add this wire to node and add node to this wire
                            {
                                n = CheckForNode();
                                if (n)
                                {
                                    gameObject.GetComponent<Wire>().addConnection(n);
                                    n.updateWire(gameObject.GetComponent<Wire>());
                                }
                                else if (w)
                                {
                                    gameObject.GetComponent<Wire>().addConnection(w);
                                    w.addConnection(gameObject.GetComponent<Wire>());
                                }
                                else
                                {
                                    //do nothing

                                }
                                // if component place 
                                gameObject.GetComponent<Wire>().GridMoveEnded();
                            }
                        }
                    }
                    else
                    {

                    //if blocked do nothing
                    }
                }
            }
            //if rightclick destroy this gameobject
            else if (Input.GetMouseButtonDown(1))
            {
                Cursor.visible = true;
                isMoving = false;
                
                Destroy(gameObject);
            }



        }



    }



    /// <summary>
    /// check if gameobject is blocked
    /// </summary>
    /// <returns>if blocked</returns>
    private bool checkToBlock()
    {
        Wire wireCheck;
        //get all colliders of hit by this gameobject collider
        List<Collider2D> collider2Ds = new List<Collider2D>(Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y), colliderSize / 2, 0, circuitLayer));
        collider2Ds.Remove(this.collider2D);
//        Debug.Log("hits " + gameObject.name + "    " + string.Join(" ", collider2Ds.ConvertAll(x => x.gameObject.name)));
       
        //if colliders hit
        if (collider2Ds.Count > 0)
        {
            //if this is a wire
            if (TryGetComponent<Wire>(out wireCheck))
            {
                //cast on node layer for any nodes, if a node is present the return false, circuit and nodes overlap by defualt
                if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), colliderSize, 0f, nodeLayer))
                {
                    wireCheck.toConnectedColor();
                    return false;
                }
                // if no node found, check for anywires hit
                else if (collider2Ds.FindAll(x => x.gameObject.tag != "Wire").Count == 0)
                {
                    wireCheck.toConnectedColor();
                    return false;
                }
                //else blocked
                else{
                    wireCheck.toErrorColor();
                    return true;
                }


            }
            else // not a wire
            {
                //if blocked show error return true
                if (TryGetComponent<CircuitComponent>(out CircuitComponent c)){
                    c.toErrorColor();
                } 

                return true;
            }
        }

        //else wire or component to normal color
        if (TryGetComponent<Wire>(out wireCheck)) wireCheck.toNormalColor();
        else if (TryGetComponent<CircuitComponent>(out CircuitComponent c)) c.toNormColor();
        return false;



    }


    /// <summary>
    /// checks for a node
    /// </summary>
    /// <returns>node if found</returns>
    private Node CheckForNode()
    {
        try
        {
            return Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), colliderSize, 0, nodeLayer).GetComponent<Node>();
        }
        catch
        {
            return null;
        }

    }



}




