using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GenerateCircuit : MonoBehaviour
{
    public CircuitManager circuitTest;
    Dictionary<int, List<DiagramComponent>>  diagramData;
    private int VertComponentGap = 4, HorizontalComponentGap = 3;
    public Vector3 CellLocation, ComponentOrigin;
    public GameObject cellPrefab;
    public GameObject wirePrefab;
    public GameObject resistorPrefab;
    [SerializeField]
    public List<List<CircuitComponent>> generatorMatrix;
    public CircuitComponent cell;


    private void GenerateCircuitObject( Dictionary<int, List<DiagramComponent>> diagramData){
        
        this.diagramData = diagramData;
        initialiseMatrix();    
        generateValues();
        foreach(var layerComponents in diagramData){
            foreach(DiagramComponent diagramComponent in layerComponents.Value){
                createComponent(layerComponents.Key,diagramComponent);
            }
        }


    }

    private void createComponent(int layerValue, DiagramComponent d)
    {
        GameObject temp;
        if(layerValue == 0){
            temp = (GameObject)Instantiate(cellPrefab,CellLocation,Quaternion.identity,transform);
            

        }else{
            switch(d.type){
                case ComponentType.RESISTOR:
                    temp = (GameObject)Instantiate(resistorPrefab,generateLocation(layerValue),Quaternion.identity,transform);
                    break;
                default:
                    Debug.LogError("unknown");
                    temp = new GameObject("ERROR");
                    break;

            }

        }
        
        CircuitComponent circuitComponent = temp.GetComponent<CircuitComponent>();
        circuitComponent.component= d;
        circuitComponent.gameObject.name= d.name;
        circuitComponent.name = d.name;
        if(layerValue == 0){
            cell = circuitComponent;
        }else{
            generatorMatrix[layerValue-1].Add(circuitComponent);


        }


    }

    private void initialiseMatrix(){
        generatorMatrix = new List<List<CircuitComponent>>();
        for(int i=0; i<diagramData.Count-1;i++){
            generatorMatrix.Add(new List<CircuitComponent>());
        }
    }

    private Vector3 generateLocation(int layerValue){
        //x
        int x = Mathf.RoundToInt(ComponentOrigin.x + ((generatorMatrix[layerValue-1].Count)*HorizontalComponentGap));
        //y
        int y = Mathf.RoundToInt(ComponentOrigin.y-((layerValue-1)*VertComponentGap));
        return new Vector3Int(x,y,0);
    }

    private void generateValues(){
        int numberOfLayers = diagramData.Count;
        CellLocation = new Vector2(0,0);//-Mathf.RoundToInt((numberOfLayers-1)/2));
        ComponentOrigin = new Vector2(HorizontalComponentGap,0);
        




    }



    private void connectConponents(DiagramComponent a, DiagramComponent b){
        
    }


    public void runGeneration(){
        GenerateCircuitObject(circuitTest.diagramData);
    }
}
