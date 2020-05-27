using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;

public class SolverScript : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    public GameObject errorMessagePrefab;
    private Transform ErrorsDisplay;
    private List<Pair<GameObject, DiagramError>> ErrorMessages;

    private DiagramInstanceData question;
    private HashSet<DiagramError> foundErrors;
    public Text questionDesc; 



    // Start is called before the first frame update
    void Start()
    {
        foundErrors = new HashSet<DiagramError>();
        question = GlobalValues.selectedDiagram;
        if(question.diagramData == null){
            question = transform.Find("/ProgramMaster").GetComponent<CsvManager>().testRead();

        }  
        questionDesc.text = question.diagramQuestion;

        canvasGroup = GetComponent<CanvasGroup>();
        ErrorMessages = new List<Pair<GameObject, DiagramError>>();
        ErrorsDisplay = transform.Find("Image/Image/Panel/Scroll View/Viewport/ErrorsDisplay");
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

    }

    // Update is called once per frame
    public void displayErrors(HashSet<DiagramError> diagramErrors)
    {
        Debug.Log("displayErrors");
        clear();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.Find("Image/Image/Panel/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().value = 1f;
        if(diagramErrors.Count >0){
            foreach (DiagramError d in diagramErrors)
            {
                GameObject errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
                errorLog.transform.Find("ErrorName").GetComponent<Text>().text = d.errorName;
                errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = d.errorDiscription;
                ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, d));
            }
        }else{
            Debug.Log("correct");
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
                "conponents which are " + layers.Key + "away from the cell are in the problem:\n" +
                string.Join(" , ", solution[layers.Key].ConvertAll(x => x.name)) +
                "however you got:\n" + string.Join(" , ", layers.Value.ConvertAll(x => x.name))));
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



        // check all submitted values to ohms law
        foreach (DiagramComponent d in allComponents.FindAll(x => x.Values[ComponentParameter.VOLTAGE].value / x.Values[ComponentParameter.CURRENT].value
        != x.Values[ComponentParameter.RESISTANCE].value))
        {
            if (d.type != ComponentType.CELL && d.type != ComponentType.UNTYPED)
            {
                foundErrors.Add(new DiagramError("Component VALUE INCORRECT    ", "the values for " + d.name + " dont add up correctly to ohm's law, fix values or use auto feature", d, allComponents));
            }


        }

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
                  d.Values[ComponentParameter.VOLTAGE].value == x.Values[ComponentParameter.VOLTAGE].value &&
                   d.Values[ComponentParameter.CURRENT].value == x.Values[ComponentParameter.CURRENT].value &&
                    d.Values[ComponentParameter.RESISTANCE].value == x.Values[ComponentParameter.RESISTANCE].value && d.type == x.type).Count == 0)
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
                        if (!solution[layers.Key].Find(x => x.name == d.name).Aconnections.ConvertAll(x => x.name).Contains(s))
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
}
