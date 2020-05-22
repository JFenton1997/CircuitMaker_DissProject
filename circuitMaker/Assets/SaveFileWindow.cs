using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class SaveFileWindow : MonoBehaviour
{

    InputField titleField, authorField, questionField;
    Toggle circuitToggle, AvowToggle;
    CanvasGroup canvasGroup;
    public Color errorColor; 
    Color fieldColor;
    Dictionary<int, List<DiagramComponent>> diagramData;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("i");
        titleField = transform.Find("diagramName").GetComponent<InputField>();
        authorField = transform.Find("Author").GetComponent<InputField>();
        questionField = transform.Find("Question").GetComponent<InputField>();
        circuitToggle = transform.Find("Circuit").GetComponent<Toggle>();
        AvowToggle = transform.Find("Avow").GetComponent<Toggle>();
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
        Debug.Log("hello");
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;
        circuitToggle.isOn= true;
        AvowToggle.isOn = true;

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
        if(!circuitToggle.isOn && !AvowToggle.isOn){
            circuitToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;
            AvowToggle.transform.Find("Background").GetComponent<Image>().color = errorColor;

        }
        else{
            bool successfulSave;
            DiagramInstanceData diagramToSave =  new DiagramInstanceData(titleField.text,
            authorField.text,questionField.text,new Pair<bool, bool>(circuitToggle.isOn,AvowToggle.isOn),diagramData);
            successfulSave = transform.Find("/ProgramMaster").GetComponent<CsvManager>().writeDataToCsv(diagramToSave);
            Debug.Log("SAVE SUCCESSFUL: " + successfulSave);
            cancel();
            //CLOSE WINDOW
        }
    }

    public void selectedColor(){
        titleField.GetComponent<Image>().color = fieldColor;
        authorField.GetComponent<Image>().color = fieldColor;
        questionField.GetComponent<Image>().color = fieldColor;
        circuitToggle.transform.Find("Background").GetComponent<Image>().color = fieldColor;
        AvowToggle.transform.Find("Background").GetComponent<Image>().color = fieldColor;
    }

    public void cancel(){
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0f;
    }

}
