using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using System;

public class SolverScript : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    public GameObject errorMessagePrefab;
    private Transform ErrorsDisplay;
    private List<Pair<GameObject, DiagramError>> ErrorMessages;

    private DiagramInstanceData question;
    private HashSet<DiagramError> foundErrors;
    public Text questionDesc;

    private Button FinishButton;
    private int attempts;
    public bool showAnswer, timerIsON;
    private float timer;
    public Color solvedColor, timerColor, attemptColor;


    // Start is called before the first frame update
    void Start()
    {
        FinishButton = transform.Find("Buttons/Finish").GetComponent<Button>();


        timerIsON = true;
        attempts = 0;

        timer = 0f;
        showAnswer = false;
        FinishButton.gameObject.SetActive(false);
        foundErrors = new HashSet<DiagramError>();
        question = GlobalValues.selectedDiagram;
        if (question.diagramData == null)
        {
            question = transform.Find("/ProgramMaster").GetComponent<CsvManager>().testRead();

        }
        questionDesc.text =  question.title+ " : " +question.diagramQuestion;

        canvasGroup = GetComponent<CanvasGroup>();
        ErrorMessages = new List<Pair<GameObject, DiagramError>>();
        ErrorsDisplay = transform.Find("Image/Image/Panel/Scroll View/Viewport/ErrorsDisplay");
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

    }

    private void Update()
    {
        if (timerIsON)
            timer += Time.deltaTime;
    }

    // Update is called once per frame
    public void displayErrors(HashSet<DiagramError> diagramErrors)
    {
        GameObject errorLog;
        clear();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.Find("Image/Image/Panel/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().value = 1f;
        
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");


        errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
        errorLog.transform.Find("ErrorName").GetComponent<Text>().text = "ATTEMPT NO";
        errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = attempts.ToString();
        errorLog.transform.Find("forground").GetComponent<Image>().color = attemptColor;
        ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, new DiagramError("","")));
        errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);


        errorLog.transform.Find("ErrorName").GetComponent<Text>().text = "TIME PASSED";
        errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = minutes + ":" + seconds;
        errorLog.transform.Find("forground").GetComponent<Image>().color = timerColor;
        ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, new DiagramError("","")));
    
        if (diagramErrors.Count > 0)
        {
            foreach (DiagramError d in diagramErrors)



            {
                errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
                errorLog.transform.Find("ErrorName").GetComponent<Text>().text = d.errorName;
                errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = d.errorDiscription;
                ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, d));
            }



        }
        else
        {
            timerIsON = false;

            errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
            errorLog.transform.Find("ErrorName").GetComponent<Text>().text = "CORRECT";
            errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = "you have successfully solved the question";
            errorLog.transform.Find("forground").GetComponent<Image>().color = solvedColor;
            ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, new DiagramError("","")));

            FinishButton.gameObject.SetActive(true);
        }

    }

    public void close()
    {
        clear();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

    }

    public void clear()
    {
        canvasGroup.interactable = false;
        foreach (Pair<GameObject, DiagramError> errorLog in ErrorMessages.ToArray())
        {
            GameObject.Destroy(errorLog.a);
            ErrorMessages.Remove(errorLog);


        }

    }

    public void compareAnswers(Dictionary<int, List<DiagramComponent>> diagram)
    {
        foreach (var data in diagram)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join(" \n",
data.Value
.ConvertAll(i => i.name.ToString() + "  " + i.Values[ComponentParameter.VOLTAGE].value + " "+   i.Values[ComponentParameter.CURRENT].value +"  "  +i.Values[ComponentParameter.RESISTANCE].value)
.ToArray()));





        }

                foreach (var data in question.diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join(" \n",
data.Value
.ConvertAll(i => i.name.ToString() + "  " + i.Values[ComponentParameter.VOLTAGE].value + " "+   i.Values[ComponentParameter.CURRENT].value +"  "  +i.Values[ComponentParameter.RESISTANCE].value)
.ToArray()));





        }







        attempts++;
        foundErrors.Clear();
        
        Dictionary<int, List<DiagramComponent>> solution = question.diagramData;
        List<DiagramComponent> allComponents = new List<DiagramComponent>();
        List<DiagramComponent> allSolutionComponents = new List<DiagramComponent>();
        foreach (var layers in solution)
        {
            foreach (DiagramComponent d in layers.Value)
            {
                allSolutionComponents.Add(d);
            }
        }


        foreach (var layers in diagram)
        {
            foreach (DiagramComponent d in layers.Value)
            {
                allComponents.Add(d);
            }
            // comparing layout
            if (layers.Value.Count != solution[layers.Key].Count)
            {
                foundErrors.Add(new DiagramError("LAYOUT INCORRECT    ", "the components dont follow the same layout , using max number of components away from the cell\n" +
                "components which are " + layers.Key + " away from the cell are:\n" +
                string.Join(" , ", solution[layers.Key].ConvertAll(x => x.name)) +
                "\n\nHowever you got:\n" + string.Join(" , ", layers.Value.ConvertAll(x => x.name))));
            }

        }



        foreach (DiagramComponent d in allComponents)
        {
            if (allSolutionComponents.Find(x => x.name == d.name) == null)
            {
                foundErrors.Add(new DiagramError("Component Name INCORRECT    ", "Component " + d.name + " dont match with any component in the problem", d, allComponents));

            }
        }

        foreach (DiagramComponent d in allSolutionComponents)
        {
            if (allComponents.Find(x => x.name == d.name) == null)
            {
                foundErrors.Add(new DiagramError("Component MISSING    ", "Component " + d.name + " is missing in your solution"));

            }
        }

        if (foundErrors.Count != 0)
        {
            displayErrors(foundErrors);
            return;
        }



        if (foundErrors.Count != 0)
        {
            displayErrors(foundErrors);
            return;
        }
        

        foreach(DiagramComponent d in allComponents){
            DiagramComponent dSolution = allSolutionComponents.Find(x=> x.name == d.name);
            string errorDesc ="";
            if(d.Values[ComponentParameter.VOLTAGE].value != dSolution.Values[ComponentParameter.VOLTAGE].value ){
                errorDesc +="\n Voltage is incorrect";
            }
            if(d.Values[ComponentParameter.CURRENT].value != dSolution.Values[ComponentParameter.CURRENT].value ){
                errorDesc +="\n current is incorrect";
            }
            if(d.Values[ComponentParameter.RESISTANCE].value != dSolution.Values[ComponentParameter.RESISTANCE].value ){
                errorDesc +="\n resistance is incorrect";
            }
            if((int)d.type != (int)dSolution.type){
                errorDesc += "\n type is incorrect";
            }
            foreach(string sComp in dSolution.Aconnections.ConvertAll(x=>x.name)){
                if(d.Aconnections.FindAll(x => x.name == sComp).Count != 1){
                    errorDesc +="\n Missing Input "+ sComp;
                }
            }
            foreach(string sComp in dSolution.Bconnections.ConvertAll(x=>x.name)){
                if(d.Bconnections.FindAll(x => x.name == sComp).Count != 1){
                    errorDesc +="\n Missing Output "+ sComp;
                }
            }
            if(errorDesc != ""){
                 foundErrors.Add(new DiagramError("COMPONENT "+d.name +" INCORRECT", "Found ERRORS:"+errorDesc,d,allComponents));    

            }



        }

       
            

     



            displayErrors(foundErrors);
            return;














        
    }

    public void showAnswers()
    {
        showAnswer = true;
        Debug.Log("SHOW ANSWER VALUE " + showAnswer);
        close();
    }
}
