using UnityEngine;
/// <summary>
/// class for controling the main camera
/// </summary>
public class CameraController : MonoBehaviour
{
    //camrea parameters
    public float panSpeed = 30f;
    public float panBorderThickness = 10f;
    private Camera camera;
    //keys used to move camera
    public KeyCode moveUp,moveDown, moveLeft,MoveRight;

    public float scrollSpeed = 5f;
    public float minY = 10f;
    public float maxY = 80f;
    public float clamps = 128f;

    private Vector3 mouseLocation;

     [Range(0.1f, 1f)]
    public float panIntensity;

    // Update is called once per frame

/// <summary>
/// get camera on start
/// </summary>
    private void Awake()
    {
		camera = transform.GetComponent<Camera>();

    }

/// <summary>
/// used for pan camera with middle mouse button
/// </summary>
/// <param name="Axis">axis of movement</param>
/// <param name="dir">direction of pan</param>
/// <param name="response">pan intensity</param>
     void GetInputForAxis(string Axis, Vector3 dir, float response)
    {
        
        float move = 0;
        float speed = Input.GetAxis(Axis); //axis of movement on mouse
        move += speed * response;
 
        if (move != 0)
        {
            transform.Translate(dir * move); //translate camera to new location in rection
        }
    }
    void Update()
    {
        //if middle mouse button is down, pan camera
         if(Input.GetMouseButton(2))
        {
            Cursor.visible = false;
            mouseLocation = Input.mousePosition;
            Cursor.lockState = CursorLockMode.Confined;
            GetInputForAxis("Mouse X", Vector3.right, panIntensity);
            GetInputForAxis("Mouse Y", Vector3.up, panIntensity);
        }

        // if middle mouse is up, show mouse, and unlock mouse
        if(Input.GetMouseButtonUp(2)){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }



        //if key in direction is down, move camera in that directions
        if (Input.GetKey(moveUp) )//|| Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            transform.Translate(Vector3.up * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(moveDown) )//|| Input.mousePosition.y <= panBorderThickness)
        {
            transform.Translate(Vector3.down * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(MoveRight))// || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(moveLeft))//|| Input.mousePosition.x <= panBorderThickness)
        {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }

        //get scroll value 
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 pos = transform.position;
        //increase camera size based of scroll, clamp to min and max zoom
        camera.orthographicSize -= scroll * 500 * scrollSpeed * Time.deltaTime;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minY, maxY);
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minY, maxY);
          
        Vector3 checkPos = new Vector2();
        //stop camera leaving diagram boundary
        checkPos.z =-10; 
        checkPos.x= Mathf.Clamp(  transform.position.x,-clamps, clamps);
        checkPos.y= Mathf.Clamp(transform.position.y, -clamps, clamps);
        transform.position = checkPos;

    }

    /// <summary>
    /// resets camera position to middle, used incase user loses diagram
    /// </summary>
    public void resetCam(){
        transform.position = new Vector3(0f,0f,-10f);

    }
}
