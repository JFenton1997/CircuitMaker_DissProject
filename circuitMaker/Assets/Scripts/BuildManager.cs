using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BuildManager : MonoBehaviour
{
    char letter = 'A';
    private CircuitComponentBlueprint circuitComponentToBuild;

    public static BuildManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager is scene!");
            return;
        }
        instance = this;
        Debug.Log("Build Instance Created");
    }

    public void SelectCoponentToBuild(CircuitComponentBlueprint circuitComponent)
    {
        if (circuitComponent.type == ComponentType.CELL)
        {
            foreach (Transform child in transform)
            {
                try
                {
                    if (child.GetComponentInChildren<Component>().type == ComponentType.CELL)
                    {
                        Debug.Log("cell already exists");
                        return;
                    }
                    else
                    {
                        break;

                    }
                }
                catch
                {
                    Debug.Log(child + " has no circuit component");
                }

            }

        }
        circuitComponentToBuild = circuitComponent;
        Debug.Log("build " + circuitComponent.type);
        CreateComponent();



    }


    private void CreateComponent()
    {
        GameObject circuitComponent = (GameObject)Instantiate(circuitComponentToBuild.prefab, new Vector2(10000, 10000), Quaternion.identity, transform);
        if (circuitComponent.GetComponent<Component>())
        {
            circuitComponent.GetComponent<GridMove>().enabled = true;
            circuitComponent.GetComponent<GridMove>().MoveStart();
            circuitComponent.name = (letter.ToString());
            circuitComponent.GetComponent<Component>().type = circuitComponentToBuild.type;
            circuitComponent.GetComponent<CircuitComponent>().name = letter.ToString();
            letter++;
            if(circuitComponentToBuild.type == ComponentType.CELL){
                circuitComponent.GetComponent<Component>().direction = Direction.B_to_A;
            }else{
                circuitComponent.GetComponent<Component>().direction = Direction.A_to_B;
            }


        }
        else if (circuitComponent.GetComponent<Wire>())
        {
            circuitComponent.GetComponent<Wire>().createdFromButton();
        }


    }

}
