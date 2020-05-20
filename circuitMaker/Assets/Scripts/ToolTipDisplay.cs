using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string TooltipMessage;
         public void OnPointerEnter(PointerEventData eventData)
     {
         Tooltip.ShowTooltip_Static(TooltipMessage);
     }
 
     public void OnPointerExit(PointerEventData eventData)
     {
         Tooltip.HideTooltip_Static();
     }


    
}
