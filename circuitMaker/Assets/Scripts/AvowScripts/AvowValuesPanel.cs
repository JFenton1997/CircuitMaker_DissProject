using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utilities;
using System;

/// <summary>
/// class which operates the avow value panel, including editing values of selected avows
/// </summary>
public class AvowValuesPanel : MonoBehaviour
{

    private AvowComponent currentAvow;
    public InputField height, width, selectedText;
    public Toggle voltHidden, currentHidden, resistanceHidden;
    public Dropdown AvowComponentType;

    private CanvasGroup canvasGroup;
    // Start is called before the first frame update


/// <summary>
/// get canvas group
/// </summary>
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }


    /// <summary>
    /// set selected to selected color , if rightclick, unselect
    /// </summary>
    private void Update()
    {
        // if none selected, hide
        if (!currentAvow)
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;


        }
        else
        {
            currentAvow.ColorToSelected();
        }

        if (Input.GetMouseButtonDown(1) && currentAvow)
        {
            updateAvowValues();
            currentAvow.ColorToMain();
            currentAvow = null;

        }


    }



/// <summary>
/// set UI components to show the values of the newly selected Avow
/// </summary>
/// <param name="avowComponent"> new selected Avow</param>
    public void newSelected(AvowComponent avowComponent)
    {

        if (currentAvow && currentAvow != avowComponent){
            updateAvowValues();
            currentAvow.ColorToMain();
        }



        currentAvow = null;
        canvasGroup.blocksRaycasts = true;


        currentAvow = avowComponent;
        currentAvow.ColorToSelected();
        canvasGroup.alpha = 1f;
        selectedText.text = currentAvow.gameObject.name;
        width.text = avowComponent.current.ToString();
        height.text = avowComponent.voltage.ToString();
        AvowComponentType.value = (int)currentAvow.component.type - 2;

        if (currentAvow)
        {
            // if a builder, show hidden toggles
            if (currentAvow.isBuilder)
            {
                Debug.Log("newSelected");
                currentHidden.transform.parent.gameObject.SetActive(true);
                voltHidden.isOn = currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden;
                Debug.Log(currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden);
                currentHidden.isOn = currentAvow.component.Values[ComponentParameter.CURRENT].hidden;
                resistanceHidden.isOn = currentAvow.component.Values[ComponentParameter.RESISTANCE].hidden;
            }
            else
            {
                // dont show hidden
                Debug.Log("noBuilder");
                currentHidden.transform.parent.gameObject.SetActive(false);

            }
        }

    }


/// <summary>
/// run this method on any UI element change, get UI element values and set them to the selected avow 
/// </summary>
    public void updateAvowValues()
    {
        if (float.Parse(height.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) >= 1f)
            currentAvow.voltage = (float)Math.Round(decimal.Parse(height.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

        if (float.Parse(width.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) >= 1f)
            currentAvow.current = (float)Math.Round(decimal.Parse(width.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

        if (selectedText.text != "")
        {
            currentAvow.name = selectedText.text;
            currentAvow.gameObject.name = selectedText.text;
            currentAvow.component.name = selectedText.text;
        }
        Debug.Log(currentAvow.current + "  " + currentAvow.voltage);

        
        currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden = voltHidden.isOn;
        currentAvow.component.Values[ComponentParameter.CURRENT].hidden = currentHidden.isOn;
        currentAvow.component.Values[ComponentParameter.RESISTANCE].hidden = resistanceHidden.isOn;
        currentAvow.component.type = (ComponentType)AvowComponentType.value + 2;

        //         Debug.Log(currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden+" "+
        // currentAvow.component.Values[ComponentParameter.CURRENT].hidden +" "+currentAvow.component.Values[ComponentParameter.RESISTANCE ].hidden);
        newSelected(currentAvow);

    }

/// <summary>
/// call on destoy button press, destory current and hide
/// </summary>
    public void DestoryCurrentSelected()
    {
        if (currentAvow)
        {
            Destroy(currentAvow.gameObject);
            currentAvow = null;
            Cursor.visible = true;
        }
    }


/// <summary>
/// call on close button press, unselect current
/// </summary>
    public void Close()
    {
        if (currentAvow)
        {
            currentAvow = null;
        }
    }


}
