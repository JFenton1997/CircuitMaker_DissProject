using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public static class GlobalValues 
{
    public static bool AvowSnapping;
    public static float AvowSnappingOffset;

    public static bool ToolTipsEnabled;
    public static bool circuitDisplayAll;
    public static string workingDirectory;

    public static string fileSearch;

    public static DiagramInstanceData selectedDiagram;

    public static void getPlayerPrefs(){
        workingDirectory  = PlayerPrefs.GetString("workingDirectory","/DiagramFiles");
        switch( PlayerPrefs.GetInt("toolTipsEnables", 1)){
            case 0:
                ToolTipsEnabled = false;
                break;
            case 1:
                ToolTipsEnabled = true;
                break;
            default:
                Debug.Log("tool tips enabled invalid number, corrupted player prefs");
                break;
        }
        

    }

    public static void updatePlayerPrefs(){
        PlayerPrefs.SetString("workingDirectory",workingDirectory);
         if(ToolTipsEnabled)  PlayerPrefs.SetInt("toolTipsEnables", 1);
         else PlayerPrefs.SetInt("toolTipsEnables",0);
    }


    
}
