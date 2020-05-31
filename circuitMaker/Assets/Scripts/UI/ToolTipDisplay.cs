using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI class attempted to gameobjects to show a tooltip
/// </summary>
public class ToolTipDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public bool UseObjectName = false;//display object name instead of the string given, used for debugging
    public string TooltipMessage; //string to display to user
         public void OnPointerEnter(PointerEventData eventData) //if pointer enter gameobject, show tooltip with given string
     {
         if(!UseObjectName)Tooltip.ShowTooltip_Static(TooltipMessage);
         else Tooltip.ShowTooltip_Static(gameObject.name);
     }
 
     public void OnPointerExit(PointerEventData eventData) //if exit hide pointer
     {
         Tooltip.HideTooltip_Static();
     }


    
}
