using System.Collections;
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
    RectTransform rectTransform;
    public float panSpeed = 5f;

    int DisplayW, DisplayH;
    public bool displayValues = true;
    // Start is called before the first frame update
    void Start()
    {

        DisplayH = Screen.height;
        DisplayW = Screen.width;
        problemCam = transform.Find("/ProblemCamera").GetComponent<Camera>();
        viewImage = transform.Find("Display").GetComponent<RawImage>();
        if (problemCam.activeTexture == null)
        {
            problemCam.targetTexture = new RenderTexture(DisplayH, DisplayW, 24);
        }
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(DisplayW / 5, DisplayH / 5);


        var renderer = GetComponent<Renderer>();





    }

    // Update is called once per frame
    void Update()
    {


        viewImage.texture = problemCam.activeTexture;



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








        }



    }
}
