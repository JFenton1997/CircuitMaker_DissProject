using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HeldDown : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent method;
    private bool ispressed;


    private void Update() {
        if(Input.GetMouseButtonUp(0)){
            ispressed = false;
        }


        if(ispressed){
            method.Invoke();
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ispressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ispressed = false;
    }



}
