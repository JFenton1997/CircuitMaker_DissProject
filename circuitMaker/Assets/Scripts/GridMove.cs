using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class GridMove : MonoBehaviour
{




    public Color dragColor = Color.gray;
    public Color blockedColor = Color.red;
    private Color normColor;
    public bool isMoving;
    private bool blocked;
    [SerializeField]
    private LayerMask circuitLayer;
    private Vector2 colliderSize;
    private Wire w;
    private Node n;

    public void MoveStart()
    {
        normColor = this.GetComponent<SpriteRenderer>().color;
        isMoving = true;
        colliderSize = this.GetComponent<BoxCollider2D>().size * 0.5f;



    }

    private void Update()
    {
        if (isMoving)
        {
            Cursor.visible = false;
            this.GetComponent<SpriteRenderer>().color = dragColor;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            transform.position = new Vector2(Mathf.RoundToInt(curPosition.x), Mathf.RoundToInt(curPosition.y));

            n = null;
            w = null;
            //check for collision
            Vector2 startingPos = new Vector2(transform.position.x, transform.position.y);
            Collider2D[] rayHit = Physics2D.OverlapBoxAll(startingPos, colliderSize, 0f, circuitLayer);
            bool attachable = false;
            foreach (Collider2D hitInfo in rayHit)
            {
                if (hitInfo)
                {

                    if (gameObject.GetComponent<Wire>())
                    {
                        if (hitInfo.gameObject != gameObject)
                        {
                            Debug.Log(hitInfo.name);
                            if (hitInfo.gameObject.tag == "Wire")
                            {
                                Debug.LogError("wire");
                                w = hitInfo.gameObject.GetComponent<Wire>();
                                attachable = true;
                                
                            }
                            else if (hitInfo.gameObject.tag == "Node")
                            {
                                Debug.Log(hitInfo.gameObject.GetComponent<Node>().ConnectedWire);
                                if(hitInfo.gameObject.GetComponent<Node>().ConnectedWire == null){
                                    Debug.LogError("node");
                                    n = hitInfo.gameObject.GetComponent<Node>();
                                    attachable = true;
                                }else{
                                        blocked = true;
                                        this.GetComponent<SpriteRenderer>().color = blockedColor;
                                        attachable = false;
                                }

                            }
                            else if (hitInfo.gameObject.tag == "CircuitComponent")
                            {
                                blocked = true;
                                this.GetComponent<SpriteRenderer>().color = blockedColor;
                            }
                            else
                            {
                                blocked = false;
                                this.GetComponent<SpriteRenderer>().color = dragColor;
                            }
                        }
                    }
                    else
                    {


                        if (checkToBlock(hitInfo))
                        {
                            blocked = true;
                            this.GetComponent<SpriteRenderer>().color = blockedColor;
                        }
                        else
                        {
                            blocked = false;
                            this.GetComponent<SpriteRenderer>().color = dragColor;
                        }
                    }
                }

            }
            if (attachable == true)
            {
                blocked = false;
                this.GetComponent<SpriteRenderer>().color = dragColor;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                transform.Rotate(0, 0, 90, Space.Self);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                transform.Rotate(0, 0, -90, Space.Self);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (isMoving && !blocked)
                {
                    Cursor.visible = true;
                    isMoving = false;
                    this.GetComponent<SpriteRenderer>().color = normColor;
                    transform.SendMessage("gridMoveEnded");
                    if (gameObject.GetComponent<Wire>())
                    {
                        if (n)
                        {
                            Debug.LogError("n added");
                            gameObject.GetComponent<Wire>().addConnection(n);
                            n.updateWire(gameObject.GetComponent<Wire>());
                        }
                        else if(w)
                        {
                            Debug.LogError("W added");
                            gameObject.GetComponent<Wire>().addConnection(w);
                            w.addConnection(gameObject.GetComponent<Wire>());
                        }
                        else{

                        }
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Cursor.visible = true;
                    Destroy(gameObject);
                }



            }



        }

    }

    private bool checkToBlock(Collider2D hit)
    {
        Transform target = hit.transform;
        if (transform)
        {
            Debug.Log(transform.name);
            foreach (Transform child in transform)
            {
                if (target.GetInstanceID() == child.GetInstanceID())
                {
                    return false;
                }
            }
        }
        if (target.gameObject.GetInstanceID() == transform.gameObject.GetInstanceID())
        {
            return false;
        }
        else
        {
            Debug.LogError(transform.name);
            return true;
        }

    }


}


