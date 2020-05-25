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
    private CanvasGroup canvasGroup;
    
    // Start is called before the first frame update
    void Start()
    {
        voltage = transform.Find("Voltage").GetComponent<Text>();
        current = transform.Find("Current").GetComponent<Text>();
        resistance = transform.Find("Resistance").GetComponent<Text>();
        type = transform.Find("Type").GetComponent<Text>();
        name = transform.Find("Name").GetComponent<Text>();

        direction =transform.Find("Direction").GetComponent<Image>();
        conponent = transform.parent.GetComponent<CircuitComponent>().conponent;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        
    }

    // Update is called once per frame
    void Update()
    {
        voltage.text = conponent.Values[ComponentParameter.VOLTAGE].value.ToString();
        current.text = conponent.Values[ComponentParameter.CURRENT].value.ToString();
        resistance.text = conponent.Values[ComponentParameter.RESISTANCE].value.ToString();
        type.text = conponent.type.ToString();
        name.text = conponent.name;

        if(conponent.direction == Direction.A_to_B){
            direction.sprite = downArrow;
        }
        else{
            direction.sprite = upArrow;
        }


        transform.rotation = Quaternion.Euler(0f,0f,0f);
        direction.transform.rotation = transform.parent.rotation;




    }


    public void display(){
        canvasGroup.alpha = 1f;
        
    }

    public void hide(){
        canvasGroup.alpha = 0f;
    }
}
