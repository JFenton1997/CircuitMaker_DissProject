using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager: MonoBehaviour
{
    public static BuildManager instance;
    private void Awake() {
        if(instance != null){
            Debug.LogError("More than one BuildManager is scene!");
            return;
        }
        instance=this;
        Debug.Log("Build Instance Created");
    }

    private CircuitComponentBlueprint circuitComponentToBuild; 
	public bool CanBuild { get { return circuitComponentToBuild != null; } }
    public void SelectCoponentToBuild(CircuitComponentBlueprint circuitComponent){
        circuitComponentToBuild = circuitComponent;
        CreateComponent();


    }


    public void TestActivation(string message){
        Debug.Log(message);
    }
    private void CreateComponent(){
        GameObject circuitComponent = (GameObject)Instantiate(circuitComponentToBuild.prefab, Input.mousePosition, Quaternion.identity);
        Debug.Log(circuitComponentToBuild.name);
        circuitComponent.GetComponent<ComponentMove>().MoveStart();
    }

}
