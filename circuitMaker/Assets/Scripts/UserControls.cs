using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControls : MonoBehaviour
{


    private void Start() {
        GlobalValues.AvowSnapping = false;
        GlobalValues.AvowSnappingOffset = 0.5f;
        GlobalValues.ToolTipsEnabled = true;
        GlobalValues.workingDirectory = "Assets/DiagramFiles/"; 
    }
    public KeyCode toggleSnap = KeyCode.LeftShift;
    public KeyCode plusOffset = KeyCode.N;
    public KeyCode minusOffset = KeyCode.M;
    public KeyCode toggleToolTips = KeyCode.T;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKey(toggleSnap) || Input.GetKeyDown(toggleSnap)){
              GlobalValues.AvowSnapping = true;
        }else{
              GlobalValues.AvowSnapping = false;
        }

        if(Input.GetKeyDown(plusOffset) && GlobalValues.AvowSnappingOffset <2.9f){
            
              GlobalValues.AvowSnappingOffset += 0.1f;
              Debug.Log("Snapping increassed to: " + GlobalValues.AvowSnappingOffset);
        }


        if(Input.GetKeyDown(minusOffset)&& GlobalValues.AvowSnappingOffset >0.1f){
              GlobalValues.AvowSnappingOffset -= 0.1f;
              Debug.Log("Snapping increassed to: " + GlobalValues.AvowSnappingOffset);
        }

        if(Input.GetKeyDown(toggleToolTips)){
            GlobalValues.ToolTipsEnabled = !GlobalValues.ToolTipsEnabled;
        }

        
    }
}
