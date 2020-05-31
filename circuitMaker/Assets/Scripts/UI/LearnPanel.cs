using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI classed used to navigate learnPanel
/// </summary>
public class LearnPanel : MonoBehaviour
{
    private GameObject howToPage; //how to page
    private GameObject CircuitBuilder; //circuit builder page
    private GameObject AvowBuilder; // avowbuilder page
    private CanvasGroup canvasGroup;// canvasgroup 
    public bool window;// if this should act as a window on another scene or has it own scene

    /// <summary>
    /// gets UI components and makes self invisible depending on is window
    /// </summary>
    void Start()
    {
        howToPage = transform.Find("MainPanel/How To Page").gameObject;
        CircuitBuilder = transform.Find("MainPanel/CircuitBuilder").gameObject;
        AvowBuilder = transform.Find("MainPanel/AvowBuilder").gameObject;
        canvasGroup = GetComponent<CanvasGroup>();
        howToPage.SetActive(false);
        CircuitBuilder.SetActive(false);
        AvowBuilder.SetActive(false);
        if(window){
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            
        }else{
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

        }
        
    }

/// <summary>
/// display learn window
/// </summary>
    public void displayWindow(){
                    canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
    }

/// <summary>
/// hide learn window
/// </summary>
        public void HideWindow(){
                    canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
    }


    
  /// <summary>
  /// show a learn page based of a given int
  /// </summary>
  /// <param name="i"> 0 : how to use , 1: circuit Builder Info, 2: avow Builder Info</param>  
    public void LearnPageSelected(int i){
        switch(i){
            case 0:
                //how to use
                howToPage.SetActive(true);
                break;
            case 1:
                //Circuit Builder Info
                CircuitBuilder.SetActive(true);

                break;
            case 2:
                //Avow Builder Info
                AvowBuilder.SetActive(true);
                break;
            default:
                Debug.LogError("INVALID SELECTION");
                break;





        }
    }

/// <summary>
/// back to window main menu
/// </summary>
    public void back(){
         howToPage.SetActive(false);
        CircuitBuilder.SetActive(false);
        AvowBuilder.SetActive(false);

    }
}
