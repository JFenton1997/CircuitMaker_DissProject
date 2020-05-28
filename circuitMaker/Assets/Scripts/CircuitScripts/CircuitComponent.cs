using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



using Utilities;
using System;

public class CircuitComponent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{


    public DiagramComponent component;



    [HideInInspector]
    public string name;
    [HideInInspector]
    public Node nodeA;
    [HideInInspector]
    public Node nodeB;


    public Sprite spriteCell, spriteResistor, spriteLight;
    private Image componentImage, highlight;
    private Color normalHighlightColor;
    public Color errorHighlightColor;
    private DisplayComponentValues UIdisplay;
    [HideInInspector] public CircuitClickAndDrag clickAndDrag;

    private bool prevDisplayValue;

    public GenerateCircuit foundGen;
    private ProblemViewer viewer;
    public bool isBuilder;





    private void Awake()
    {
        isBuilder = transform.parent.GetComponent<CircuitManager>().isBuilder;
        prevDisplayValue = false;
        componentImage = GetComponent<Image>();
        // component.type = ComponentType.CELL;
        clickAndDrag = GetComponent<CircuitClickAndDrag>();

        try
        {
            nodeA = transform.Find("nodeA").gameObject.GetComponent<Node>();
            nodeB = transform.Find("nodeB").gameObject.GetComponent<Node>();
            UIdisplay = transform.Find("Values").GetComponent<DisplayComponentValues>();
            highlight = transform.Find("Highlight").GetComponent<Image>();
            highlight.enabled = false;
            normalHighlightColor = highlight.color;




        }
        catch
        {
        }

        if (transform.parent.TryGetComponent<GenerateCircuit>(out GenerateCircuit gen))
        {
            foundGen = gen;
            viewer = transform.Find("/UI/ProblemDisplayer/ProblemView").GetComponent<ProblemViewer>();
        }

    }

    private void Start()
    {

        DiagramComponent component = new DiagramComponent();
        //Assigning Text Variables




    }




    private void Update()
    {


        switch (component.type)
        {
            case ComponentType.CELL:
                componentImage.sprite = spriteCell;
                break;
            case ComponentType.RESISTOR:
                componentImage.sprite = spriteResistor;
                break;
            case ComponentType.LIGHT:
                componentImage.sprite = spriteLight;
                break;
            default:
                componentImage.sprite = null;
                //Debug.LogError("UNKOWN component TYPE");
                break;




        }

        if (GlobalValues.circuitDisplayAll == true)
        {
            if (!foundGen)
            {
                if (component.type != ComponentType.UNTYPED)
                    UIdisplay.display();
                prevDisplayValue = true;
            }
        }
        if (GlobalValues.circuitDisplayAll == false && prevDisplayValue == true)
        {
            if (!foundGen)
            {
                if (component.type != ComponentType.UNTYPED)
                    UIdisplay.hide();
                prevDisplayValue = false;
            }

        }

        if (foundGen)
        {
            if(component.type != ComponentType.UNTYPED)
            if (viewer.displayValues)
            {
                UIdisplay.display();

            }
            else
                UIdisplay.hide();
        }

        component.name = this.gameObject.name;
        if (component.type == ComponentType.CELL)
        {
            component.direction = Direction.B_to_A;
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
            //toNormColor();
            if (component.type != ComponentType.UNTYPED)
                UIdisplay.display();
        }

    }

    public void ShowHighlight()
    {
        if (!foundGen)
        {
            if (component.type != ComponentType.UNTYPED)
                highlight.enabled = true;
        }
    }

    public void toErrorColor() { if (component.type != ComponentType.UNTYPED) highlight.enabled = true; highlight.color = errorHighlightColor; }
    public void toNormColor() { if (component.type != ComponentType.UNTYPED) highlight.enabled = false; highlight.color = normalHighlightColor; }



    public void hideHighlight()
    {
        highlight.enabled = false;
    }

    private void OnDestroy()
    {
        if(! foundGen)
                transform.parent.GetComponent<CircuitManager>().allcomponents.Remove(this);
    }





    public void OnPointerExit(PointerEventData eventData)
    {
        if (!foundGen)
        {
            if (component.type != ComponentType.UNTYPED)
                UIdisplay.hide();
        }
    }


}


