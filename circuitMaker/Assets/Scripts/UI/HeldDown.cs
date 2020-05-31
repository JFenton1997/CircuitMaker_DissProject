using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// UI class to invoke a method continuously while mouse is held down over it
/// </summary>
public class HeldDown : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent method; //method to invoke, set in inspector
    private bool ispressed; //if currently pressed


    /// <summary>
    /// if button isnt up, invoke method each frame
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ispressed = false;
        }


        if (ispressed)
        {
            method.Invoke();
        }


    }

    /// <summary>
    /// if pointer is down on gameobject, set isPressed to true
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        ispressed = true;
    }

    /// <summary>
    /// if pointer ever is up, stopped ispressed
    /// </summary>
    /// <param name="eventData"></param>

    public void OnPointerUp(PointerEventData eventData)
    {
        ispressed = false;
    }



}
