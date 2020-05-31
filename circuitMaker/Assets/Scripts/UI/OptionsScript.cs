using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI class for Options scene
/// </summary>
public class OptionsScript : MonoBehaviour
{

    public InputField newDirectory;
    public Text activeFileDirectory;
    private string currentDirectory;
    public Color errorColor, normalColor;

/// <summary>
/// getting UI components
/// </summary>
    private void Start()
    {
        currentDirectory = GlobalValues.workingDirectory;
        newDirectory.text = currentDirectory;
        activeFileDirectory.text = currentDirectory;


    }

/// <summary>
/// resetting directory to applications default 
/// </summary>
    public void resetDirectory()
    {
        newDirectory.textComponent.color = normalColor; //reset color
        if (!System.IO.Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/DiagramFiles")) //if original file dont exist create it
        {
            System.IO.Directory.CreateDirectory((System.Environment.SpecialFolder.MyDocuments) + "/DiagramFiles");
        }
        GlobalValues.workingDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/DiagramFiles"; //set globalvalues directory to its default state
        GlobalValues.updatePlayerPrefs(); //save new value to player prefs
        newDirectory.text = currentDirectory;// updates texts on options scene
        activeFileDirectory.text = currentDirectory;

    }

/// <summary>
/// used to change the directory from a input field
/// </summary>
    public void changeDirectory()
    {
        string newPath = newDirectory.text; //get input field value
        if (!System.IO.Directory.Exists(newPath)) //if path dont exist
        {
            newDirectory.textComponent.color = errorColor; //show error to user
        }
        else
        {
            newDirectory.textComponent.color = normalColor; //set to normal colour
            GlobalValues.workingDirectory = newDirectory.text; //set new working directory
            GlobalValues.updatePlayerPrefs(); //update playerprefs with new directory
            newDirectory.text = currentDirectory; //update text values to show change
            activeFileDirectory.text = currentDirectory;

        }

    }

/// <summary>
/// reset all player prefs
/// </summary>
    public void resetPlayerPreff()
    {
        newDirectory.textComponent.color = normalColor;
        GlobalValues.clearPlayerPrefs();
        GlobalValues.getPlayerPrefs();
        currentDirectory = GlobalValues.workingDirectory;
        newDirectory.text = currentDirectory; //show reset to user
        activeFileDirectory.text = currentDirectory;
        normalColor = newDirectory.textComponent.color;

    }




}
