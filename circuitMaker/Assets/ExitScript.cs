using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitScript : MonoBehaviour
{
    private CanvasGroup canvas;
    public bool isShowing;

    private void Start() {
        canvas= GetComponent<CanvasGroup>();
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
        isShowing = false;
    }

    public void displayExitPanel(){
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;
        canvas.interactable = true;
         isShowing = true;
    }

    public void hideExitPanel(){
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
        isShowing = false;
    }

    public void backToMain(){
        Debug.Log("QUIT TO MAIN");
    }
    
}
