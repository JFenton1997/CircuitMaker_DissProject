using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppSceneManager : MonoBehaviour
{
    private void Awake() {
              GlobalValues.getPlayerPrefs(); 
    }

    private void Update() {
        try{
            transform.Find("/Canvas/Text").GetComponent<Text>().text = GlobalValues.workingDirectory;
        }catch{}
    }

    public void loadScene(int sceneInt){
        try{
            GlobalValues.updatePlayerPrefs();
            SceneManager.LoadScene (sceneInt);
        }catch(System.Exception e){
            Debug.LogError(e);
        }
    }

    public void exitApp(){
        Application.Quit();
    }

    public void wipePlayerPref(){
        GlobalValues.clearPlayerPrefs();
    }


    
}
