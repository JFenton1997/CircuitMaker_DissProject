using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager: MonoBehaviour
{
    int index;

    public static BuildManager instance;
    private void Awake() {
        if(instance != null){
            Debug.LogError("More than one BuildManager is scene!");
            return;
        }
        instance=this;
        index = 0;
        Debug.Log("Build Instance Created");
    }

    private CircuitComponentBlueprint circuitComponentToBuild; 
    public void SelectCoponentToBuild(CircuitComponentBlueprint circuitComponent){
        circuitComponentToBuild = circuitComponent;
        CreateComponent();


    }


    private void CreateComponent(){
        GameObject circuitComponent = (GameObject)Instantiate(circuitComponentToBuild.prefab, new Vector2(10000,10000), Quaternion.identity,transform);
        circuitComponent.name = circuitComponentToBuild.name.ToString();
        if(circuitComponent.GetComponent<CircuitComponent>()){
            Debug.Log("circuit comp");
            circuitComponent.GetComponent<GridMove>().MoveStart();
        }else if(circuitComponent.GetComponent<Wire>()){
            Debug.Log("wire");
            circuitComponent.GetComponent<Wire>().createdFromButton();
        }


    }

}
