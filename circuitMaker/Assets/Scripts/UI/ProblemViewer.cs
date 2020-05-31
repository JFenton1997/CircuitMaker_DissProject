using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/// <summary>
/// UI class used to show the problem to solve to the user
/// </summary>
public class ProblemViewer : MonoBehaviour
{
    Camera problemCam; //problem cam
    RawImage viewImage; //raw feed of camera view

    [SerializeField]
    public RenderTexture renderTarget; //target to save image feed
    private Text scaleText; // show scale to user
    RectTransform rectTransform;
    public float panSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minY = 2f;
    public float maxY = 20f;
    public float sizeChangeSpeed = 10f;

    private Vector3 defaultPos;
    private float defaultZoom;

    int DisplayW, DisplayH;
    public bool displayValues = true;
    

    /// <summary>
    /// Getting UI components
    /// </summary>
    void Start()
    {

        DisplayH = Screen.height;
        DisplayW = Screen.width;
        scaleText = transform.Find("Buttons/Scale/ScaleText").GetComponent<Text>();
       scaleText.transform.parent.gameObject.SetActive(false);
        problemCam = transform.Find("/ProblemCamera").GetComponent<Camera>();
        defaultPos = problemCam.transform.position;
        defaultZoom = problemCam.orthographicSize;
        viewImage = transform.Find("Display").GetComponent<RawImage>(); //creating a raw image if not existing
        if (problemCam.activeTexture == null)
        {
            problemCam.targetTexture = new RenderTexture( DisplayW,DisplayH, 24); //use size of screen to set size
        }
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(DisplayW / 7 , DisplayH / 7);


        var renderer = GetComponent<Renderer>();





    }

    /// <summary>
    /// update viewImage texture to cameras feed
    /// </summary>
    void Update()
    {


        viewImage.texture = problemCam.activeTexture;



    }

    /// <summary>
    /// minimised problem viewer
    /// </summary>
    public void toggleMinimise(){
        gameObject.SetActive(!gameObject.activeSelf);
    }

    /// <summary>
    /// toggle component values in the problem panel
    /// </summary>
    public void toggleDetails(){
        displayValues = !displayValues;

    }

/// <summary>
/// show scale text, called by avow Gen
/// </summary>
/// <param name="scale"></param>
    public void showScaleText(float scale){
        scaleText.text = scale.ToString() + " : 1";
        scaleText.transform.parent.gameObject.SetActive(true);
    }

/// <summary>
/// change size of the window,
/// </summary>
/// <param name="value">0: reset to defualt, 1: decrease size , 2: increase size</param>
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
            
            

        
        Vector2 SizeCheck = new Vector2();//check the size is within limits to prevent UI coverup
        SizeCheck.x= Mathf.Clamp(rectTransform.sizeDelta.x, DisplayW/60, DisplayW/5);
        SizeCheck.y= Mathf.Clamp(rectTransform.sizeDelta.y, DisplayH/60, DisplayH/5);
        rectTransform.sizeDelta = SizeCheck;
    }



/// <summary>
/// used to moveProblem camera
/// </summary>
/// <param name="Direction">0: move up, 1: move down, 2:move left, 3:move right 4:zoom in, 5:zoom out</param>
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
                problemCam.orthographicSize = defaultZoom;
                problemCam.transform.position = defaultPos;
                break;










        }



    }
}
