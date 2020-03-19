using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UxManager : MonoBehaviour
{
    public Canvas canvas;





    private void GridMoveStart(){
        disableCanvas();
    } 

    public void GridMoveEnded(){
        enableCanvas();
    }

    public void StartWireDraw(){
        disableCanvas();
    }

    public void EndWireDraw(){
        enableCanvas();
    }

    private void enableCanvas(){
        canvas.enabled = true;
    }

    private void disableCanvas(){
        canvas.enabled = false;
    }
}
