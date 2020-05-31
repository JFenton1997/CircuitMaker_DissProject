using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; //text mesh pro, used for small text, as regular dont render
using Utilities;
public class AvowComponent : MonoBehaviour
{
    //component to component infromation
    public DiagramComponent component; 
    // colours used for UI
    public Color fillColour, connectedColor = Color.green, hoverColor = Color.gray, selectedColor = Color.magenta, errorColor = Color.red, pastColor;
    public RectTransform AvowFillColorTrans;
    public BoxCollider2D boxCollider2D;
    private Image AvowFillColorImg; // the fill image inside each avow
    private AvowManager avowManager; // perant
    private List<AvowComponent> sameLayer; // used to add connections if component end at exact space (follow AVOW connection logic)
    private CanvasGroup infoPanel;
    private TMP_Text widthText , nameText, heightText , typeText; 

    public bool isBlocked; // if on top of another avow or vis versa
    public bool isBuilder; // if perant is part of the question builder scene

    [SerializeField]
    public List<AvowComponent> TopConnections, BotConnections, LeftConnections, RightConnections;
    private ProblemViewer problem;// used if part of problem
    private AvowGenerator foundGen; // used if part of problem
    private SolverScript solver; // used if part of problem
    bool checkIfAnswers;

    Image image;
    public RectTransform rectTransform;

    public float voltage = 1, current = 1; // to set size, and to keep component values and scale seperate 
    Vector2 fillSize;
    public Color hiddenColor;


    [HideInInspector]
    public string name; // name, old variable 





   /// <summary>
   /// check if part of a solver scene, if so assign solver object. assign UI components
   /// </summary>
    private void Awake()
    {try{
        // if part of solver scene (problem display found), get solver 
        if(transform.Find("/UI/ProblemDisplayer/ProblemView").TryGetComponent<ProblemViewer>(out problem)){
            Debug.Log("FOUNDPROBLEM");
            solver = transform.Find("/UI/SolverPanel").GetComponent<SolverScript>();

        }
        // if created from a genarator and not user
        if(transform.parent.TryGetComponent<AvowGenerator>(out foundGen)){
            Debug.Log("FOUNDGEN");

        }
        //if not found, ignore as designed to
    }catch{}
        

        // assigning variables and pointer
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        rectTransform = this.GetComponent<RectTransform>();
        image = this.GetComponent<Image>();
        AvowFillColorTrans = transform.GetChild(0).GetComponent<RectTransform>();
        AvowFillColorImg = AvowFillColorTrans.GetComponent<Image>();
        TopConnections = new List<AvowComponent>();
        BotConnections = new List<AvowComponent>();
        LeftConnections = new List<AvowComponent>();
        RightConnections = new List<AvowComponent>();
        avowManager = transform.GetComponentInParent<AvowManager>();
        isBuilder = avowManager.isBuilder;
        sameLayer = new List<AvowComponent>();
        pastColor = fillColour;
        infoPanel = transform.Find("AvowFillColor/Panel").GetComponent<CanvasGroup>();
        widthText = infoPanel.transform.Find("Width").GetComponent<TMP_Text>();
        nameText = infoPanel.transform.Find("Name").GetComponent<TMP_Text>();
        heightText = infoPanel.transform.Find("Height").GetComponent<TMP_Text>();
        typeText = infoPanel.transform.Find("Type").GetComponent<TMP_Text>();
        infoPanel.alpha = 0f;




    }



    /// <summary>
    ///  used to update size, check if blocked and control UI elements
    /// </summary>
    void Update()
    {
        updateFill();
        component.name = gameObject.name; // assign name of component to this objects name.
        this.GetComponent<BoxCollider2D>().size = fillSize; // keep size of avow updated
        Collider2D[] hit = new Collider2D[100]; // checking for block

        if (boxCollider2D.OverlapCollider(new ContactFilter2D(), hit) > 0)
        {
            pastColor = AvowFillColorImg.color;
            AvowFillColorImg.color = errorColor;
            isBlocked = true;
        }
        else
        {
            isBlocked = false;
            if (AvowFillColorImg.color == errorColor && isBlocked == true)
            {
                AvowFillColorImg.color = pastColor;

            }
            else
            {
                pastColor = AvowFillColorImg.color;

            }

        }
        //keeping component values text updated
        nameText.text = component.name;
        typeText.text =component.type.ToString();

        widthText.text = current.ToString()+" :W";
        heightText.text = voltage.ToString()+" :H";

        // get if show answers pressed
        if(foundGen) checkIfAnswers = solver.showAnswer;
        else checkIfAnswers = false;

        // if value is not hidden or not part of a generator, show width
        if(!component.Values[ComponentParameter.CURRENT].hidden || !foundGen) widthText.text =widthText.text = current.ToString()+" :W";
        else if (checkIfAnswers){ //if show answers been toggled 
            widthText.text = current.ToString()+" :W";

        }// else, thus, value is hidden and part of a generator
        else{ widthText.text = "? :W"; widthText.color = hiddenColor;};

        // same for height
        if(!component.Values[ComponentParameter.VOLTAGE].hidden || !foundGen) heightText.text = voltage + " :H";
        else if (checkIfAnswers){
            heightText.text = voltage + ":H";

        }
        else{ heightText.text = "? :H"; heightText.color = hiddenColor;};

        // if display values is enabled and not part of gen
        if(GlobalValues.circuitDisplayAll && !foundGen){
            infoPanel.alpha=1;
        }
        // if part of gen, toggle values showing on the problem finder panels value
        else if(foundGen){
            if(problem.displayValues){

                infoPanel.alpha=1;
            }
            else infoPanel.alpha = 0f;
        }
        else{
            infoPanel.alpha=0;
        }
    }





    

/// <summary>
///  update the size of the avow and border based on voltage and current values.
/// uses camera size to set border size
/// </summary>
    private void updateFill()
    {
        // updating the size of fill to keep the outline, uses camera to decide thickness of the avows borders.
        
        updateSize(voltage, current);
        if(isBuilder)
        fillSize = new Vector2(rectTransform.rect.width - (0.1f * (Camera.main.orthographicSize / 5)), rectTransform.rect.height
         - (0.05f * (Camera.main.orthographicSize / 5)));
         else{
             fillSize = new Vector2(rectTransform.rect.width - 0.1f, rectTransform.rect.height - 0.1f);
         }

        AvowFillColorTrans.sizeDelta = fillSize;



    }

    /// <summary>
    /// used to update size of avow
    /// </summary>
    /// <param name="voltage"> height of avow</param>
    /// <param name="current"> width of avow</param>
    public void updateSize(float voltage, float current)
    {
        if (current > 0 && voltage > 0)
        {
            rectTransform.sizeDelta = new Vector2((float)current, (float)voltage);
        }
    }

    /// <summary>
    /// called to highlight green
    /// </summary>
    public void ColorToConnected()
    {
        AvowFillColorImg.color = connectedColor;
    }

    /// <summary>
    /// called to reset colour 
    /// </summary>
    public void ColorToMain()
    {
        AvowFillColorImg.color = fillColour;

    }

    /// <summary>
    /// called to highlight connect green, change self to hover colour
    /// </summary>
    public void ColorToHover()
    {
        AvowFillColorImg.color = hoverColor;
        foreach (AvowComponent avow in TopConnections)
        {
            avow.ColorToConnected();
        }
        foreach (AvowComponent avow in BotConnections)
        {
            avow.ColorToConnected();
        }
        foreach (AvowComponent avow in RightConnections)
        {
            avow.ColorToConnected();
        }
        foreach (AvowComponent avow in LeftConnections)
        {
            avow.ColorToConnected();
        }


    }

    /// <summary>
    /// changes colour to selected if selected in values panel
    /// </summary>
    public void ColorToSelected()
    {
        AvowFillColorImg.color = selectedColor;

    }

    /// <summary>
    /// sets colour to a given parameter , used for debugging 
    /// </summary>
    /// <param name="color"> colour to be changed to</param>
    public void ColorToParam(Color color)
    {
        AvowFillColorImg.color = color;
    }


    /// <summary>
    /// returns the values of distance in direction given to edge of Avow to center
    /// </summary>
    /// <param name="direction"> the given direction</param>
    /// <returns> float position of edge of give direction</returns>
    public float nextSpaceInDirection(char direction)
    {
        switch (direction)
        {
            case 'U':
                return rectTransform.position.y + rectTransform.sizeDelta.y / 2;
            case 'D':
                return rectTransform.position.y - (rectTransform.sizeDelta.y / 2);
            case 'L':
                return rectTransform.position.x - rectTransform.sizeDelta.x / 2;
            case 'R':
                return rectTransform.position.x + (rectTransform.sizeDelta.x / 2);
            default:
                Debug.LogError(this.name + " nextSpace call with unknown direction: " + direction);
                return 0f;

        }
    }

    /// <summary>
    /// used for snapping and Avow gen gets the position of edge in a given direction, with the direction being with coliding the any current connections
    /// </summary>
    /// <param name="direction"> given direction as a char</param>
    /// <returns>float position of edge of give direction in next free slot</returns>
    public float nextFreeSlotInSpaceInDirection(char direction)
    {
        switch (direction)
        {
            case 'U':
                if (TopConnections.Count == 0)
                {
                    Debug.Log("Correct");
                    return rectTransform.position.x - rectTransform.sizeDelta.x / 2;
                }
                else
                {
                    return TopConnections[TopConnections.Count - 1].nextSpaceInDirection('R');
                }
            case 'D':
                if (BotConnections.Count == 0)
                {
                    return rectTransform.position.x - rectTransform.sizeDelta.x / 2;
                }
                else
                {
                    return BotConnections[BotConnections.Count - 1].nextSpaceInDirection('R');
                }
            case 'L':
                if (LeftConnections.Count == 0)
                {
                    return rectTransform.position.y + rectTransform.sizeDelta.y / 2;
                }
                else
                {
                    return LeftConnections[LeftConnections.Count - 1].nextSpaceInDirection('D');
                }
            case 'R':
                if (RightConnections.Count == 0)
                {
                    return rectTransform.position.y + rectTransform.sizeDelta.y / 2;
                }
                else
                {
                    return RightConnections[RightConnections.Count - 1].nextSpaceInDirection('D');
                }
            default:
                Debug.LogError(this.name + " nextSlot Space call with unknown direction: " + direction);
                return 0f;



        }
    }

    // reset connections
    public void clearConnections()
    {
        TopConnections.Clear();
        BotConnections.Clear();
        RightConnections.Clear();
        LeftConnections.Clear();
        sameLayer.Clear();
    }

    /// <summary>
    ///  used to share connections if the side avows share the same bottom value
    /// </summary>
    public void updateSameLayerConnections()
    {
        Vector3[] corners = new Vector3[4]; //stores corners of this avow
        Vector3[] rCorners = new Vector3[4]; // stores corners of a right connected avow
        Vector3[] lCorners = new Vector3[4]; // stores corners of a left connected avow
        rectTransform.GetWorldCorners(corners); //get corners of this avow
        if (LeftConnections.Count >= 1) // if a left connection exists
        {

            AvowComponent l = LeftConnections[LeftConnections.Count - 1]; 
            l.rectTransform.GetWorldCorners(lCorners);//corners of the last elements in left connections
            if (ExtraUtilities.isEqualWithTolarance(corners[0].y, lCorners[3].y, 0.01f)) // check if bottom left corner of this and bottom right of l is on same y level
            {
                foreach (AvowComponent b in l.BotConnections) //if yes, share bottom connections
                {
                    if (!this.BotConnections.Contains(b) && b != this)
                    {
                        this.BotConnections.Add(b);
                    }
                }

            }
        }
        if (RightConnections.Count >= 1)
        {
            AvowComponent r = RightConnections[RightConnections.Count - 1];
            r.rectTransform.GetWorldCorners(rCorners);//corners of the last elements in right connections
            Debug.Log(r.gameObject.name);
           Debug.Log(corners[3].y+ "   "+ rCorners[0].y);
            if (ExtraUtilities.isEqualWithTolarance(corners[3].y, rCorners[0].y, 0.01f)) // check if bottom right corner of this and bottom left of r is on same y level
            {
                foreach (AvowComponent b in r.BotConnections)//if yes, share bottom connections
                {
                    if (!this.BotConnections.Contains(b) && b != this)
                    {
                        this.BotConnections.Add(b);
                    }
                }

            }
        }
        // order bottom connections by the x position, needed to prevent overlaps in generation
        BotConnections.Sort((x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x));
    }




    /// <summary>
    /// used to update connections using approximate distance 
    /// </summary>
    public void updateConnections()
    {


        Vector2 sizeDelta = rectTransform.sizeDelta;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners); // get this avows corners


        //TopConnections, create a overlap box cast to get all avows the at above this avow ( near distance)
        Collider2D[] hitsTOP = Physics2D.OverlapAreaAll(new Vector2(corners[1].x, corners[1].y), new Vector2(corners[2].x, corners[2].y + 0.1f)); 
        Array.Sort(hitsTOP, (x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x)); // sort hits by x position left to right
        if (hitsTOP.Length > 0)
        {
            foreach (Collider2D hit in hitsTOP) // add to top connections in order
            {
                if (hit.TryGetComponent(out AvowComponent avowHit))
                {
                    if (avowHit != this) TopConnections.Add(avowHit);
                }

            }
        }
        //BotConnections, create a overlap box cast to get all avows the at below this avow ( near distance)
        Collider2D[] hitsBOT = Physics2D.OverlapAreaAll(new Vector2(corners[0].x, corners[0].y), new Vector2(corners[3].x, corners[3].y - 0.1f));
        Array.Sort(hitsBOT, (x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x)); // sort hits by x position
        if (hitsBOT.Length > 0)
        {
            foreach (Collider2D hit in hitsBOT) // add bot connections in order
            {
                if (hit.TryGetComponent(out AvowComponent avowHit))
                {
                    if (avowHit != this) BotConnections.Add(avowHit);
                }

            }
        }
        //LeftConnections, create a overlap box cast to get all avows the at left this avow ( near distance)
        Collider2D[] hitsL = Physics2D.OverlapAreaAll(new Vector2(corners[0].x, corners[0].y), new Vector2(corners[1].x - 0.1f, corners[1].y));
        if (hitsL.Length > 0)
        {
            Array.Sort(hitsL, (x1, x2) => x2.transform.position.y.CompareTo(x1.transform.position.y)); // sort hits by y position highest to lowest
            foreach (Collider2D hit in hitsL) // add left connections
            {
                if (hit.TryGetComponent(out AvowComponent avowHit)) 
                {
                    if (avowHit != this) LeftConnections.Add(avowHit); 
                }

            }
        }
        //rightConnections, create a overlap box cast to get all avows the at right this avow ( near distance)
        Collider2D[] hitsR = Physics2D.OverlapAreaAll(new Vector2(corners[2].x, corners[2].y), new Vector2(corners[3].x + 0.1f, corners[3].y));
        if (hitsR.Length > 0)
        {

            Array.Sort(hitsR, (x1, x2) => x2.transform.position.y.CompareTo(x1.transform.position.y)); // sort hits by y position highest to lowest
            foreach (Collider2D hit in hitsR) // add right connections
            {
                if (hit.TryGetComponent(out AvowComponent avowHit))
                {
                    if (avowHit != this) RightConnections.Add(avowHit);
                }

            }
        }


    }



    /// <summary>
    /// run on mouse down on object, run new selected on this avow
    /// </summary>
    private void OnMouseDown()
    {
        try
        {
            GameObject.Find("ValuesPanelAvow").GetComponent<AvowValuesPanel>().newSelected(this);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("MISSING OR CANT FIND VALUES PANEL\n" + ex.ToString());
        }

    }

/// <summary>
/// removes a given avow from connections 
/// </summary>
/// <param name="avowComponent"> avow to destroy</param>
    public void removeAvowConnection(AvowComponent avowComponent)
    {
        TopConnections.Remove(avowComponent);
        BotConnections.Remove(avowComponent);
        RightConnections.Remove(avowComponent);
        LeftConnections.Remove(avowComponent);
    }

/// <summary>
/// on destroy, call this method, get avow manager to remove this avow from all other avows to prevent null pointers
/// </summary>
    private void OnDestroy()
    {
        this.GetComponent<AvowDragDrop>().enabled = false;
        try
        {
            GetComponentInParent<AvowManager>().removeAvow(this);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(ex.Message);

        }
    }


}
