using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Utilities;

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
        if(circuitComponent.name == CircuitComponentName.Cell){
            foreach(Transform child in transform){
                try{
                    if(child.GetComponentInChildren<CircuitComponent>().name == CircuitComponentName.Cell){
                        Debug.Log("cell already exists");
                        return;
                    }
                    else{
                        break;

                    }
                }catch{
                    Debug.Log(child +" has no circuit component");
                }
                
            }

        }
                    circuitComponentToBuild = circuitComponent;
            Debug.Log("build "+ circuitComponent.name);
            CreateComponent();



    }


    private void CreateComponent(){
        GameObject circuitComponent = (GameObject)Instantiate(circuitComponentToBuild.prefab, new Vector2(10000,10000), Quaternion.identity,transform);
        circuitComponent.name = circuitComponentToBuild.name.ToString();
        if(circuitComponent.GetComponent<CircuitComponent>()){
            circuitComponent.GetComponent<GridMove>().MoveStart();
            circuitComponent.GetComponent<CircuitComponent>().name = circuitComponentToBuild.name;
        }else if(circuitComponent.GetComponent<Wire>()){
            circuitComponent.GetComponent<Wire>().createdFromButton();
        }


    }

}
