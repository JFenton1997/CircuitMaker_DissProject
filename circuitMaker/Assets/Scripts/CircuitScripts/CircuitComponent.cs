using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



using Utilities;


public class CircuitComponent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{


    public DiagramComponent conponent;



    [HideInInspector]
    public string name;
    [HideInInspector]
    public Node nodeA;
    [HideInInspector]
    public Node nodeB;


    public Sprite spriteCell, spriteResistor, spriteLight;
    private Image conponentImage, highlight;
    private Color normalHighlightColor;
    public Color errorHighlightColor;
    private DisplayConponentValues UIdisplay;
    [HideInInspector] public CircuitClickAndDrag clickAndDrag;

    private bool prevDisplayValue;

    private GenerateCircuit foundGen;
    private ProblemViewer viewer;





    private void Awake()
    {
        prevDisplayValue = false;
        conponentImage = GetComponent<Image>();
        // conponent.type = ComponentType.CELL;
        clickAndDrag = GetComponent<CircuitClickAndDrag>();

        try
        {
            nodeA = transform.Find("nodeA").gameObject.GetComponent<Node>();
            nodeB = transform.Find("nodeB").gameObject.GetComponent<Node>();
            UIdisplay = transform.Find("Values").GetComponent<DisplayConponentValues>();
            highlight = transform.Find("Highlight").GetComponent<Image>();
            highlight.enabled = false;
            normalHighlightColor = highlight.color;




        }
        catch
        {
            Debug.LogError(this.name + " failed to find nodes");
        }

        if (transform.parent.TryGetComponent<GenerateCircuit>(out GenerateCircuit gen))
        {
            foundGen = gen;
            viewer = transform.Find("/UI/ProblemView").GetComponent<ProblemViewer>();
        }

    }

    private void Start()
    {

        DiagramComponent component = new DiagramComponent();
        //Assigning Text Variables




    }




    private void Update()
    {


        switch (conponent.type)
        {
            case ComponentType.CELL:
                conponentImage.sprite = spriteCell;
                break;
            case ComponentType.RESISTOR:
                conponentImage.sprite = spriteResistor;
                break;
            case ComponentType.LIGHT:
                conponentImage.sprite = spriteLight;
                break;
            default:
                conponentImage.sprite = null;
                //Debug.LogError("UNKOWN CONPONENT TYPE");
                break;




        }

        if (GlobalValues.circuitDisplayAll == true)
        {
            if (!foundGen)
            {
                if (conponent.type != ComponentType.UNTYPED)
                    UIdisplay.display();
                prevDisplayValue = true;
            }
        }
        if (GlobalValues.circuitDisplayAll == false && prevDisplayValue == true)
        {
            if (!foundGen)
            {
                if (conponent.type != ComponentType.UNTYPED)
                    UIdisplay.hide();
                prevDisplayValue = false;
            }

        }

        if (foundGen)
        {
            if(conponent.type != ComponentType.UNTYPED)
            if (viewer.displayValues)
            {
                UIdisplay.display();

            }
            else
                UIdisplay.hide();
        }

        conponent.name = this.gameObject.name;
        if (conponent.type == ComponentType.CELL)
        {
            conponent.direction = Direction.B_to_A;
        }

    }

    public void removeWireConnections()
    {
        if (nodeA.ConnectedWire)
            Destroy(nodeA.ConnectedWire.gameObject);
        if (nodeB.ConnectedWire)
            Destroy(nodeB.ConnectedWire.gameObject);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (!foundGen)
        {
            transform.Find("/UI/ValuesPanelCircuit").GetComponent<CircuitValuesPanel>().newSelected(GetComponent<CircuitComponent>());
        }
    }



    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     Cursor.visible = true;

    // }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!foundGen)
        {
            toNormColor();
            if (conponent.type != ComponentType.UNTYPED)
                UIdisplay.display();
        }

    }

    public void ShowHighlight()
    {
        if (!foundGen)
        {
            if (conponent.type != ComponentType.UNTYPED)
                highlight.enabled = true;
        }
    }

    public void toErrorColor() { if (conponent.type != ComponentType.UNTYPED) highlight.enabled = true; highlight.color = errorHighlightColor; }
    public void toNormColor() { if (conponent.type != ComponentType.UNTYPED) highlight.enabled = false; highlight.color = normalHighlightColor; }



    public void hideHighlight()
    {

        highlight.enabled = false;
    }

    private void OnDestroy()
    {
        transform.parent.GetComponent<CircuitManager>().allConponents.Remove(this);
    }





    public void OnPointerExit(PointerEventData eventData)
    {
        if (!foundGen)
        {
            if (conponent.type != ComponentType.UNTYPED)
                UIdisplay.hide();
        }
    }




}


