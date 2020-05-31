using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// class operating majority of keypresses, futureproofed for keybinding
/// </summary>
public class UserControls : MonoBehaviour
{
    // get values set in inspector and key trigger
    public KeyCode toggleSnap = KeyCode.LeftShift; 
    public KeyCode plusOffset = KeyCode.Plus;
    public KeyCode minusOffset = KeyCode.Minus;
    public KeyCode toggleToolTips = KeyCode.T;

    private Toggle avowSnapToggle;
    private bool avowSnapToggleLastPos;

    public KeyCode circuitDisplayAll = KeyCode.LeftShift;
    // Start is called before the first frame update


// set values to be used
    private void Start()
    {
 
        GlobalValues.AvowSnapping = false;
        GlobalValues.AvowSnappingOffset = 0.5f;
        avowSnapToggleLastPos = false;

        try
        {
            transform.Find("/UI/BotAvow/Snapping/Toggle").TryGetComponent<Toggle>(out avowSnapToggle);


        }
        catch
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        //if button do x
        if(avowSnapToggle)
        if(Input.GetKeyDown(toggleSnap))avowSnapToggleLastPos = avowSnapToggle.isOn;
        if (Input.GetKey(toggleSnap)|| Input.GetKeyDown(toggleSnap))
        {
            GlobalValues.AvowSnapping = true;
           if(avowSnapToggle)
            avowSnapToggle.isOn = true;

        }
        else
        {
            GlobalValues.AvowSnapping = false;
            
        }

        if(avowSnapToggle) if(Input.GetKeyUp(toggleSnap)) avowSnapToggle.isOn = avowSnapToggleLastPos;

        if (avowSnapToggle) if (avowSnapToggle.isOn == true) GlobalValues.AvowSnapping = true;



        if (Input.GetKeyDown(plusOffset) && Input.GetKey(toggleSnap)&& GlobalValues.AvowSnappingOffset < 2.9f)
        {

            GlobalValues.AvowSnappingOffset += 0.1f;
            Debug.Log("Snapping increassed to: " + GlobalValues.AvowSnappingOffset);
        }


        if (Input.GetKeyDown(minusOffset) && Input.GetKey(toggleSnap) && GlobalValues.AvowSnappingOffset > 0.1f)
        {
            GlobalValues.AvowSnappingOffset -= 0.1f;
            Debug.Log("Snapping increassed to: " + GlobalValues.AvowSnappingOffset);
        }

        if (Input.GetKeyDown(toggleToolTips))
        {
            GlobalValues.ToolTipsEnabled = !GlobalValues.ToolTipsEnabled;
        }


        if (Input.GetKeyDown(circuitDisplayAll))
        {
            GlobalValues.circuitDisplayAll = !GlobalValues.circuitDisplayAll;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitScript exit = transform.Find("/UI/Exit").GetComponent<ExitScript>();
            if (exit.isShowing)
            {
                exit.hideExitPanel();
            }
            else
            {
                exit.displayExitPanel();
            }
        }


    }
}

