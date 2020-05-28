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
            errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = "you have successfully the question";
            errorLog.transform.Find("forground").GetComponent<Image>().color = solvedColor;

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
                foundErrors.Add(new DiagramError("LAYOUT INCORRECT    ", "the component dont follow the same layout , using max number of component away from the cell\n" +
                "components which are " + layers.Key + "away from the cell are in the problem:\n" +
                string.Join(" , ", solution[layers.Key].ConvertAll(x => x.name)) +
                "\nhowever you got:\n" + string.Join(" , ", layers.Value.ConvertAll(x => x.name))));
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



        // // check all submitted values to ohms law
        // foreach (DiagramComponent c in allComponents.FindAll(x => Math.Round(x.Values[ComponentParameter.VOLTAGE].value / x.Values[ComponentParameter.CURRENT].value
        // , 2) != Math.Round(x.Values[ComponentParameter.RESISTANCE].value, 2)))
        // {

        //     if (c.type != ComponentType.CELL && c.type != ComponentType.UNTYPED)
        //     {
        //         Debug.Log(Math.Round(c.Values[ComponentParameter.VOLTAGE].value / c.Values[ComponentParameter.CURRENT].value
        // , 2));

        //         foundErrors.Add(new DiagramError("component VALUE ERROR    ", "the values for " + c.name + " dont add up correctly to ohm's law, fix values or use auto feature", c, allComponents));
        //     }
        // }


        if (foundErrors.Count != 0)
        {
            displayErrors(foundErrors);
            return;
        }

        // compare Values
        foreach (var layers in diagram)
        {
            foreach (DiagramComponent d in layers.Value)
            {
                if (solution[layers.Key].FindAll(x => x.name == d.name &&
                  Math.Round(d.Values[ComponentParameter.VOLTAGE].value, 2) == Math.Round(x.Values[ComponentParameter.VOLTAGE].value, 2) &&
                   Math.Round(d.Values[ComponentParameter.CURRENT].value, 2) == Math.Round(x.Values[ComponentParameter.CURRENT].value, 2) &&
                    Math.Round(d.Values[ComponentParameter.RESISTANCE].value, 2) == Math.Round(x.Values[ComponentParameter.RESISTANCE].value, 2) && d.type == x.type).Count == 0)
                {
                    foundErrors.Add(new DiagramError("Component Is INCORRECT    ", "the values for " + d.name + " correlate with the question", d, allComponents));
                }

                if (solution[layers.Key].Find(x => x.name == d.name) != null)
                {
                    foreach (string s in d.Aconnections.ConvertAll(x => x.name))
                    {
                        if (!solution[layers.Key].Find(x => x.name == d.name).Aconnections.ConvertAll(x => x.name).Contains(s))
                        {
                            foundErrors.Add(new DiagramError("Component Has Incorrect inputs ", " component "
                            + d.name + " is missing a input ", d, allComponents));

                        }

                    }

                }
                if (solution[layers.Key].Find(x => x.name == d.name) != null)
                {
                    foreach (string s in d.Bconnections.ConvertAll(x => x.name))
                    {
                        if (!solution[layers.Key].Find(x => x.name == d.name).Bconnections.ConvertAll(x => x.name).Contains(s))
                        {
                            foundErrors.Add(new DiagramError("Component Has Incorrect Outputs ", " component "
                            + d.name + " is missing a Output ", d, allComponents));

                        }

                    }

                }
            }


            displayErrors(foundErrors);
            return;














        }
    }

    public void showAnswers()
    {
        showAnswer = true;
        Debug.Log("SHOW ANSWER VALUE " + showAnswer);
        close();
    }
}
