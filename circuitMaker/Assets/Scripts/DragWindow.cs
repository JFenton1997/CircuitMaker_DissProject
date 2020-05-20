// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;

// public class DragWindow : MonoBehaviour, IDragHandler
// {

//     [SerializeField]
//     private RectTransform dragRectTransform;
//     [SerializeField] private Canvas window;
//     public float offset;
//     void awake(){
//         if(dragRectTransform == null){
//             dragRectTransform = transform.parent.GetComponent<RectTransform>();
//         }

//         if(window == null){
//             Transform testCanvasTranform = transform.parent;
//             while(testCanvasTranform != null){
//                 window = testCanvasTranform.GetComponent<Canvas>();
//                 if(window){
//                     break;
//                 }
//                 testCanvasTranform = testCanvasTranform.parent;
//             }
//         }

//     }


//     public void OnBeginDrag(PointerEventData eventData){
        
//     }
//     public void OnDrag(PointerEventData eventData){
//         Debug.Log("dragging");
//         dragRectTransform.anchoredPosition+= eventData.delta/(window.scaleFactor*offset);
//     }

//     public void OnPointerDown(PointerEventData eventData){
//         dragRectTransform.SetAsLastSibling();
//     }

// }
