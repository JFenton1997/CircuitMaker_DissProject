using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class AvowManager : MonoBehaviour
{

    private List<AvowConponent> allAvow;
    public GameObject avowPrefab;
    public char currentName;


    // Start is called before the first frame update
    private void Awake() {
        allAvow = new List<AvowConponent>();
        currentName = 'A';
        GlobalValues.AvowScaler = 1f;
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform t in transform.GetComponentInChildren<Transform>()){
            if(t.parent == transform){
                if(!allAvow.Contains(t.GetComponent<AvowConponent>())){
                    allAvow.Add(t.GetComponent<AvowConponent>());
                }
            }
        }
        
    }

    public void updateConnections(){
        foreach( AvowConponent avow in allAvow){
            avow.updateConnections();
        }
    }

    public void buildAvow(){
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        GameObject avowObject = (GameObject) Instantiate(avowPrefab,new Vector3(curPosition.x, curPosition.y+6f, 0),Quaternion.identity,transform);
        AvowConponent avowConponent = avowObject.GetComponent<AvowConponent>();
        avowObject.name = "Avow" + currentName;
        avowConponent.component.type = ComponentType.RESISTOR;
        currentName++;





    }


}
