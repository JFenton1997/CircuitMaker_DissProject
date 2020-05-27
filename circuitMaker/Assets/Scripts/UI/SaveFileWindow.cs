using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class SaveFileWindow : MonoBehaviour
{

    InputField titleField, authorField, questionField;
    Toggle circuitToCiruitToggle, circuitToAvowToggle, avowToCircuitToggle, avowToAvowToggle;
    CanvasGroup canvasGroup;
    public Color errorColor; 
    Color fieldColor;
    Dictionary<int, List<DiagramComponent>> diagramData;

    // Start is called before the first frame update
    private void Start()
    {
        titleField = transform.Find("diagramName").GetComponent<InputField>();
        authorField = transform.Find("Author").GetComponent<InputField>();
        questionField = transform.Find("Question").GetComponent<InputField>();

        circuitToCiruitToggle = transform.Find("CircuitToCircuit").GetComponent<Toggle>();
        circuitToAvowToggle = transform.Find("CircuitToAvow").GetComponent<Toggle>();
        avowToCircuitToggle = transform.Find("AvowToCircuit").GetComponent<Toggle>();
        avowToAvowToggle = transform.Find("AvowToAvow").GetComponent<Toggle>();

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0f;
        fieldColor = titleField.GetComponent<Image>().color;
    }

    // Update is called once per frame
    public void intialiseSaveWindow(Dictionary<int, List<DiagramComponent>> diagramData)
    {
        this.diagramData = diagramData;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;
        circuitToAvowToggle.isOn= true;
        avowToAvowToggle.isOn = true;
        circuitToCiruitToggle.isOn= true;
        avowToCircuitToggle.isOn = true;

    }

    public void saveFile(){
        if(titleField.text == ""){
            titleField.GetComponent<Image>().color = errorColor;
        }
        if(authorField.text == ""){
            authorField.GetComponent<Image>().color = errorColor;

        }
        if(questionField.text == ""){
            questionField.GetComponent<Image>().color = errorColor;

        }
        if(!circuitToAvowToggle.isOn && !circuitToCiruitToggle.isOn && !avowToAvowToggle.isOn && !avowToCircuitToggle){
            circuitToAvowToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;
            circuitToCiruitToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;
            avowToCircuitToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;
            avowToAvowToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;

        }
        else{
            bool successfulSave;
            DiagramInstanceData diagramToSave =  new DiagramInstanceData(titleField.text,
            authorField.text,questionField.text
            ,new bool[]{circuitToCiruitToggle.isOn, circuitToAvowToggle.isOn, avowToCircuitToggle.isOn, avowToAvowToggle.isOn}
            ,diagramData);
            successfulSave = transform.Find("/ProgramMaster").GetComponent<CsvManager>().writeDataToCsv(diagramToSave);
            if(successfulSave){
                 transform.Find("/ProgramMaster").GetComponent<AppSceneManager>().loadScene(0);
            }
            else{
                Debug.LogError("FAILED TO SAVE FILE");
            }
            cancel();
            //CLOSE WINDOW
        }
    }

    public void selectedColor(){
        titleField.GetComponent<Image>().color = fieldColor;
        authorField.GetComponent<Image>().color = fieldColor;
        questionField.GetComponent<Image>().color = fieldColor;
        circuitToAvowToggle.transform.Find("Background").GetComponent<Image>().color = fieldColor;
        circuitToCiruitToggle.transform.Find("Background").GetComponent<Image>().color = fieldColor;
        avowToCircuitToggle.transform.Find("Background").GetComponent<Image>().color = fieldColor;
        avowToAvowToggle.transform.Find("Background").GetComponent<Image>().color = fieldColor;
    }

    public void cancel(){
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0f;
    }

}
