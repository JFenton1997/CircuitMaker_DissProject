using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AvowValuesPanel : MonoBehaviour
{

    private AvowConponent currentAvow;
    public InputField height, width;
    public Toggle voltHidden, currentHidden, resistanceHidden;
    public Dropdown AvowConponentType;
    public Text selectedText;
    public CanvasGroup canvasGroup;
    // Start is called before the first frame update

    private void Update() {
        if(!currentAvow){
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            
        }else{
            currentAvow.ColorToSelected();
        }

      if (Input.GetMouseButtonDown(1) && currentAvow){
          currentAvow.ColorToMain();
          currentAvow= null;

      }

    }



    public void newSelected(AvowConponent avowConponent){
        if(currentAvow && currentAvow != avowConponent) currentAvow.ColorToMain();
        canvasGroup.blocksRaycasts = true;
        currentAvow = avowConponent;
        currentAvow.ColorToSelected();
        canvasGroup.alpha = 1f;
        selectedText.text = avowConponent.gameObject.name;
        Vector2 avowSize = currentAvow.rectTransform.sizeDelta;
        width.text = avowSize.x.ToString();
        height.text = avowSize.y.ToString();
        AvowConponentType.value = (int)currentAvow.component.type - 2;

        

    }



    public void updateAvowValues(){
        currentAvow.voltage = float.Parse(height.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        currentAvow.current = float.Parse(width.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
    }


}
