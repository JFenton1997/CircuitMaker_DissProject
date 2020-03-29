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
    public Component currentlySelected;

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



    }

    public void newComponentSelected(Component component)
    {
        if (!currentlySelected)
        {
            SendMessage("GridMoveStart");
            window.enabled = true;
            Debug.Log("selected new compnent");
            window.transform.position = component.transform.position;
            this.currentlySelected = component;
            if (component.direction == Direction.B_to_A)
            {
                directionSwitch.State = 0;
            }
            else
            {
                directionSwitch.State = 1;
            }
            updateDisplayValues();
            typeText.text = currentlySelected.type.ToString();
        }

    }


    // Update is called once per frame

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            if (currentlySelected)
            {
                closeWindow();
            }
        }
    }

    public void updateDisplayValues()
    {
        setDirectionGraphic();
        compnentName.text = currentlySelected.GetComponent<CircuitComponent>().name;
        voltageText.text = currentlySelected.Values[ComponentParameter.VOLTAGE].value.ToString();
        voltageToggle.isOn = currentlySelected.Values[ComponentParameter.VOLTAGE].hidden;
        currentText.text = currentlySelected.Values[ComponentParameter.CURRENT].value.ToString();
        currentToggle.isOn = currentlySelected.Values[ComponentParameter.CURRENT].hidden;
        resistanceText.text = currentlySelected.Values[ComponentParameter.RESISTANCE].value.ToString();
        resistanceToggle.isOn = currentlySelected.Values[ComponentParameter.RESISTANCE].hidden;



    }

    public void updateComponentInfo()
    {
        compnentName.interactable = false;
        compnentName.selectionColor = Color.clear;
        if (voltageText.text == "")
        {
            voltageText.text = currentlySelected.Values[ComponentParameter.VOLTAGE].value.ToString();
            Debug.Log("empty");
        }
        if (currentText.text == "")
        {
            currentText.text = currentlySelected.Values[ComponentParameter.CURRENT].value.ToString();
        }
        if (resistanceText.text == "")
        {
            resistanceText.text = currentlySelected.Values[ComponentParameter.RESISTANCE].value.ToString();
        }
        if (compnentName.text == "")
        {
            compnentName.text = currentlySelected.GetComponent<CircuitComponent>().name;
        }


        currentlySelected.Values[ComponentParameter.VOLTAGE].value = float.Parse(voltageText.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        currentlySelected.Values[ComponentParameter.VOLTAGE].hidden = voltageToggle.isOn;

        currentlySelected.Values[ComponentParameter.CURRENT].value = float.Parse(currentText.text
       , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        currentlySelected.Values[ComponentParameter.CURRENT].hidden = currentToggle.isOn;

        currentlySelected.Values[ComponentParameter.RESISTANCE].value = float.Parse(resistanceText.text
       , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        currentlySelected.Values[ComponentParameter.RESISTANCE].hidden = resistanceToggle.isOn;

        currentlySelected.GetComponent<CircuitComponent>().name = compnentName.text;
        if (directionSwitch.State == 0)
        {
            currentlySelected.direction = Direction.B_to_A;
        }
        else
        {
            currentlySelected.direction = Direction.A_to_B;
        }
        currentlySelected.GetComponent<CircuitComponent>().updateUXValues();
        updateDisplayValues();
    }


    private void setDirectionGraphic()
    {
        if (directionSwitch.State == 0)
        {
            currentlySelected.direction = Direction.B_to_A;
            left.color = onColor;
            right.color = offColor;
        }
        else
        {
            currentlySelected.direction = Direction.A_to_B;
            left.color = offColor;
            right.color = onColor;

        }
    }

    public void closeWindow()
    {
        SendMessage("GridMoveEnded");
        currentlySelected = null;
        window.enabled = false;
    }

    public void editName(){
        compnentName.interactable = true;
        compnentName.selectionColor = Color.gray;
        compnentName.Select();
        compnentName.ActivateInputField();
    }

    public void deleteComponent(){
        Destroy(currentlySelected.gameObject);
        closeWindow();
    }
}
