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
    public Component currentlySelected;

    [Header("Parameters")]
    public InputField voltageText;
    public Text currentText;
    public Text resistanceText;

    [Header("DirectionSwitch")]
    public LeanSwitch directionSwitch;
    public Image left;
    public Image right;






    // Start is called before the first frame update
    private void Awake() {


        
    }

    public void newComponentSelected(Component component){
        window.transform.position = component.transform.position;
        this.currentlySelected = component;
    }


    // Update is called once per frame
    void Update()
    {
        if(currentlySelected){
            updateValues(); 
        }
       
    }

    public void updateValues(){
        voltageText.placeholder.GetComponent<Text>().text = currentlySelected.Values[ComponentParameter.VOLTAGE].value.ToString();

    }
}
