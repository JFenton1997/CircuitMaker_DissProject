using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI class which operates the tooltip
/// </summary>
public class Tooltip : MonoBehaviour
{

    private static Tooltip instance;
    private Text tooltipText;
    private RectTransform backgroundRectTrans;
    private CanvasGroup canvasGroup;
    public float textPadding = 2f;

    /// <summary>
    /// get UI components and hide self
    /// </summary>
    private void Awake()
    {
        backgroundRectTrans = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = transform.Find("text").GetComponent<Text>();
        canvasGroup = transform.GetComponent<CanvasGroup>();

        instance = this;
        HideTooltip();


    }

/// <summary>
/// on each frame, get the location of mousepoint and set the local position of the tooltip to the mousepointer, if tooltip is enable show, else hide
/// </summary>
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
        //if cursor in left side of the screen, have the tool tip come out for the right side of the mouse
         if(Input.mousePosition.x <= Screen.width *0.50){
             backgroundRectTrans.anchorMin = new Vector2(0f,0f);
            backgroundRectTrans.anchorMax = new Vector2(0f,0f);
             tooltipText.rectTransform.anchorMin = new Vector2(0f,0f);
            tooltipText.rectTransform.anchorMax = new Vector2(0f,0f);
            
            


         }else{
             // have tooltip come out of the right side of the mouse 
            transform.localPosition = new Vector2(localPoint.x - backgroundRectTrans.sizeDelta.x, localPoint.y);
        }
    }


    /// <summary>
    /// show tooltip with a written sting
    /// </summary>
    /// <param name="TooltipString">string to display to user</param>
    private void ShowTooltip(string TooltipString)
    {
            gameObject.SetActive(true);

            tooltipText.text = TooltipString;           
            //set background to match tooltip text
            Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPadding * 2f, tooltipText.preferredHeight + textPadding * 2f);
            backgroundRectTrans.sizeDelta = backgroundSize;

    }


/// <summary>
/// hides tooltip
/// </summary>
    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

/// <summary>
/// static method to show tooltip
/// </summary>
/// <param name="tooltipString">string to display to the user</param>
    public static void ShowTooltip_Static(string tooltipString)
    {
        instance.ShowTooltip(tooltipString);
    }

/// <summary>
/// static method to hide the tooltip
/// </summary>
    public static void HideTooltip_Static()
    {
        instance.HideTooltip();

    }
}
