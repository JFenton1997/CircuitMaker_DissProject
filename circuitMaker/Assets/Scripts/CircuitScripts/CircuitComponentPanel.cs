using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Utilities;
public class CircuitComponentPanel : MonoBehaviour
{
    [Header("Core")]
    public Canvas window;
    public Text typeText;
    public InputField compnentName;
    public CircuitComponent currentlySelectedCircuitComponent;
    public DiagramComponent currentlySelectedComponent;

    [Header("Voltage")]
    public InputField voltageText;
    public Toggle voltageToggle;

    [Header("Current")]
    public InputField currentText;
    public Toggle currentToggle;

    [Header("Resistance")]
    public InputField resistanceText;
    public Toggle resistanceToggle;


    [Header("DirectionSwitch")]
    public LeanSwitch directionSwitch;
    public Image left;
    public Image right;
    public Color onColor;
    public Color offColor;






    // Start is called before the first frame update
    private void Awake()
    {
        currentlySelectedComponent= null;


    }

    public void newComponentSelected(CircuitComponent circuitComponent)
    {
        if (currentlySelectedComponent == null)
        {
            DiagramComponent component = circuitComponent.component;
            SendMessageUpwards("GridMoveStart");
            window.enabled = true;
            Debug.Log("selected new compnent");
            window.transform.position = circuitComponent.transform.position;
            this.currentlySelectedComponent = component;
            this.currentlySelectedCircuitComponent = circuitComponent;
            if (component.direction == Direction.B_to_A)
            {
                directionSwitch.State = 0;
            }
            else
            {
                directionSwitch.State = 1;
            }
            updateDisplayValues();
            typeText.text = component.type.ToString();
        }

    }


    // Update is called once per frame

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            if (currentlySelectedComponent != null)
            {
                closeWindow();
            }
        }
    }

    public void updateDisplayValues()
    {
        setDirectionGraphic();
        compnentName.text = currentlySelectedCircuitComponent.name;
        voltageText.text = currentlySelectedComponent.Values[ComponentParameter.VOLTAGE].value.ToString();
        voltageToggle.isOn = currentlySelectedComponent.Values[ComponentParameter.VOLTAGE].hidden;
        currentText.text = currentlySelectedComponent.Values[ComponentParameter.CURRENT].value.ToString();
        currentToggle.isOn = currentlySelectedComponent.Values[ComponentParameter.CURRENT].hidden;
        resistanceText.text = currentlySelectedComponent.Values[ComponentParameter.RESISTANCE].value.ToString();
        resistanceToggle.isOn = currentlySelectedComponent.Values[ComponentParameter.RESISTANCE].hidden;



    }

    public void updateComponentInfo()
    {
        compnentName.interactable = false;
        compnentName.selectionColor = Color.clear;
        if (voltageText.text == "")
        {
            voltageText.text = currentlySelectedComponent.Values[ComponentParameter.VOLTAGE].value.ToString();
            Debug.Log("empty");
        }
        if (currentText.text == "")
        {
            currentText.text = currentlySelectedComponent.Values[ComponentParameter.CURRENT].value.ToString();
        }
        if (resistanceText.text == "")
        {
            resistanceText.text = currentlySelectedComponent.Values[ComponentParameter.RESISTANCE].value.ToString();
        }
        if (compnentName.text == "")
        {
            compnentName.text = currentlySelectedCircuitComponent.name;
        }


        currentlySelectedComponent.Values[ComponentParameter.VOLTAGE].value = float.Parse(voltageText.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        currentlySelectedComponent.Values[ComponentParameter.VOLTAGE].hidden = voltageToggle.isOn;

        currentlySelectedComponent.Values[ComponentParameter.CURRENT].value = float.Parse(currentText.text
       , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        currentlySelectedComponent.Values[ComponentParameter.CURRENT].hidden = currentToggle.isOn;

        currentlySelectedComponent.Values[ComponentParameter.RESISTANCE].value = float.Parse(resistanceText.text
       , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        currentlySelectedComponent.Values[ComponentParameter.RESISTANCE].hidden = resistanceToggle.isOn;

        currentlySelectedCircuitComponent.name= compnentName.text;
        if (directionSwitch.State == 0)
        {
            currentlySelectedComponent.direction = Direction.B_to_A;
        }
        else
        {
            currentlySelectedComponent.direction = Direction.A_to_B;
        }
        currentlySelectedCircuitComponent.updateUXValues();
        updateDisplayValues();
    }


    private void setDirectionGraphic()
    {
        if (directionSwitch.State == 0)
        {
            currentlySelectedComponent.direction = Direction.B_to_A;
            left.color = onColor;
            right.color = offColor;
        }
        else
        {
            currentlySelectedComponent.direction = Direction.A_to_B;
            left.color = offColor;
            right.color = onColor;

        }
    }

    public void closeWindow()
    {
        SendMessageUpwards("GridMoveEnded");
        currentlySelectedComponent = null;
        currentlySelectedCircuitComponent=null;
        window.enabled = false;
    }

    public void editName(){
        compnentName.interactable = true;
        compnentName.selectionColor = Color.gray;
        compnentName.Select();
        compnentName.ActivateInputField();
    }

    public void deleteComponent(){
        Destroy(currentlySelectedCircuitComponent.gameObject);
        closeWindow();
    }
}
