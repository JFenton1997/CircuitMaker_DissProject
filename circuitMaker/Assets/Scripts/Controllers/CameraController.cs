using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float panSpeed = 30f;
    public float panBorderThickness = 10f;
    private Camera camera;

    public KeyCode moveUp,moveDown, moveLeft,MoveRight;

    public float scrollSpeed = 5f;
    public float minY = 10f;
    public float maxY = 80f;
    public float clamps = 128f;

    private Vector3 mouseLocation;

     [Range(0.1f, 1f)]
    public float panIntensity;

    // Update is called once per frame

    private void Awake()
    {
		camera = transform.GetComponent<Camera>();

    }

     void GetInputForAxis(string Axis, Vector3 dir, float response)
    {
        
        float move = 0;
        float speed = Input.GetAxis(Axis);
        move += speed * response;
 
        if (move != 0)
        {
            transform.Translate(dir * move);
        }
    }
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        
         if(Input.GetMouseButton(3))
        {
            Cursor.visible = false;
            mouseLocation = Input.mousePosition;
            Cursor.lockState = CursorLockMode.Confined;
            GetInputForAxis("Mouse X", Vector3.right, panIntensity);
            GetInputForAxis("Mouse Y", Vector3.up, panIntensity);
        }

        if(Input.GetMouseButtonUp(3)){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }




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

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 pos = transform.position;

        camera.orthographicSize -= scroll * 500 * scrollSpeed * Time.deltaTime;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minY, maxY);
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minY, maxY);
          
        Vector3 checkPos = new Vector2();
        checkPos.z =-10; 
        checkPos.x= Mathf.Clamp(  transform.position.x,-clamps, clamps);
        checkPos.y= Mathf.Clamp(transform.position.y, -clamps, clamps);
        transform.position = checkPos;

    }

    public void resetCam(){
        transform.position = new Vector3(0f,0f,-10f);

    }
}
