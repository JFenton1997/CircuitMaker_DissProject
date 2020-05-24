using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using UnityEngine.EventSystems;
using Utilities;

public class CircuitValuesPanel : MonoBehaviour
{


    private CircuitComponent currentCircuit;
    private InputField voltage, current, resistance, selectedText;
    private Toggle voltHidden, currentHidden, resistanceHidden;
    private Dropdown conponentType;
    private LeanSwitch direction;

    private CanvasGroup canvasGroup;
    private Image upArrow, downArrow;
    private Color onColor, offColor;
    private GameObject autoV, autoC;
    // Start is called before the first frame update

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        voltage = transform.Find("Values/Voltage").GetComponent<InputField>();
        current = transform.Find("Values/Current").GetComponent<InputField>();
        resistance = transform.Find("Values/Resistance").GetComponent<InputField>();
        selectedText = transform.Find("Values/SelectedName").GetComponent<InputField>();

        voltHidden = transform.Find("Values/Voltage/Hidden").GetComponent<Toggle>();
        currentHidden = transform.Find("Values/Current/Hidden").GetComponent<Toggle>();
        resistanceHidden = transform.Find("Values/Resistance/Hidden").GetComponent<Toggle>();

        conponentType = transform.Find("Values/Dropdown").GetComponent<Dropdown>();

        direction = transform.Find("Values/DirectionSelector").GetComponent<LeanSwitch>();
        upArrow = direction.transform.Find("BtoA (R)").GetComponent<Image>();
        downArrow = direction.transform.Find("AtoB (L)").GetComponent<Image>();
        onColor = Color.white;
        offColor = upArrow.color;

        autoV = voltage.transform.Find("AutoV").gameObject;
        autoC = current.transform.Find("AutoC").gameObject;
    }

    private void Update()
    {
        if (!currentCircuit)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;


        }
        else
        {

            // currentCircuit.ColorToSelected();
        }

        if (Input.GetMouseButtonDown(1) && currentCircuit)
        {
            updateConponentValues();
            //   currentCircuit.ColorToMain();
            currentCircuit.hideHighlight();
            currentCircuit = null;

        }


        if (currentCircuit)
        {
            if (direction.State == 0)
            {
                upArrow.color = offColor;
                downArrow.color = onColor;
            }
            else
            {
                upArrow.color = onColor;
                downArrow.color = offColor;
            }
        }

    }



    public void newSelected(CircuitComponent selectedCoponent)
    {
        if (currentCircuit)
        {
            updateConponentValues();
            currentCircuit.hideHighlight();
        }

        //if(currentCircuit && currentCircuit != selectedCoponent) currentCircuit.ColorToMain();
        this.currentCircuit = null;


        // canvasGroup.blocksRaycasts = true;


        this.currentCircuit = selectedCoponent;
        currentCircuit.ShowHighlight();
        // currentCircuit.ColorToSelected();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        selectedText.text = currentCircuit.gameObject.name;
        voltage.text = currentCircuit.conponent.Values[ComponentParameter.VOLTAGE].value.ToString();
        current.text = currentCircuit.conponent.Values[ComponentParameter.CURRENT].value.ToString();
        resistance.text = currentCircuit.conponent.Values[ComponentParameter.RESISTANCE].value.ToString();
        conponentType.value = (int)currentCircuit.conponent.type - 1;
        voltHidden.isOn = currentCircuit.conponent.Values[ComponentParameter.VOLTAGE].hidden;
        currentHidden.isOn = currentCircuit.conponent.Values[ComponentParameter.CURRENT].hidden;
        resistanceHidden.isOn = currentCircuit.conponent.Values[ComponentParameter.RESISTANCE].hidden;





    }



    public void updateConponentValues()
    {
        if (float.Parse(voltage.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > 0)
            currentCircuit.conponent.Values[ComponentParameter.VOLTAGE].value = double.Parse(voltage.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        if (float.Parse(current.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > 0)
            currentCircuit.conponent.Values[ComponentParameter.CURRENT].value = double.Parse(current.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        if (float.Parse(resistance.text
                 , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > 0 && conponentType.value != 0)
            currentCircuit.conponent.Values[ComponentParameter.RESISTANCE].value = double.Parse(resistance.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);


        if (conponentType.value == 0)
        {
            currentCircuit.conponent.Values[ComponentParameter.RESISTANCE].value = 0;
            resistance.gameObject.SetActive(false);
            direction.gameObject.SetActive(false);
            autoC.SetActive(false);
            autoV.SetActive(false);
        }
        else
        {
            resistance.gameObject.SetActive(true);
            direction.gameObject.SetActive(true);
            autoC.SetActive(true);
            autoV.SetActive(true);
        }

        if (direction.State == 0)
        {
            currentCircuit.conponent.direction = Direction.A_to_B;
        }
        else
        {
            currentCircuit.conponent.direction = Direction.B_to_A;
        }



        if (selectedText.text != "")
        {
            currentCircuit.name = selectedText.text;
            currentCircuit.gameObject.name = selectedText.text;
        }

        currentCircuit.conponent.type = (ComponentType)conponentType.value + 1;
        conponentType.value = (int)currentCircuit.conponent.type - 1;

        currentCircuit.conponent.Values[ComponentParameter.VOLTAGE].hidden = voltHidden.isOn;
        currentCircuit.conponent.Values[ComponentParameter.CURRENT].hidden = currentHidden.isOn;
        currentCircuit.conponent.Values[ComponentParameter.RESISTANCE].hidden = resistanceHidden.isOn;

        //         Debug.Log(currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden+" "+
        // currentAvow.component.Values[ComponentParameter.CURRENT].hidden +" "+currentAvow.component.Values[ComponentParameter.RESISTANCE ].hidden);

    }

    public void DestoryCurrentSelected()
    {
        if (currentCircuit)
        {
            Destroy(currentCircuit.gameObject);
            currentCircuit = null;
            Cursor.visible = true;
        }
    }

    public void btnAutoV()
    {
        autoPick(ComponentParameter.VOLTAGE);
    }

    public void btnAutoC()
    {
        autoPick(ComponentParameter.CURRENT);
    }

    public void btnAutoR()
    {
        autoPick(ComponentParameter.RESISTANCE);
    }

    public void autoPick(ComponentParameter c)
    {
        double voltageT = 0f;
        double currentT = 0f;
        double resistanceT = 0f;
        if (float.Parse(voltage.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > 0)
            voltageT = double.Parse(voltage.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        if (float.Parse(current.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > 0)
            currentT = double.Parse(current.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        if (float.Parse(resistance.text
                 , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > 0 && conponentType.value != 0)
            resistanceT = double.Parse(resistance.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        if (voltageT > 0f && currentT > 0f && resistanceT > 0)
        {

            switch (c)
            {
                case ComponentParameter.VOLTAGE:
                    voltage.text = (resistanceT * currentT).ToString();
                    break;
                case ComponentParameter.CURRENT:
                    current.text = (voltageT / resistanceT).ToString();
                    break;
                case ComponentParameter.RESISTANCE:
                    resistance.text = (voltageT / currentT).ToString();
                    break;
                default:
                    Debug.LogError("invalide type");
                    break;
            }
            updateConponentValues();
        }




    }

    public void close()
    {
        currentCircuit.hideHighlight();
        currentCircuit = null;
    }

    public void delete()
    {
        currentCircuit.hideHighlight();
        currentCircuit.removeWireConnections();
        GameObject.Destroy(currentCircuit.gameObject);
        close();
    }

    public void Move()
    {
        currentCircuit.hideHighlight();
        currentCircuit.removeWireConnections();
        currentCircuit.GetComponent<CircuitClickAndDrag>().MoveStart();
        close();
    }


}



