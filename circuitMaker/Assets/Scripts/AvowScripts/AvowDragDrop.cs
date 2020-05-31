using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

/// <summary>
/// Class operating the click and drag and snapping of avow components
/// </summary>
public class AvowDragDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// structure used to store other avow locations in the diagram
    /// diffX is distance in X cord from avow object this is attached to
    /// diffY same by fro y cord
    /// </summary>
    private struct LocationStruct
    {
        public RectTransform rectTransform;
        public float diffX;
        public float diffY;


        /// <summary>
        /// constructure
        /// </summary>
        /// <param name="rect"> rect transform of other avow</param>
        /// <param name="diffX">distance in x</param>
        /// <param name="diffY">distance in y</param>
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
    AvowComponent YAvow = null;
    AvowComponent XAvow = null;
    AvowManager avowManager;

    BoxCollider2D boxCollider2D;

/// <summary>
/// getting refrences
/// </summary>
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        selfRectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        avowManager = transform.GetComponentInParent<AvowManager>();


    }

/// <summary>
/// method apart of UnityEngine.EventsSystems
/// runs on a frame the mouse if held down on the collider and moves
/// initializes data needed for drag method
/// </summary>
/// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            // Debug.Log("OnBeginDrag");
            transform.SetAsLastSibling(); // puts avow as last to make sure the objects on top and not under other objects
            boxCollider2D.enabled = false; // prevent current object being seen as connected to any components
            locationStructs = getAvowObjects(); //get list of all avows in scene.
            canvasGroup.blocksRaycasts = false; // prevent object interactions

            avowManager.updateConnections(); // update connections to remove this avow


            canvasGroup.alpha = 0.6f; // fade to show movement to user
        }
    }


/// <summary>
/// called every frame the object being dragged
/// deals with snapping and movement
/// </summary>
/// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // run while mouse is down (left click) prevent drag on other clicks 
        if (Input.GetMouseButton(0))
        {
            snapOffset = GlobalValues.AvowSnappingOffset;
            Cursor.visible = false;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint); // get position of the cursor in 3d screen view
            Pair<LocationStruct, float> xToNearest = checkX(curPosition); // get nearest X component
            Pair<LocationStruct, float> yToNearest = checkY(curPosition);// get nearest Y component








            if (xToNearest.a.rectTransform != null)
            {
                XAvow = xToNearest.a.rectTransform.GetComponent<AvowComponent>();
            }
            if (yToNearest.a.rectTransform != null)
            {
                YAvow = yToNearest.a.rectTransform.GetComponent<AvowComponent>();
            }



            //rule #1 if both nearest y and x in offest, snap to them
            if (Mathf.Abs(yToNearest.b) < snapOffset && Mathf.Abs(xToNearest.b) < snapOffset &&
            ExtraUtilities.isEqualWithTolarance(xToNearest.a.rectTransform.position.y, selfRectTransform.position.y,
            xToNearest.a.diffY) && ExtraUtilities.isEqualWithTolarance(yToNearest.a.rectTransform.position.x, selfRectTransform.position.x,
            xToNearest.a.diffX))
            {
                //new position is snap position
                transform.position = new Vector2(curPosition.x + xToNearest.b, curPosition.y + yToNearest.b);

            }
            //rule #2 if nearest is Y but not X
            else if (Mathf.Abs(yToNearest.b) < snapOffset &&
            ExtraUtilities.isEqualWithTolarance(yToNearest.a.rectTransform.position.x, selfRectTransform.position.x,
            yToNearest.a.diffX))
            {
                // if snapping is enabled
                if (GlobalValues.AvowSnapping)
                {

                    // new position is the next free slot in nearest
                    if (yToNearest.b > 0)
                    {

                        
                        transform.position = new Vector2(
                            YAvow.nextFreeSlotInSpaceInDirection('D') + selfRectTransform.sizeDelta.x / 2, curPosition.y + yToNearest.b);
                        direction = 'D';
                    }

                    else
                    {
                        // if not, the position of near avow with snapping into next free slot   
                        transform.position = new Vector2(

                            YAvow.nextFreeSlotInSpaceInDirection('U') + selfRectTransform.sizeDelta.x / 2, curPosition.y + yToNearest.b);
                        direction = 'U';
                    }
                }
                else // if not snapping, go to y cord of nearest but dont snap
                {
                    transform.position = new Vector2(curPosition.x, curPosition.y + yToNearest.b);
                }


            }
            // rule #3 same as Y but for X
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



    /// <summary>
    /// run on the frame the dragging ends
    /// re-enable box collider, generate new connections, enable raycasts
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        boxCollider2D.enabled = true;
        avowManager.updateConnections();

        Cursor.visible = true;
        // Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

    }


/// <summary>
/// was used for debugging, kept to retain events
/// </summary>
/// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }


/// <summary>
/// called on the frame the mouse pointer is up, make cursor revisable
/// </summary>
/// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.visible = true;

    }


/// <summary>
/// gets all other avow objects apart this and calculates location struct for each
/// </summary>
/// <returns>completely filled list containing all avow objects and they diffx and diffy </returns>
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

/// <summary>
/// checks all avows in location struct and finds the avow nearest to cursor position on X cord
/// </summary>
/// <param name="curPosition"> vector 2 of current cursor cords to the screen</param>
/// <returns> the location struct of the nearest on the x cord as well as distance from edge to current avows edge</returns>
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


/// <summary>
/// checks all avows in location struct and finds the avow nearest to cursor position on Y cord
/// </summary>
/// <param name="curPosition"> vector 2 of current cursor cords to the screen</param>
/// <returns> the location struct of the nearest on the Y cord as well as distance from edge to current avows edge</returns>
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

/// <summary>
/// runs on the frame the mouse enters the avow, change avow to hover color and show connections
/// </summary>
/// <param name="eventData">unity event data</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<AvowComponent>().ColorToHover();
        
    }


/// <summary>
/// runs on the frame the points leaves the objects collider, change all avows back to normal colour
/// </summary>
/// <param name="eventData">unity event data</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<AvowComponent>().ColorToMain();
         foreach(AvowComponent avow in this.GetComponent<AvowComponent>().TopConnections){
            avow.ColorToMain();
        }
        foreach(AvowComponent avow in this.GetComponent<AvowComponent>().BotConnections){
            avow.ColorToMain();
        }
        foreach(AvowComponent avow in this.GetComponent<AvowComponent>().RightConnections){
            avow.ColorToMain();
        }
        foreach(AvowComponent avow in this.GetComponent<AvowComponent>().LeftConnections){
            avow.ColorToMain();
        }
    }


}


