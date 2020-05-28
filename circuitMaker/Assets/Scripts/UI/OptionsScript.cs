using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionsScript : MonoBehaviour
{

    public InputField newDirectory;
    public Text activeFileDirectory;
    private string currentDirectory;
    public Color errorColor, normalColor;

    private void Start()
    {
        currentDirectory = GlobalValues.workingDirectory;
        newDirectory.text = currentDirectory;
        activeFileDirectory.text = currentDirectory;


    }

    public void resetDirectory()
    {
        newDirectory.textComponent.color = normalColor;
        if (!System.IO.Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/diagramFiles"))
        {
            System.IO.Directory.CreateDirectory((System.Environment.SpecialFolder.MyDocuments) + "/diagramFiles");
        }
        GlobalValues.workingDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/diagramFiles";
        GlobalValues.updatePlayerPrefs();
        newDirectory.text = currentDirectory;
        activeFileDirectory.text = currentDirectory;

    }

    public void changeDirectory()
    {
        string newPath = newDirectory.text;
        if (!System.IO.Directory.Exists(newPath))
        {
            newDirectory.textComponent.color = errorColor;
        }
        else
        {
            newDirectory.textComponent.color = normalColor;
            GlobalValues.workingDirectory = newDirectory.text;
            GlobalValues.updatePlayerPrefs();
            newDirectory.text = currentDirectory;
            activeFileDirectory.text = currentDirectory;

        }

    }

    public void resetPlayerPreff()
    {
        newDirectory.textComponent.color = normalColor;
        GlobalValues.clearPlayerPrefs();
        GlobalValues.getPlayerPrefs();
        currentDirectory = GlobalValues.workingDirectory;
        newDirectory.text = currentDirectory;
        activeFileDirectory.text = currentDirectory;
        normalColor = newDirectory.textComponent.color;

    }




}
