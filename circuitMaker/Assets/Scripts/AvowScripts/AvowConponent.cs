using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class AvowConponent : MonoBehaviour
{
    public DiagramComponent component;
    public Color fillColour, connectedColor = Color.green, hoverColor = Color.gray, selectedColor = Color.magenta, errorColor = Color.red, pastColor;
    public RectTransform AvowFillColorTrans;
    public BoxCollider2D boxCollider2D;
    private Image AvowFillColorImg;
    private AvowManager avowManager;


[SerializeField]
    public List<AvowConponent> TopConnections, BotConnections, LeftConnections, RightConnections;

    Image image;
     public RectTransform rectTransform;

    public float voltage = 1, current = 1;
    Vector2 fillSize;



    [HideInInspector]
    public string name;



    public Canvas ValuesUX;


    // Start is called before the first frame update
    private void Awake()
    {
       
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        rectTransform = this.GetComponent<RectTransform>();
        image = this.GetComponent<Image>();
        AvowFillColorTrans = transform.GetChild(0).GetComponent<RectTransform>();
        AvowFillColorImg = AvowFillColorTrans.GetComponent<Image>();
        TopConnections = new List<AvowConponent>();
        BotConnections = new List<AvowConponent>();
        LeftConnections = new List<AvowConponent>();
        RightConnections = new List<AvowConponent>();
        avowManager = transform.GetComponentInParent<AvowManager>();


    }

    // Update is called once per frame
    void Update()
    {
        updateFill();
        this.GetComponent<BoxCollider2D>().size = fillSize;
        Collider2D[] hit = new Collider2D[10];
        Color pastColor = AvowFillColorImg.color;
        if(boxCollider2D.OverlapCollider(new ContactFilter2D(),  hit) >0){

           AvowFillColorImg.color = Color.red;
        }
        else{
            if(AvowFillColorImg.color == Color.red){
                ColorToMain();
            }
            else{
                AvowFillColorImg.color = pastColor;

            }
            
        }
 
        

    }



    private void updateFill()
    {
        //temp
        updateSize(voltage, current);
        fillSize = new Vector2(rectTransform.rect.width - (0.05f *  (Camera.main.orthographicSize/ 10)), rectTransform.rect.height
         - (0.05f *  (Camera.main.orthographicSize/10)));

        AvowFillColorTrans.sizeDelta = fillSize;



    }

    public void updateSize(float voltage, float current)
    {
        if(current >0 && voltage > 0){
        rectTransform.sizeDelta = new Vector2(current * avowManager.scale, voltage * avowManager.scale);
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

    public void ColorToHover(){
        AvowFillColorImg.color = hoverColor;
        foreach(AvowConponent avow in TopConnections){
            avow.ColorToConnected();
        }
        foreach(AvowConponent avow in BotConnections){
            avow.ColorToConnected();
        }
        foreach(AvowConponent avow in RightConnections){
            avow.ColorToConnected();
        }
        foreach(AvowConponent avow in LeftConnections){
            avow.ColorToConnected();
        }


    }

        public void ColorToSelected(){
        AvowFillColorImg.color = selectedColor;

    }

        public void ColorToParam(Color color){
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


    public void updateConnections()
    {
        TopConnections.Clear();
        BotConnections.Clear();
        RightConnections.Clear();
        LeftConnections.Clear();
        Vector2 sizeDelta = rectTransform.sizeDelta;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        // Debug.Log("update connection");

        //TopConnections
        Collider2D[] hitsTOP = Physics2D.OverlapAreaAll(new Vector2(corners[1].x, corners[1].y), new Vector2(corners[2].x, corners[2].y + 0.1f));
        Array.Sort(hitsTOP, (x1, x2) => x1.transform.position.x.CompareTo(x2.transform.position.x));
        if (hitsTOP.Length > 0)
        {
            foreach (Collider2D hit in hitsTOP)
            {
                if (hit.TryGetComponent(out AvowConponent avowHit))
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
                if (hit.TryGetComponent(out AvowConponent avowHit))
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
                if (hit.TryGetComponent(out AvowConponent avowHit))
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
                if (hit.TryGetComponent(out AvowConponent avowHit))
                {
                    if (avowHit != this) RightConnections.Add(avowHit);
                }

            }
        }

    }

    public void addToConnections(AvowConponent avow, char direction)
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

    private void OnMouseDown() {
        try{
                    GameObject.Find("ValuesPanel").GetComponent<AvowValuesPanel>().newSelected(this);
        }
        catch(System.Exception ex){
            Debug.LogError("MISSING OR CANT FIND VALUES PANEL\n"+ ex.ToString());
        }

    }

    public void removeAvowConnection(AvowConponent avowConponent){
        TopConnections.Remove(avowConponent);
        BotConnections.Remove(avowConponent);
        RightConnections.Remove(avowConponent);
        LeftConnections.Remove(avowConponent);
    }

    private void OnDestroy() {
        this.GetComponent<AvowDragDrop>().enabled = false;
        try{
            GetComponentInParent<AvowManager>().removeAvow(this);
        }
        catch(System.Exception ex){
            Debug.LogWarning(ex.Message);

        }
    }


}
