using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]

public class CircuitClickAndDrag : MonoBehaviour
{




    public Color dragColor = Color.gray;
    public Color blockedColor = Color.red;
    private Color normColor;
    public bool isMoving;

    [SerializeField]
    public LayerMask circuitLayer, nodeLayer;
    private Vector2 colliderSize;
    private BoxCollider2D collider2D;
    private Wire w;
    private Node n;
    private bool attachable;
    private bool isBlocked;
    public void MoveStart()
    {
        isMoving = true;
        collider2D = this.GetComponent<BoxCollider2D>();
        colliderSize = collider2D.size;
        Cursor.visible = false;



    }

    private void Update()
    {
        if (isMoving)
        {
            
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            transform.position = new Vector2(Mathf.RoundToInt(curPosition.x), Mathf.RoundToInt(curPosition.y));
            

            isBlocked = checkToBlock();
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
                if (isMoving)
                {

                    if (!isBlocked)
                    {
                        {
                            
                            isMoving = false;
                            Cursor.visible = true;
                            if (gameObject.GetComponent<Wire>())
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

                                }
                                gameObject.GetComponent<Wire>().GridMoveEnded();
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("blocked");
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Cursor.visible = true;
                isMoving = false;
                
                Destroy(gameObject);
            }



        }



    }



    private bool checkToBlock()
    {
        Wire wireCheck;
        List<Collider2D> collider2Ds = new List<Collider2D>(Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y), colliderSize / 2, 0, circuitLayer));
        collider2Ds.Remove(this.collider2D);
//        Debug.Log("hits " + gameObject.name + "    " + string.Join(" ", collider2Ds.ConvertAll(x => x.gameObject.name)));
        if (collider2Ds.Count > 0)
        {

            if (TryGetComponent<Wire>(out wireCheck))
            {
                if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), colliderSize, 0f, nodeLayer))
                {
                    wireCheck.toConnectedColor();
                    return false;
                }
                else if (collider2Ds.FindAll(x => x.gameObject.tag != "Wire").Count == 0)
                {
                    wireCheck.toConnectedColor();
                    return false;
                }
                else{
                    wireCheck.toErrorColor();
                    return true;
                }


            }
            else
            {
                if (TryGetComponent<CircuitComponent>(out CircuitComponent c)) c.toErrorColor();

                return true;
            }
        }


        if (TryGetComponent<Wire>(out wireCheck)) wireCheck.toNormalColor();
        else if (TryGetComponent<CircuitComponent>(out CircuitComponent c)) c.toNormColor();
        return false;



    }


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


    private void wireFuctionality(Collider2D hitInfo)
    {
        if (hitInfo.gameObject != gameObject)
        {
            if (hitInfo.gameObject.tag == "Wire")
            {
                w = hitInfo.gameObject.GetComponent<Wire>();
                attachable = true;

            }
            else if (hitInfo.gameObject.tag == "Node")
            {
                if (hitInfo.gameObject.GetComponent<Node>().ConnectedWire == null)
                {
                    n = hitInfo.gameObject.GetComponent<Node>();
                    attachable = true;
                }
                else
                {
                    isBlocked = true;
                    this.GetComponent<SpriteRenderer>().color = blockedColor;
                    attachable = false;
                }

            }
            else if (hitInfo.gameObject.tag == "CircuitComponent")
            {
                isBlocked = true;
                this.GetComponent<SpriteRenderer>().color = blockedColor;
            }
            else
            {
                isBlocked = false;
                this.GetComponent<SpriteRenderer>().color = dragColor;
            }
        }
        if (attachable == true)
        {
            isBlocked = false;
            this.GetComponent<SpriteRenderer>().color = dragColor;
        }
    }




}




