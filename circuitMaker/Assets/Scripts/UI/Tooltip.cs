using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{

    private static Tooltip instance;
    private Text tooltipText;
    private RectTransform backgroundRectTrans;
    private CanvasGroup canvasGroup;
    public float textPadding = 2f;
    private void Awake()
    {
        backgroundRectTrans = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = transform.Find("text").GetComponent<Text>();
        canvasGroup = transform.GetComponent<CanvasGroup>();

        instance = this;
        HideTooltip();


    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        transform.localPosition = localPoint;
        if(GlobalValues.ToolTipsEnabled){
            canvasGroup.alpha = 1f;
        }
        else{
            canvasGroup.alpha = 0f;
        }
         if(Input.mousePosition.x <= Screen.width *0.50){
             backgroundRectTrans.anchorMin = new Vector2(0f,0f);
            backgroundRectTrans.anchorMax = new Vector2(0f,0f);
             tooltipText.rectTransform.anchorMin = new Vector2(0f,0f);
            tooltipText.rectTransform.anchorMax = new Vector2(0f,0f);
            
            


         }else{
            transform.localPosition = new Vector2(localPoint.x - backgroundRectTrans.sizeDelta.x, localPoint.y);
        }
    }


    private void ShowTooltip(string TooltipString)
    {
            gameObject.SetActive(true);

            tooltipText.text = TooltipString;           

            Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPadding * 2f, tooltipText.preferredHeight + textPadding * 2f);
            backgroundRectTrans.sizeDelta = backgroundSize;

    }


    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString)
    {
        instance.ShowTooltip(tooltipString);
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();

    }
}
