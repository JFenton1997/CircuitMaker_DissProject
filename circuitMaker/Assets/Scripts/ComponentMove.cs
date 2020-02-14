using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class ComponentMove : MonoBehaviour {
    private Vector3 screenPoint;
    private Vector3 offset;
    

    public Color dragColor;
    private Color normColor;

    private bool isMoving;
    public void MoveStart() {
        Debug.Log("move");
        transform.position = new Vector3(0,0,0);
        //offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        normColor = this.GetComponent<SpriteRenderer>().color;
        isMoving = true;

    }

    private void Update() {
        if(isMoving){
            Cursor.visible = false;
            this.GetComponent<SpriteRenderer>().color = dragColor;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);// + offset;
            transform.position = new Vector2(Mathf.RoundToInt(curPosition.x), Mathf.RoundToInt(curPosition.y));


 
        

        }



            Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
            // RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
            if(hitInfo)
            {
                Debug.Log("Collider");
                Debug.Log( hitInfo.transform.gameObject.name );
                // Here you can check hitInfo to see which collider has been hit, and act appropriately.
            }
    }
    

    private void OnMouseDown() {
        if(isMoving){
            Cursor.visible = true;
            

            Debug.Log("endMoving");
            isMoving = false;
            this.GetComponent<SpriteRenderer>().color = normColor;  
        }
    }
}