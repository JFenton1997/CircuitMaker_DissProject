using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



using Utilities;


public class CircuitComponent : MonoBehaviour
{


    public DiagramComponent component;
    public Image directionImage;

    public Color normalColor = Color.black;
    public Color hiddenColor = Color.gray;

    [HideInInspector]
    public string name;

    public Node nodeA;
    public Node nodeB;

    public Canvas ValuesUX;
    private Text voltageText;
    private Text currentText;
    private Text resistanceText;
    private Text componentNameText;
    private Text componentTypeText;





    private void Awake()
    {
        try
        {
            nodeA = transform.GetChild(0).gameObject.GetComponent<Node>();
            nodeB = transform.GetChild(1).gameObject.GetComponent<Node>();




        }
        catch
        {
            Debug.LogError(this.name + " failed to find nodes");
        }

    }

    private void Start()
    {
        DiagramComponent component = new DiagramComponent();
        //Assigning Text Variables
        foreach (Text t in ValuesUX.GetComponentsInChildren<Text>())
        {
            if (t.transform.parent.name == "Voltage" && t.text == "0")
            {
                voltageText = t;
            }
            else if (t.transform.parent.name == "Current" && t.text == "0")
            {
                currentText = t;
            }
            else if (t.transform.parent.name == "Resistance" && t.text == "0")
            {
                resistanceText = t;
            }
            else if (t.name == "ComponentName")
            {
                componentNameText = t;
            }
            else if (t.name == "ComponentType")
            {

                componentTypeText = t;
                

            }



        }
        updateUXValues();

    }


    public void updateUXValues()
    {
        voltageText.text = component.Values[ComponentParameter.VOLTAGE].value.ToString();
        if (component.Values[ComponentParameter.VOLTAGE].hidden)
        {
            voltageText.color = hiddenColor;
        }
        else
        {
            voltageText.color = normalColor;
        }

        currentText.text = component.Values[ComponentParameter.CURRENT].value.ToString();
        if (component.Values[ComponentParameter.CURRENT].hidden)
        {
            currentText.color = hiddenColor;
        }
        else
        {
            currentText.color = normalColor;
        }

        resistanceText.text = component.Values[ComponentParameter.RESISTANCE].value.ToString();
        if (component.Values[ComponentParameter.RESISTANCE].hidden)
        {
            resistanceText.color = hiddenColor;
        }
        else
        {
            resistanceText.color = normalColor;
        }

        componentNameText.text = this.name;
        componentTypeText.text = component.type.ToString();
        component.name = this.name;
        updateDirection();
    }

    private void updateDirection()
    {
        if (component.direction == Direction.A_to_B)
        {
            directionImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            directionImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void OnMouseDown()
    {
        if (this.GetComponent<GridMove>().isMoving == false)
        {
            transform.GetComponentInParent<CircuitComponentPanel>().newComponentSelected(this);
        }

    }




}


