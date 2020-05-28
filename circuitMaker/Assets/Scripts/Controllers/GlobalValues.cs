using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.IO;

public static class GlobalValues 
{
    public static bool AvowSnapping;
    public static float AvowSnappingOffset;

    public static bool ToolTipsEnabled;
    public static bool circuitDisplayAll;
    public static string workingDirectory;

    public static string fileSearch;
    public static string authorName;

    public static DiagramInstanceData selectedDiagram;
     public static void getPlayerPrefs(){
         if (!PlayerPrefs.HasKey(workingDirectory)){
             if(!System.IO.Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/diagramFiles"))
             Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/diagramFiles");
         }

        workingDirectory  = PlayerPrefs.GetString("workingDirectory",System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/diagramFiles");
        authorName  = PlayerPrefs.GetString("author","");
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
        PlayerPrefs.SetString("author",authorName);
         if(ToolTipsEnabled)  PlayerPrefs.SetInt("toolTipsEnables", 1);
         else PlayerPrefs.SetInt("toolTipsEnables",0);
    }

    public static void clearPlayerPrefs(){
        PlayerPrefs.DeleteAll();
    }


    
}
