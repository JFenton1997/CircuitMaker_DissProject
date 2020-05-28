using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
public class AvowComponent : MonoBehaviour
{
    public DiagramComponent component;
    public Color fillColour, connectedColor = Color.green, hoverColor = Color.gray, selectedColor = Color.magenta, errorColor = Color.red, pastColor;
    public RectTransform AvowFillColorTrans;
    public BoxCollider2D boxCollider2D;
    private Image AvowFillColorImg;
    private AvowManager avowManager;
    private List<AvowComponent> sameLayer;
    private CanvasGroup infoPanel;
    private TMP_Text widthText , nameText, heightText , typeText; 

    public bool isBlocked;
    public bool isBuilder;

    [SerializeField]
    public List<AvowComponent> TopConnections, BotConnections, LeftConnections, RightConnections;
    private ProblemViewer problem;
    private AvowGenerator foundGen;
    private SolverScript solver;
    bool checkIfAnswers;

    Image image;
    public RectTransform rectTransform;

    public float voltage = 1, current = 1;
    Vector2 fillSize;
    public Color hiddenColor;


    [HideInInspector]
    public string name;





    // Start is called before the first frame update
    private void Awake()
    {try{
        if(transform.Find("/UI/ProblemDisplayer/ProblemView").TryGetComponent<ProblemViewer>(out problem)){
            Debug.Log("FOUNDPROBLEM");
            solver = transform.Find("/UI/SolverPanel").GetComponent<SolverScript>();

        }
        if(transform.parent.TryGetComponent<AvowGenerator>(out foundGen)){
            Debug.Log("FOUNDGEN");

        }
    }catch{}
        


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



    // Update is called once per frame
    void Update()
    {
        updateFill();
        component.name = gameObject.name;
        this.GetComponent<BoxCollider2D>().size = fillSize;
        Collider2D[] hit = new Collider2D[10];

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
        nameText.text = component.name;
        typeText.text =component.type.ToString();

        widthText.text = current.ToString()+" :W";
        heightText.text = voltage.ToString()+" :H";

        if(foundGen) checkIfAnswers = solver.showAnswer;
        else checkIfAnswers = false;

        if(!component.Values[ComponentParameter.CURRENT].hidden || !foundGen) widthText.text =widthText.text = current.ToString()+" :W";
        else if (checkIfAnswers){
            widthText.text = current.ToString()+" :W";

        }
        else{ widthText.text = "? :W"; widthText.color = hiddenColor;};

        if(!component.Values[ComponentParameter.VOLTAGE].hidden || !foundGen) heightText.text = voltage + " :H";
        else if (checkIfAnswers){
            heightText.text = voltage + ":H";

        }
        else{ heightText.text = "? :H"; heightText.color = hiddenColor;};














        if(GlobalValues.circuitDisplayAll && !foundGen){
            infoPanel.alpha=1;
        }
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





    


    private void updateFill()
    {
        //temp
        
        updateSize(voltage, current);
        if(isBuilder)
        fillSize = new Vector2(rectTransform.rect.width - (0.1f * (Camera.main.orthographicSize / 5)), rectTransform.rect.height
         - (0.05f * (Camera.main.orthographicSize / 5)));
         else{
             fillSize = new Vector2(rectTransform.rect.width - 0.1f, rectTransform.rect.height - 0.1f);
         }

        AvowFillColorTrans.sizeDelta = fillSize;



    }

    public void updateSize(float voltage, float current)
    {
        if (current > 0 && voltage > 0)
        {
            rectTransform.sizeDelta = new Vector2((float)current, (float)voltage);
        }
    }

    public void ColorToConnected()
    {
        AvowFillColorImg.color = connectedColor;
    }

    public void ColorToMain()
    {
        AvowFillColorImg.color = fillColour;

    }

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

    public void ColorToSelected()
    {
        AvowFillColorImg.color = selectedColor;

    }

    public void ColorToParam(Color color)
    {
        AvowFillColorImg.color = color;
    }



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

    public void clearConnections()
    {
        TopConnections.Clear();
        BotConnections.Clear();
        RightConnections.Clear();
        LeftConnections.Clear();
        sameLayer.Clear();
    }

    public void updateSameLayerConnections()
    {
        Debug.Log("CHECK " +gameObject.name);
        Vector3[] corners = new Vector3[4];
        Vector3[] rCorners = new Vector3[4];
        Vector3[] lCorners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        if (LeftConnections.Count >= 1)
        {

            AvowComponent l = LeftConnections[LeftConnections.Count - 1];
            Debug.Log(l.gameObject.name);
            l.rectTransform.GetWorldCorners(lCorners);
            if (ExtraUtilities.isEqualWithTolarance(corners[0].y, lCorners[3].y, 0.01f))
            {
                foreach (AvowComponent b in l.BotConnections)
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
            r.rectTransform.GetWorldCorners(rCorners);
            Debug.Log(r.gameObject.name);
           Debug.Log(corners[3].y+ "   "+ rCorners[0].y);
            if (ExtraUtilities.isEqualWithTolarance(corners[3].y, rCorners[0].y, 0.01f))
            {
                foreach (AvowComponent b in r.BotConnections)
                {
                    if (!this.BotConnections.Contains(b) && b != this)
                    {
                        this.BotConnections.Add(b);
                    }
                }

            }
        }
        BotConnections.Sort((x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x));
    }




    public void updateConnections()
    {


        Vector2 sizeDelta = rectTransform.sizeDelta;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);


        //TopConnections
        Collider2D[] hitsTOP = Physics2D.OverlapAreaAll(new Vector2(corners[1].x, corners[1].y), new Vector2(corners[2].x, corners[2].y + 0.1f));
        Array.Sort(hitsTOP, (x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x));
        if (hitsTOP.Length > 0)
        {
            foreach (Collider2D hit in hitsTOP)
            {
                if (hit.TryGetComponent(out AvowComponent avowHit))
                {
                    if (avowHit != this) TopConnections.Add(avowHit);
                }

            }
        }
        //BotConnections
        Collider2D[] hitsBOT = Physics2D.OverlapAreaAll(new Vector2(corners[0].x, corners[0].y), new Vector2(corners[3].x, corners[3].y - 0.1f));
        Array.Sort(hitsBOT, (x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x));
        if (hitsBOT.Length > 0)
        {
            foreach (Collider2D hit in hitsBOT)
            {
                if (hit.TryGetComponent(out AvowComponent avowHit))
                {
                    if (avowHit != this) BotConnections.Add(avowHit);
                }

            }
        }
        //LeftConnections
        Collider2D[] hitsL = Physics2D.OverlapAreaAll(new Vector2(corners[0].x, corners[0].y), new Vector2(corners[1].x - 0.1f, corners[1].y));
        if (hitsL.Length > 0)
        {
            Array.Sort(hitsL, (x1, x2) => x2.transform.position.y.CompareTo(x1.transform.position.y));
            foreach (Collider2D hit in hitsL)
            {
                if (hit.TryGetComponent(out AvowComponent avowHit))
                {
                    if (avowHit != this) LeftConnections.Add(avowHit);
                }

            }
        }
        Collider2D[] hitsR = Physics2D.OverlapAreaAll(new Vector2(corners[2].x, corners[2].y), new Vector2(corners[3].x + 0.1f, corners[3].y));
        if (hitsR.Length > 0)
        {

            Array.Sort(hitsR, (x1, x2) => x2.transform.position.y.CompareTo(x1.transform.position.y));
            foreach (Collider2D hit in hitsR)
            {
                if (hit.TryGetComponent(out AvowComponent avowHit))
                {
                    if (avowHit != this) RightConnections.Add(avowHit);
                }

            }
        }


        //FIX


    }




    public void addToConnections(AvowComponent avow, char direction)
    {
        switch (direction)
        {
            case 'U':
                TopConnections.Add(avow);
                return;
            case 'D':
                BotConnections.Add(avow);
                return;
            case 'L':
                LeftConnections.Add(avow);
                return;
            case 'R':
                RightConnections.Add(avow);
                return;
            default:
                Debug.LogError(this.name + " delete call with unknown direction: " + direction);
                return;

        }
    }

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

    public void removeAvowConnection(AvowComponent avowComponent)
    {
        TopConnections.Remove(avowComponent);
        BotConnections.Remove(avowComponent);
        RightConnections.Remove(avowComponent);
        LeftConnections.Remove(avowComponent);
    }

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
