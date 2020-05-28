﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class ProblemViewer : MonoBehaviour
{
    Camera problemCam;
    RawImage viewImage;

    [SerializeField]
    public RenderTexture renderTarget;
    private Text scaleText;
    RectTransform rectTransform;
    public float panSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minY = 2f;
    public float maxY = 20f;
    public float sizeChangeSpeed = 10f;

    private Vector3 defaultPos;
    private float defualZoom;

    int DisplayW, DisplayH;
    public bool displayValues = true;
    // Start is called before the first frame update
    void Start()
    {

        DisplayH = Screen.height;
        DisplayW = Screen.width;
        scaleText = transform.Find("Buttons/Scale/ScaleText").GetComponent<Text>();
       scaleText.transform.parent.gameObject.SetActive(false);
        problemCam = transform.Find("/ProblemCamera").GetComponent<Camera>();
        defaultPos = problemCam.transform.position;
        defualZoom = problemCam.orthographicSize;
        viewImage = transform.Find("Display").GetComponent<RawImage>();
        if (problemCam.activeTexture == null)
        {
            problemCam.targetTexture = new RenderTexture( DisplayW,DisplayH, 24);
        }
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(DisplayW / 7 , DisplayH / 7);


        var renderer = GetComponent<Renderer>();





    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("PROBLEM V:" + displayValues);


        viewImage.texture = problemCam.activeTexture;



    }

    public void toggleMinimise(){
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void toggleDetails(){
        displayValues = !displayValues;

    }

    public void showScaleText(float scale){
        scaleText.text = scale.ToString() + " : 1";
        scaleText.transform.parent.gameObject.SetActive(true);
    }

    public void sizeChange(int value){
        switch (value){
            case 0:
             rectTransform.sizeDelta = new Vector2(DisplayW / 12, DisplayH / 12);
            break;
            case 1:
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - sizeChangeSpeed , rectTransform.sizeDelta.y - sizeChangeSpeed);
            break;
            case 2:
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + sizeChangeSpeed , rectTransform.sizeDelta.y + sizeChangeSpeed);
            break;


        }
            
            

        
        Vector2 SizeCheck = new Vector2();
        SizeCheck.x= Mathf.Clamp(rectTransform.sizeDelta.x, DisplayW/60, DisplayW/5);
        SizeCheck.y= Mathf.Clamp(rectTransform.sizeDelta.y, DisplayH/60, DisplayH/5);
        rectTransform.sizeDelta = SizeCheck;
    }



    public void MoveCamera(int Direction)
    {
        switch (Direction)
        {
            case 0:
                problemCam.transform.Translate(Vector3.up * panSpeed * Time.deltaTime, Space.World);
                break;
            case 1:
                problemCam.transform.Translate(Vector3.down * panSpeed * Time.deltaTime, Space.World);
                break;

            case 2:
                problemCam.transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
                break;

            case 3:
                problemCam.transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
                break;

            case 4:
                problemCam.orthographicSize += zoomSpeed * Time.deltaTime;
                problemCam.orthographicSize = Mathf.Clamp(problemCam.orthographicSize, minY, maxY);
                break;
             case 5:
                problemCam.orthographicSize -= zoomSpeed * Time.deltaTime;
                problemCam.orthographicSize = Mathf.Clamp(problemCam.orthographicSize, minY, maxY);
                break;
            case 6:
                problemCam.orthographicSize = defualZoom;
                problemCam.transform.position = defaultPos;
                break;










        }



    }
}
