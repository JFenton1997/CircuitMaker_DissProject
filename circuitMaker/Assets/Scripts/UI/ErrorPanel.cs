using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;

public class ErrorPanel : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    public GameObject errorMessagePrefab;
    private Transform ErrorsDisplay;
    private List<Pair<GameObject, DiagramError>> ErrorMessages;


    // Start is called before the first frame update
    void Start()
    {
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
        
        foreach (DiagramError d in diagramErrors)
        {
            GameObject errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
            errorLog.transform.Find("ErrorName").GetComponent<Text>().text = d.errorName;
            errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = d.errorDiscription;
            ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, d));
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
}
