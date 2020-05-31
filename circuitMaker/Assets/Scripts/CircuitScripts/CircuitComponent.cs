using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



using Utilities;
using System;

/// <summary>
/// handles circuit component, stores DiagramComponent and UI feedback
/// </summary>
public class CircuitComponent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{


    public DiagramComponent component;



    [HideInInspector]
    public string name;
    [HideInInspector]
    public Node nodeA;
    [HideInInspector]
    public Node nodeB;

    //UI stuff
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




    /// <summary>
    /// initialize UI components and checks if a builder
    /// </summary>
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

        if (transform.parent.TryGetComponent<GenerateCircuit>(out GenerateCircuit gen))//if component is apart from a generator object
        {
            foundGen = gen;
            viewer = transform.Find("/UI/ProblemDisplayer/ProblemView").GetComponent<ProblemViewer>();
        }

    }

/// <summary>
/// diagram component constructor
/// </summary>
    private void Start()
    {

        DiagramComponent component = new DiagramComponent();
        //Assigning Text Variables




    }



/// <summary>
/// use type to set the sprite the component uses
/// </summary>
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
        
        // display circuit display if no gen and is set to show in the global values
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
        //if appart from gen show based on problem finders values
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

    /// <summary>
    /// removes all connections from nodes, called on destroy
    /// </summary>
    public void removeWireConnections()
    {
        if (nodeA.ConnectedWire)
            Destroy(nodeA.ConnectedWire.gameObject);
        if (nodeB.ConnectedWire)
            Destroy(nodeB.ConnectedWire.gameObject);
    }


/// <summary>
/// if pointer on gameobject run selected on Circuit UI panel
/// </summary>
/// <param name="eventData"> Unity event data</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!foundGen)
        {
            transform.Find("/UI/ValuesPanelCircuit").GetComponent<CircuitValuesPanel>().newSelected(GetComponent<CircuitComponent>());
        }
    }




    /// <summary>
    /// if point enters circuit component, and not gen, show UI display
    /// </summary>
    /// <param name="eventData"> unity event data</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!foundGen)
        {
            //toNormColor();
            if (component.type != ComponentType.UNTYPED)
                UIdisplay.display();
        }

    }

/// <summary>
/// show highlight to colour component, used to show if selected or error
/// </summary>
    public void ShowHighlight()
    {
        if (!foundGen)
        {
            if (component.type != ComponentType.UNTYPED)
                highlight.enabled = true;
        }
    }

/// <summary>
/// set highlight to show and red error color
/// </summary>
    public void toErrorColor() { if (component.type != ComponentType.UNTYPED) highlight.enabled = true; highlight.color = errorHighlightColor; }
    /// <summary>
    /// set highlight off and to normal colour of selected
    /// </summary>
    /// <param name="!"></param>
    public void toNormColor() { if (component.type != ComponentType.UNTYPED) highlight.enabled = false; highlight.color = normalHighlightColor; }


/// <summary>
/// hide highlight
/// </summary>
    public void hideHighlight()
    {
        highlight.enabled = false;
    }

/// <summary>
/// run on destroy, remove this component from allcomponents to prevent null pointers
/// </summary>
    private void OnDestroy()
    {
        if(! foundGen)
                transform.parent.GetComponent<CircuitManager>().allcomponents.Remove(this);
    }




/// <summary>
/// on pointer exit, stop showing circuit details if not set to in global values
/// </summary>
/// <param name="eventData">unity event data</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!foundGen)
        {
            if (component.type != ComponentType.UNTYPED)
                UIdisplay.hide();
        }
    }


}


