using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
/// <summary>
/// UI class used to get additional information from the user to create a diagramInstanceData to be saved into a csv
/// </summary>
public class SaveFileWindow : MonoBehaviour
{

    InputField titleField, authorField, questionField;
    Toggle circuitToCiruitToggle, circuitToAvowToggle, avowToCircuitToggle, avowToAvowToggle;
    CanvasGroup canvasGroup;
    public Color errorColor; 
    Color fieldColor;
    Dictionary<int, List<DiagramComponent>> diagramData;
    float scale;

    /// <summary>
    /// get UI components and hide self
    /// </summary>
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

    /// <summary>
    /// show window and get diagram details
    /// </summary>
    /// <param name="diagramData">diagramData created from a diagram manager</param>
    /// <param name="scale">scale value calculated or set by diagram manager</param>
    public void intialiseSaveWindow(Dictionary<int, List<DiagramComponent>> diagramData, float scale)
    {
        this.diagramData = diagramData;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;
        circuitToAvowToggle.isOn= true;
        avowToAvowToggle.isOn = true;
        circuitToCiruitToggle.isOn= true;
        avowToCircuitToggle.isOn = true;
        this.scale = scale;
        if(GlobalValues.authorName!=""){ // if prev author name been used preset author to this
            authorField.text = GlobalValues.authorName;
        }

    }

/// <summary>
/// if all filed been filled correctly, save diagramInstanceData to csv
/// if successful load main menu
/// else do nothing
/// </summary>
    public void saveFile(){
        //if field left empty, go red to show user the error
        if(titleField.text == ""){ 
            titleField.GetComponent<Image>().color = errorColor;
        }
        else if(authorField.text == ""){
            authorField.GetComponent<Image>().color = errorColor;

        }
        else if(questionField.text == ""){
            questionField.GetComponent<Image>().color = errorColor;

        }//if file isnt of any type of problem
        else if(!circuitToAvowToggle.isOn && !circuitToCiruitToggle.isOn && !avowToAvowToggle.isOn && !avowToCircuitToggle){
            circuitToAvowToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;
            circuitToCiruitToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;
            avowToCircuitToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;
            avowToAvowToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;

        }
        else{//if all fields filled, get values and sent to csv manager to be saved
            bool successfulSave;
            DiagramInstanceData diagramToSave =  new DiagramInstanceData(titleField.text,
            authorField.text,questionField.text
            ,new bool[]{circuitToCiruitToggle.isOn, circuitToAvowToggle.isOn, avowToCircuitToggle.isOn, avowToAvowToggle.isOn}
            ,scale,diagramData);
            successfulSave = transform.Find("/ProgramMaster").GetComponent<CsvManager>().writeDataToCsv(diagramToSave);
            if(successfulSave){
                GlobalValues.authorName = authorField.text;
                 transform.Find("/ProgramMaster").GetComponent<AppSceneManager>().loadScene(0); //go back to main menu to successful
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
