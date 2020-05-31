using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitScript : MonoBehaviour
{
    /// <summary>
    /// UI class to control the exit window
    /// </summary>
    private CanvasGroup canvas;
    public bool isShowing;

/// <summary>
/// getting UI components
/// </summary>
    private void Start() {
        canvas= GetComponent<CanvasGroup>();
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
        isShowing = false;
    }

/// <summary>
/// disply Exit Panel to the user
/// </summary>
    public void displayExitPanel(){
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;
        canvas.interactable = true;
         isShowing = true;
    }

/// <summary>
/// hide exit panel to the user
/// </summary>
    public void hideExitPanel(){
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
        isShowing = false;
    }

/// <summary>
/// go back to the main menu
/// </summary>
    public void backToMain(){
        Debug.Log("QUIT TO MAIN");
    }
    
}
