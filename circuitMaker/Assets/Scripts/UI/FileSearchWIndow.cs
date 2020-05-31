using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using UnityEngine.Events;
/// <summary>
/// UI and Functional class to show files and get the selected file from the user
/// </summary>
public class FileSearchWIndow : MonoBehaviour
{


    private CanvasGroup canvasGroup;
    public GameObject FileMessagePrefab;
    private Transform FilesDisplay;
    private List<Pair<DiagramInstanceData,string>> DiagramFiles;
    public DiagramFilter filter;
    private int colorNum = 0;
    private int fileNo = 0;
    public Color errorColor = Color.red;

    [SerializeField]
    List<Color> colours;
    
    /// <summary>
    /// getting UI components
    /// </summary>
    void Start()
    {
        DiagramFiles = transform.Find("/ProgramMaster").GetComponent<CsvManager>().GetAllFilesType(filter); //get files from directory of a given filter
        FilesDisplay = transform.Find("Image/Contents/Main/Panel/Scroll View/Viewport/FileDisplay"); // get file display window
        displayFiles(); //display files
    }

   /// <summary>
   /// display all obtained files to the user 
   /// </summary>
    public void displayFiles()
    {

        //set the scroll bar to the top 
        transform.Find("Image/Contents/Main/Panel/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().value = 1f;
        //if no file where found, notify the user
        if (DiagramFiles.Count == 0)
        {
            GameObject FileLog = (GameObject)Instantiate(FileMessagePrefab, FilesDisplay.position, Quaternion.identity, FilesDisplay);
            FileLog.transform.Find("Title").GetComponent<Text>().text = "No Files Found";
            FileLog.transform.Find("Author").GetComponent<Text>().text = "Current Directory:" + GlobalValues.workingDirectory;
            FileLog.transform.Find("Desc").GetComponent<Text>().text = "either change directory in settings or make sure there is a created problem with the correct problem to solve enabled";
            FileLog.transform.Find("forground").GetComponent<Image>().color = colours[colorNum % colours.Count];
            FileLog.transform.Find("FileName").GetComponent<Text>().text = "";
            FileLog.transform.Find("Time").GetComponent<Text>().text ="";
            colorNum++;


        }
        //for each file found, display file information to the user
        foreach (Pair<DiagramInstanceData,string> d in DiagramFiles)
        {
            GameObject FileLog = (GameObject)Instantiate(FileMessagePrefab, FilesDisplay.position, Quaternion.identity, FilesDisplay);
            FileLog.gameObject.name = fileNo.ToString(); //set message GameObject name to correspond to the index of the file it represents
            FileLog.transform.Find("Title").GetComponent<Text>().text = d.a.title;
            FileLog.transform.Find("Author").GetComponent<Text>().text = d.a.author;
            FileLog.transform.Find("Desc").GetComponent<Text>().text = d.a.diagramQuestion;
            FileLog.transform.Find("FileName").GetComponent<Text>().text = d.a.title+ "_"+d.a.author+".csv";
            FileLog.transform.Find("Time").GetComponent<Text>().text = d.b;
            Button fileLogButton = FileLog.GetComponent<Button>();
            ColorBlock buttonColorBlock = fileLogButton.colors; //set the colour of the block
            buttonColorBlock.normalColor = colours[colorNum % colours.Count];
            fileLogButton.colors = buttonColorBlock;
            //add a event to each file log, to invoke diagram select when clicked and send its name to this class
            fileLogButton.onClick.AddListener(delegate { diagramSelected(fileLogButton.transform); });
            colorNum++;
            fileNo++;
        }
    }

/// <summary>
/// using a file message name to get the index of the selected file 
/// </summary>
/// <param name="button">file log selected by the user</param>
    public void diagramSelected(Transform button)
    {
        //set selected file in the globalValues to generated in the solver using buttons name
        GlobalValues.selectedDiagram = DiagramFiles[int.Parse(button.gameObject.name)].a;
        Debug.Log(GlobalValues.selectedDiagram.title);
        //load the corresponding solver scene using the sceneManager loadscene
        switch (filter)
        {
            case DiagramFilter.CIRCUIT_TO_CIRCUIT:
                transform.Find("/ProgramMaster").GetComponent<AppSceneManager>().loadScene(6);
                break;
            case DiagramFilter.CIRCUIT_TO_AVOW:
                transform.Find("/ProgramMaster").GetComponent<AppSceneManager>().loadScene(10);
                break;
            case DiagramFilter.AVOW_TO_CIRCUIT:
                transform.Find("/ProgramMaster").GetComponent<AppSceneManager>().loadScene(12);
                break;
            case DiagramFilter.AVOW_TO_AVOW:
                transform.Find("/ProgramMaster").GetComponent<AppSceneManager>().loadScene(11);
                break;
        }


    }

}




