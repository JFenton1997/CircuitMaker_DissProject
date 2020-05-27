using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
public class DisplayConponentValues : MonoBehaviour
{

    public Sprite upArrow, downArrow;
    private Text voltage, current, resistance, type, name;
    private Image direction;
    private DiagramComponent conponent;
    private CircuitComponent circuitComponent;
    private CanvasGroup canvasGroup;
    private GenerateCircuit foundGen;

    // Start is called before the first frame update
    void Start()
    {
        voltage = transform.Find("Voltage").GetComponent<Text>();
        current = transform.Find("Current").GetComponent<Text>();
        resistance = transform.Find("Resistance").GetComponent<Text>();
        type = transform.Find("Type").GetComponent<Text>();
        name = transform.Find("Name").GetComponent<Text>();

        direction = transform.Find("Direction").GetComponent<Image>();
        circuitComponent = transform.parent.GetComponent<CircuitComponent>();
        conponent = circuitComponent.conponent;
        foundGen = circuitComponent.foundGen;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;


    }


    void Update()
    {
        if (conponent.Values[ComponentParameter.VOLTAGE].hidden && foundGen)
        {
            voltage.text = "HIDDEN";
        }
        else
        {
            voltage.text = conponent.Values[ComponentParameter.VOLTAGE].value.ToString();
        }

        if (conponent.Values[ComponentParameter.CURRENT].hidden && foundGen)
        {
            current.text = "HIDDEN";
        }
        else
        {
            current.text = conponent.Values[ComponentParameter.CURRENT].value.ToString();
        }

        if (conponent.Values[ComponentParameter.RESISTANCE].hidden && foundGen)
        {
            resistance.text = "HIDDEN";
        }
        else
        {
            resistance.text = conponent.Values[ComponentParameter.RESISTANCE].value.ToString();
        }

        type.text = conponent.type.ToString();
        name.text = conponent.name;

        if (conponent.direction == Direction.A_to_B)
        {
            direction.sprite = downArrow;
        }
        else
        {
            direction.sprite = upArrow;
        }


        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        direction.transform.rotation = transform.parent.rotation;




    }


    public void display()
    {
        canvasGroup.alpha = 1f;

    }

    public void hide()
    {
        canvasGroup.alpha = 0f;
    }
}
