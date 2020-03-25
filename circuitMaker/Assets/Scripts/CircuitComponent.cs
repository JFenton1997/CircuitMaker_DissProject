using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



using Utilities;


public class CircuitComponent : MonoBehaviour
{

    
    public Component component;
    public Image directionImage;

    [HideInInspector]
    public string name;

    private Node nodeA;
    private Node nodeB;

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
            component = transform.GetComponent<Component>();




        }
        catch
        {
            Debug.LogError(this.name + " failed to find nodes");
        }

    }
    
    private void Start() {
        //Assigning Text Variables
         foreach(Text t in ValuesUX.GetComponentsInChildren<Text>()){
                if(t.transform.parent.name == "Voltage" && t.text == "0"){
                    voltageText = t;
                }
                else if(t.transform.parent.name == "Current" && t.text == "0"){
                    currentText = t;
                }
                else if(t.transform.parent.name == "Resistance" && t.text == "0"){
                    resistanceText = t;
                }
                else if(t.name == "ComponentName"){
                    componentNameText = t;
                }
                else if(t.name == "ComponentType"){
                    componentTypeText = t;
                    componentTypeText.text = component.type.ToString();

                }

                

            }
            updateUXValues();

    }


    public void updateUXValues(){
        voltageText.text = component.Values[ComponentParameter.VOLTAGE].value.ToString();
        currentText.text = component.Values[ComponentParameter.CURRENT].value.ToString();
        resistanceText.text = component.Values[ComponentParameter.RESISTANCE].value.ToString();
        componentNameText.text = this.name;
        updateDirection();
    }

    private void updateDirection(){
        if(component.direction == Direction.A_to_B){
                directionImage.rectTransform.rotation = Quaternion.Euler(0f,0f,180f);
            }else{
                directionImage.rectTransform.rotation = Quaternion.Euler(0f,0f,0f);
            }
    }

    private void OnMouseDown() {
        if(this.GetComponent<GridMove>().isMoving == false){
            this.GetComponentInParent<CircuitComponentPanel>().newComponentSelected(component);
        }
        
    }
}


