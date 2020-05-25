using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool UseObjectName = false;
    public string TooltipMessage;
         public void OnPointerEnter(PointerEventData eventData)
     {
         if(!UseObjectName)Tooltip.ShowTooltip_Static(TooltipMessage);
         else Tooltip.ShowTooltip_Static(gameObject.name);
     }
 
     public void OnPointerExit(PointerEventData eventData)
     {
         Tooltip.HideTooltip_Static();
     }


    
}
