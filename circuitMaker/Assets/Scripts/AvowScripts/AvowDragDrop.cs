using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

public class AvowDragDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    private struct LocationStruct
    {
        public RectTransform rectTransform;
        public float diffX;
        public float diffY;



        public LocationStruct(RectTransform rect, float diffX, float diffY)
        {
            rectTransform = rect;
            this.diffX = diffX;
            this.diffY = diffY;
        }
    }

    [SerializeField] private Canvas canvas;

    List<RectTransform> ListOfComponents;
    private char direction;
    public RectTransform selfRectTransform;
    public float snapOffset = GlobalValues.AvowSnappingOffset;
    private CanvasGroup self, canvasGroup;
    private List<LocationStruct> locationStructs;
    AvowConponent YAvow = null;
    AvowConponent XAvow = null;
    AvowManager avowManager;

    BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        selfRectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        avowManager = transform.GetComponentInParent<AvowManager>();


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            // Debug.Log("OnBeginDrag");
            transform.SetAsLastSibling();
            boxCollider2D.enabled = false;
            locationStructs = getAvowObjects();
            canvasGroup.blocksRaycasts = false;

            avowManager.updateConnections();
            // Debug.Log("dragFound: " + string.Join(" ", locationStructs.ConvertAll(i => "\n" + i.rectTransform.gameObject.name + " " + i.diffX.ToString()
            //  + " " + i.diffY.ToString()).ToArray()));


            canvasGroup.alpha = 0.6f;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            snapOffset = GlobalValues.AvowSnappingOffset;
            Cursor.visible = false;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            Pair<LocationStruct, float> xToNearest = checkX(curPosition);
            Pair<LocationStruct, float> yToNearest = checkY(curPosition);








            if (xToNearest.a.rectTransform != null)
            {
                XAvow = xToNearest.a.rectTransform.GetComponent<AvowConponent>();
            }
            if (yToNearest.a.rectTransform != null)
            {
                YAvow = yToNearest.a.rectTransform.GetComponent<AvowConponent>();
            }



            //rule #1
            if (Mathf.Abs(yToNearest.b) < snapOffset && Mathf.Abs(xToNearest.b) < snapOffset &&
            ExtraUtilities.isEqualWithTolarance(xToNearest.a.rectTransform.position.y, selfRectTransform.position.y,
            xToNearest.a.diffY) && ExtraUtilities.isEqualWithTolarance(yToNearest.a.rectTransform.position.x, selfRectTransform.position.x,
            xToNearest.a.diffX))
            {
        
                transform.position = new Vector2(curPosition.x + xToNearest.b, curPosition.y + yToNearest.b);

            }
            //rule #2
            else if (Mathf.Abs(yToNearest.b) < snapOffset &&
            ExtraUtilities.isEqualWithTolarance(yToNearest.a.rectTransform.position.x, selfRectTransform.position.x,
            yToNearest.a.diffX))
            {
                if (GlobalValues.AvowSnapping)
                {

                    //Debug.Log("R3 X: " + xToNearest.a.rectTransform.name);
                    if (yToNearest.b > 0)
                    {
                        Debug.Log("bugD");
                        transform.position = new Vector2(
                            YAvow.nextFreeSlotInSpaceInDirection('D') + selfRectTransform.sizeDelta.x / 2, curPosition.y + yToNearest.b);
                        direction = 'D';
                    }

                    else
                    {
                        Debug.Log("bugU");
                        transform.position = new Vector2(

                            YAvow.nextFreeSlotInSpaceInDirection('U') + selfRectTransform.sizeDelta.x / 2, curPosition.y + yToNearest.b);
                        direction = 'U';
                    }
                }
                else
                {
                    transform.position = new Vector2(curPosition.x, curPosition.y + yToNearest.b);
                }


            }
            // rule #3
            else if (Mathf.Abs(xToNearest.b) < snapOffset &&
            ExtraUtilities.isEqualWithTolarance(xToNearest.a.rectTransform.position.y, selfRectTransform.position.y,
            xToNearest.a.diffY))
            {
                if (GlobalValues.AvowSnapping)
                {

                    //Debug.Log("R3 X: " + xToNearest.a.rectTransform.name);
                    if (xToNearest.b > 0)
                    {
                        transform.position = new Vector2(curPosition.x + xToNearest.b,
                            XAvow.nextFreeSlotInSpaceInDirection('L') - selfRectTransform.sizeDelta.y / 2);
                        direction = 'L';
                    }

                    else
                    {
                        transform.position = new Vector2(curPosition.x + xToNearest.b,
                            XAvow.nextFreeSlotInSpaceInDirection('R') - selfRectTransform.sizeDelta.y / 2);
                        direction = 'R';
                    }

                }
                else
                {
                    {
                        transform.position = new Vector2(curPosition.x + xToNearest.b, curPosition.y);
                    }
                }





            }
            else
            {
                //Debug.Log("R4 ");
                transform.position = new Vector2(curPosition.x, curPosition.y);

            }
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        boxCollider2D.enabled = true;
        avowManager.updateConnections();

        Cursor.visible = true;
        // Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

    }



    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.visible = true;

    }

    private List<LocationStruct> getAvowObjects()
    {
        ListOfComponents = new List<RectTransform>();
        Transform parent = transform.parent;
        parent.GetComponentsInChildren<RectTransform>(false, ListOfComponents);
        // Debug.Log(string.Join(" ", ListOfComponents.ConvertAll(i => i.name)));
        ListOfComponents.Remove(selfRectTransform);
        List<LocationStruct> locationStructs = new List<LocationStruct>();
        foreach (RectTransform rt in ListOfComponents)
        {
            if (rt.transform.parent != parent)
            {
                continue;
            }
            float diffX = (rt.sizeDelta.x / 2 + this.selfRectTransform.sizeDelta.x / 2);
            float diffY = (rt.sizeDelta.y / 2 + this.selfRectTransform.sizeDelta.y / 2);
            locationStructs.Add(new LocationStruct(rt, diffX, diffY));
        }
        return locationStructs;

    }

    private Pair<LocationStruct, float> checkX(Vector2 curPosition)
    {
        float lowest = Mathf.Infinity;
        bool cursIsRight = true;
        LocationStruct locationStruct = new LocationStruct();
        foreach (LocationStruct ls in locationStructs)
        {
            if (ls.rectTransform.position.x < curPosition.x)
            {
                float cursToEdge = curPosition.x - (ls.rectTransform.position.x + ls.diffX);
                if (cursToEdge < lowest && cursToEdge > 0)
                {
                    lowest = cursToEdge;
                    locationStruct = ls;
                    cursIsRight = true;
                }
            }
            else if (ls.rectTransform.position.x > curPosition.x)
            {
                float cursToEdge = (ls.rectTransform.position.x - ls.diffX) - curPosition.x;
                if (cursToEdge < lowest && cursToEdge > 0)
                {
                    lowest = cursToEdge;
                    locationStruct = ls;
                    cursIsRight = false;
                }
            }

        }
        if (lowest <= snapOffset)
        {
            if (cursIsRight)
            {
                return new Pair<LocationStruct, float>(locationStruct, -lowest);
            }
            else
            {
                return new Pair<LocationStruct, float>(locationStruct, lowest);
            }
        }
        else
        {
            return new Pair<LocationStruct, float>(locationStruct, Mathf.Infinity);
        }
    }



    private Pair<LocationStruct, float> checkY(Vector2 curPosition)
    {
        float lowest = Mathf.Infinity;
        bool cursIsRight = true;
        LocationStruct locationStruct = new LocationStruct();
        foreach (LocationStruct ls in locationStructs)
        {
            if (ls.rectTransform.position.y < curPosition.y)
            {
                float cursToEdge = curPosition.y - (ls.rectTransform.position.y + ls.diffY);
                if (cursToEdge < lowest && cursToEdge > 0)
                {
                    lowest = cursToEdge;
                    locationStruct = ls;
                    cursIsRight = true;
                }
            }
            else if (ls.rectTransform.position.y > curPosition.y)
            {
                float cursToEdge = (ls.rectTransform.position.y - ls.diffY) - curPosition.y;
                if (cursToEdge < lowest && cursToEdge > 0)
                {
                    lowest = cursToEdge;
                    locationStruct = ls;
                    cursIsRight = false;
                }
            }

        }
        if (lowest <= snapOffset)
        {
            if (cursIsRight)
            {
                return new Pair<LocationStruct, float>(locationStruct, -lowest);
            }
            else
            {
                return new Pair<LocationStruct, float>(locationStruct, lowest);
            }
        }
        else
        {
            return new Pair<LocationStruct, float>(locationStruct, Mathf.Infinity);
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<AvowConponent>().ColorToHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<AvowConponent>().ColorToMain();
    }


}


