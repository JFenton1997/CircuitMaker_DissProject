using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
public class DisplayComponentValues : MonoBehaviour
{

    public Sprite upArrow, downArrow;
    private Text voltage, current, resistance, type, name;
    private Image direction;
    private DiagramComponent component;
    private CanvasGroup canvasGroup;
    private CircuitComponent circuitComponent;  

    private ProblemViewer viewer;
    private GenerateCircuit foundGen;  
    // Start is called before the first frame update
    void Start()
    {
        voltage = transform.Find("Voltage").GetComponent<Text>();
        current = transform.Find("Current").GetComponent<Text>();
        resistance = transform.Find("Resistance").GetComponent<Text>();
        type = transform.Find("Type").GetComponent<Text>();
        name = transform.Find("Name").GetComponent<Text>();
        direction =transform.Find("Direction").GetComponent<Image>();
        circuitComponent = transform.parent.GetComponent<CircuitComponent>();
        component = circuitComponent.component;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

                if (transform.parent.TryGetComponent<GenerateCircuit>(out GenerateCircuit gen))
        {
            foundGen = gen;
            viewer = transform.Find("/UI/ProblemDisplayer/ProblemView").GetComponent<ProblemViewer>();
        }

        
    }

    // Update is called once per frame
    void Update()
    {

        voltage.text = component.Values[ComponentParameter.VOLTAGE].value.ToString();
        current.text = component.Values[ComponentParameter.CURRENT].value.ToString();
        resistance.text = component.Values[ComponentParameter.RESISTANCE].value.ToString();
        type.text = component.type.ToString();
        name.text = component.name;

        if(component.direction == Direction.A_to_B){
            direction.sprite = downArrow;
        }
        else{
            direction.sprite = upArrow;
        }


        transform.rotation = Quaternion.Euler(0f,0f,0f);
        direction.transform.rotation = transform.parent.rotation;




    }


    public void display(){
        try{
        canvasGroup.alpha = 1f;
        }catch{}
        
    }

    public void hide(){
        canvasGroup.alpha = 0f;
    }
}
