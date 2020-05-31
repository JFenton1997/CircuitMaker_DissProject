using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
/// <summary>
/// UI class used to display errors to the user
/// </summary>
public class ErrorPanel : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    public GameObject errorMessagePrefab; //error message prefab
    private Transform ErrorsDisplay; //the display window
    private List<Pair<GameObject, DiagramError>> ErrorMessages; //list of error messages and their objects


    /// <summary>
    /// get UI elements
    /// </summary>
    void Start()
    {
        
        canvasGroup = GetComponent<CanvasGroup>();
        ErrorMessages = new List<Pair<GameObject, DiagramError>>();
        ErrorsDisplay = transform.Find("Image/Image/Panel/Scroll View/Viewport/ErrorsDisplay");
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

    }

    /// <summary>
    /// show window and display all errors received in a error window viewport
    /// </summary>
    /// <param name="diagramErrors"> unique list of error message to be displayed to user </param>
    public void displayErrors(HashSet<DiagramError> diagramErrors)
    {
        Debug.Log("displayErrors");
        clear();
        canvasGroup.alpha = 1f; //make window visible and interactable
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        //set scrollbar  to be at the top
        transform.Find("Image/Image/Panel/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().value = 1f;
        
        // for each diagramError, create a errorlog to be displayed to the user
        foreach (DiagramError d in diagramErrors)
        {
            GameObject errorLog = (GameObject)Instantiate(errorMessagePrefab, ErrorsDisplay.position, Quaternion.identity, ErrorsDisplay);
            errorLog.transform.Find("ErrorName").GetComponent<Text>().text = d.errorName;
            errorLog.transform.Find("ErrorDesc").GetComponent<Text>().text = d.errorDiscription;
            ErrorMessages.Add(new Pair<GameObject, DiagramError>(errorLog, d));
        }

    }

/// <summary>
/// closes error window
/// </summary>
    public void close()
    {
        clear();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

    }

/// <summary>
/// clears error windows messages, used to prevent old message being shown
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
}
