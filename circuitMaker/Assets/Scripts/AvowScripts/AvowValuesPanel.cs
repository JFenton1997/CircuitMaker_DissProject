using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utilities;

public class AvowValuesPanel : MonoBehaviour
{

    private AvowConponent currentAvow;
    public InputField height, width, selectedText;
    public Toggle voltHidden, currentHidden, resistanceHidden;
    public Dropdown AvowConponentType;

    private CanvasGroup canvasGroup;
    // Start is called before the first frame update

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Update() {
        if(!currentAvow){
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            
        }else{
            currentAvow.ColorToSelected();
        }

      if (Input.GetMouseButtonDown(1) && currentAvow){
          updateAvowValues();
          currentAvow.ColorToMain();
          currentAvow= null;

      }

    }



    public void newSelected(AvowConponent avowConponent){
        if(currentAvow)updateAvowValues();
        
        if(currentAvow && currentAvow != avowConponent) currentAvow.ColorToMain();
        currentAvow = null;
        canvasGroup.blocksRaycasts = true;


        currentAvow = avowConponent;
        currentAvow.ColorToSelected();
        canvasGroup.alpha = 1f;
        selectedText.text = currentAvow.gameObject.name;
        Vector2 avowSize = currentAvow.rectTransform.sizeDelta;
        width.text = avowSize.x.ToString();
        height.text = avowSize.y.ToString();
        AvowConponentType.value = (int)currentAvow.component.type - 2;

        // Debug.Log(currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden+" "+
        // currentAvow.component.Values[ComponentParameter.CURRENT].hidden +" "+currentAvow.component.Values[ComponentParameter.RESISTANCE ].hidden);

        voltHidden.isOn = currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden;
        currentHidden.isOn = currentAvow.component.Values[ComponentParameter.CURRENT].hidden;
        resistanceHidden.isOn = currentAvow.component.Values[ComponentParameter.RESISTANCE].hidden;


        

    }



    public void updateAvowValues(){
        if(float.Parse(height.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat)>0)
        currentAvow.voltage = float.Parse(height.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        if(float.Parse(width.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat)!=0)
        currentAvow.current = float.Parse(width.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        if(selectedText.text !=""){
            currentAvow.name = selectedText.text;
            currentAvow.gameObject.name = selectedText.text;
        }

        currentAvow.component.type =(ComponentType)AvowConponentType.value +2;
        AvowConponentType.value = (int)currentAvow.component.type - 2;
        currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden = voltHidden.isOn ;
        currentAvow.component.Values[ComponentParameter.CURRENT].hidden = currentHidden.isOn ;
        currentAvow.component.Values[ComponentParameter.RESISTANCE].hidden = resistanceHidden.isOn;

        //         Debug.Log(currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden+" "+
        // currentAvow.component.Values[ComponentParameter.CURRENT].hidden +" "+currentAvow.component.Values[ComponentParameter.RESISTANCE ].hidden);

    }

    public void DestoryCurrentSelected(){
        if(currentAvow){
            Destroy(currentAvow.gameObject);
            currentAvow = null;
            Cursor.visible = true;
        }
    }


}
