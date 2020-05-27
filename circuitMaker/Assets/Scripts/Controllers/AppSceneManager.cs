using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppSceneManager : MonoBehaviour
{
    private void Awake() {
              GlobalValues.getPlayerPrefs(); 
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


    
}
