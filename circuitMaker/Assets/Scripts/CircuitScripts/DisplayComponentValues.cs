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
    private SolverScript solver;
    private GenerateCircuit foundGen; 
    public Color hiddenColor;
    bool checkIfAnswers; 
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

        if (transform.parent.GetComponent<CircuitComponent>().foundGen)
        {
            foundGen = transform.parent.GetComponent<CircuitComponent>().foundGen;
            viewer = transform.Find("/UI/ProblemDisplayer/ProblemView").GetComponent<ProblemViewer>();
            solver = transform.Find("/UI/SolverPanel").GetComponent<SolverScript>();
            Debug.Log("found solver" +solver);
            

        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if(solver) checkIfAnswers = solver.showAnswer;
        else checkIfAnswers = false;

        if(!component.Values[ComponentParameter.VOLTAGE].hidden || !foundGen) voltage.text = component.Values[ComponentParameter.VOLTAGE].value.ToString();
        else if (checkIfAnswers){
            voltage.text = component.Values[ComponentParameter.VOLTAGE].value.ToString();

        }
        else{ voltage.text = "?"; voltage.color = hiddenColor;};

        if(!component.Values[ComponentParameter.CURRENT].hidden || !foundGen) current.text = component.Values[ComponentParameter.CURRENT].value.ToString();
        else if (checkIfAnswers){
            current.text = component.Values[ComponentParameter.CURRENT].value.ToString();

        }
        else{ current.text = "?"; current.color = hiddenColor;};



        if(!component.Values[ComponentParameter.RESISTANCE].hidden || !foundGen) resistance.text = System.Math.Round(component.Values[ComponentParameter.RESISTANCE].value,2).ToString();
        else if (checkIfAnswers){
            resistance.text = component.Values[ComponentParameter.RESISTANCE].value.ToString();

        }
        else{ resistance.text = "?"; resistance.color = hiddenColor;};





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
