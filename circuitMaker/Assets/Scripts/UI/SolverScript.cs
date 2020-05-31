using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using System;
/// <summary>
/// UI and Functional Class User to compare answer with the question
/// </summary>
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
    private int attempts; //keeping track of number of submitted results
    public bool showAnswer, timerIsON; //keeping track if show answers or timer
    private float timer; //timer value
    public Color solvedColor, timerColor, attemptColor;


    /// <summary>
    /// getting UI Components, selectedDiagram from global values and diplaying question text to the user
    /// </summary>
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
        questionDesc.text = question.title + " : " + question.diagramQuestion;

        canvasGroup = GetComponent<CanvasGroup>();
        ErrorMessages = new List<Pair<GameObject, DiagramError>>();
        ErrorsDisplay = transform.Find("Image/Image/Panel/Scroll View/Viewport/ErrorsDisplay");
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

    }

/// <summary>
/// increment the timer
/// </summary>
    private void Update()
    {
        if (timerIsON)
            timer += Time.deltaTime;
    }

    /// <summary>
    /// called to display errors, and other information to user
    /// </summary>
    /// <param name="diagramErrors">list of errors found in solution ( can be empty if so problem be solver)</param>
    public void displayErrors(HashSet<DiagramError> diagramErrors)
    {
        GameObject errorLog;
        clear();//clear prev messages
        canvasGroup.alpha = 1f; //show window
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.Find("Image/Image/Panel/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().value = 1f;

        string minutes = Mathf.Floor(timer / 60).ToString("00"); // seting up timer displaye
        string seconds = (timer % 60).ToString("00");



        if (diagramErrors.Count > 0)
        {
            //if errors found, do for each error found
            foreach (DiagramError d in diagramErrors)



            {
                //displayer error information to the user (feedback)
                errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
                errorLog.transform.Find("ErrorName").GetComponent<Text>().text = d.errorName;
                errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = d.errorDiscription;
                ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, d));
            }



        }
        else //if no errors , solution is correct
        {
            timerIsON = false;//stop timer
            
            // if show answers was triggered, show message
            if (showAnswer)
            {
                errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
                errorLog.transform.Find("ErrorName").GetComponent<Text>().text = "ANSWERS SET TO SHOW";
                errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = "you solve correctly with the hidden values shown, try and work out where you struggled to calculate the correct value";
                errorLog.transform.Find("forground").GetComponent<Image>().color = attemptColor;
                ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, new DiagramError("", "")));
                errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);



            }
            //show solution been solved
            errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
            errorLog.transform.Find("ErrorName").GetComponent<Text>().text = "CORRECT";
            errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = "you have successfully solved the question";
            errorLog.transform.Find("forground").GetComponent<Image>().color = solvedColor;
            ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, new DiagramError("", "")));

            FinishButton.gameObject.SetActive(true);
        }

        //show attempts done, (attempt is number times submitted)
        errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
        errorLog.transform.Find("ErrorName").GetComponent<Text>().text = "ATTEMPT NO";
        errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = attempts.ToString();
        errorLog.transform.Find("forground").GetComponent<Image>().color = attemptColor;
        ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, new DiagramError("", "")));
        errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);

        //shwoing time passed, gives a sense of acheivement
        errorLog.transform.Find("ErrorName").GetComponent<Text>().text = "TIME PASSED";
        errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = minutes + ":" + seconds;
        errorLog.transform.Find("forground").GetComponent<Image>().color = timerColor;
        ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, new DiagramError("", "")));

    }

/// <summary>
/// close solver window
/// </summary>
    public void close()
    {
        clear();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

    }

/// <summary>
/// clears all messages 
/// </summary>
    public void clear()
    {
        canvasGroup.interactable = false;
        foreach (Pair<GameObject, DiagramError> errorLog in ErrorMessages.ToArray())
        {
            GameObject.Destroy(errorLog.a);
            ErrorMessages.Remove(errorLog);


        }

    }

/// <summary>
/// method used to compare answer with original diagram
/// </summary>
/// <param name="diagram">submitted answer from user</param>
    public void compareAnswers(Dictionary<int, List<DiagramComponent>> diagram)
    {
        //for debugging
        foreach (var data in diagram)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join(" \n",
data.Value
.ConvertAll(i => i.name.ToString() + "  " + i.Values[ComponentParameter.VOLTAGE].value + " " + i.Values[ComponentParameter.CURRENT].value + "  " + i.Values[ComponentParameter.RESISTANCE].value)
.ToArray()));





        }
    // for debugging
        foreach (var data in question.diagramData)
        {
            Debug.Log("layer" + data.Key.ToString() + " = " + String.Join(" \n",
data.Value
.ConvertAll(i => i.name.ToString() + "  " + i.Values[ComponentParameter.VOLTAGE].value + " " + i.Values[ComponentParameter.CURRENT].value + "  " + i.Values[ComponentParameter.RESISTANCE].value)
.ToArray()));





        }






        //increment attempts
        attempts++;
        foundErrors.Clear();

        Dictionary<int, List<DiagramComponent>> solution = question.diagramData;
        List<DiagramComponent> allComponents = new List<DiagramComponent>();
        List<DiagramComponent> allSolutionComponents = new List<DiagramComponent>();
        //get all components in both received answer and solution
        foreach (var layers in solution)
        {
            foreach (DiagramComponent d in layers.Value)
            {
                allSolutionComponents.Add(d);
            }
        }

        //for each layer in the attempted answer
        foreach (var layers in diagram)
        {
            //for each diagram component
            foreach (DiagramComponent d in layers.Value)
            {
                allComponents.Add(d);
            }
            // comparing layout out the submitted answer
            if (layers.Value.Count != solution[layers.Key].Count)
            {
                foundErrors.Add(new DiagramError("LAYOUT INCORRECT    ", "the components dont follow the same layout , using max number of components away from the cell\n" +
                "components which are " + layers.Key + " away from the cell are:\n" +
                string.Join(" , ", solution[layers.Key].ConvertAll(x => x.name)) +
                "\n\nHowever you got:\n" + string.Join(" , ", layers.Value.ConvertAll(x => x.name))));
            }

        }


        // checking for names to match up in both solution to submitted and vis versa
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

    //if errors found so far, display to user
        if (foundErrors.Count != 0)
        {
            displayErrors(foundErrors);
            return;
        }

        //comparing each component in solution to submitted answer, generating a description of whats wrong in a single string
        foreach (DiagramComponent d in allComponents)
        {
            DiagramComponent dSolution = allSolutionComponents.Find(x => x.name == d.name);
            string errorDesc = "";
            if (d.Values[ComponentParameter.VOLTAGE].value != dSolution.Values[ComponentParameter.VOLTAGE].value)
            {
                errorDesc += "\n Voltage is incorrect";
            }
            if (d.Values[ComponentParameter.CURRENT].value != dSolution.Values[ComponentParameter.CURRENT].value)
            {
                errorDesc += "\n current is incorrect";
            }
            if (d.Values[ComponentParameter.RESISTANCE].value != dSolution.Values[ComponentParameter.RESISTANCE].value)
            {
                errorDesc += "\n resistance is incorrect";
            }
            if ((int)d.type != (int)dSolution.type)
            {
                errorDesc += "\n type is incorrect";
            }
            foreach (string sComp in dSolution.Aconnections.ConvertAll(x => x.name))
            {
                if (d.Aconnections.FindAll(x => x.name == sComp).Count != 1)
                {
                    errorDesc += "\n Missing Input " + sComp;
                }
            }
            foreach (string sComp in dSolution.Bconnections.ConvertAll(x => x.name))
            {
                if (d.Bconnections.FindAll(x => x.name == sComp).Count != 1)
                {
                    errorDesc += "\n Missing Output " + sComp;
                }
            }
            if (errorDesc != "")
            {
                foundErrors.Add(new DiagramError("COMPONENT " + d.name + " INCORRECT", "Found ERRORS:" + errorDesc, d, allComponents));

            }



        }

    
        displayErrors(foundErrors);
        return;

    }

/// <summary>
/// show all hidden values to the user (for people who need help)
/// </summary>
    public void showAnswers()
    {
        showAnswer = true;
        Debug.Log("SHOW ANSWER VALUE " + showAnswer);
        close();
    }
}
