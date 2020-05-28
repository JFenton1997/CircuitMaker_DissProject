using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using UnityEngine.Events;

public class FileSearchWIndow : MonoBehaviour
{


    private CanvasGroup canvasGroup;
    public GameObject FileMessagePrefab;
    private Transform FilesDisplay;
    private List<DiagramInstanceData> DiagramFiles;
    public DiagramFilter filter;
    private int colorNum = 0;
    private int fileNo = 0;
    public Color errorColor = Color.red;

    [SerializeField]
    List<Color> colours;
    // Start is called before the first frame update
    void Start()
    {
        DiagramFiles = transform.Find("/ProgramMaster").GetComponent<CsvManager>().GetAllFilesType(filter);
        FilesDisplay = transform.Find("Image/Contents/Main/Panel/Scroll View/Viewport/FileDisplay");
        displayFiles();
    }

    // Update is called once per frame
    public void displayFiles()
    {


        transform.Find("Image/Contents/Main/Panel/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().value = 1f;
        if (DiagramFiles.Count == 0)
        {
            GameObject FileLog = (GameObject)Instantiate(FileMessagePrefab, FilesDisplay.position, Quaternion.identity, FilesDisplay);
            FileLog.transform.Find("Title").GetComponent<Text>().text = "No Files Found";
            FileLog.transform.Find("Items/Author").GetComponent<Text>().text = "Current Directory:" + GlobalValues.workingDirectory;
            FileLog.transform.Find("Items/Desc").GetComponent<Text>().text = "either change directory in settings or make sure there is a created problem with the correct problem to solve enabled";
            FileLog.transform.Find("forground").GetComponent<Image>().color = colours[colorNum % colours.Count];
            colorNum++;


        }
        foreach (DiagramInstanceData d in DiagramFiles)
        {
            GameObject FileLog = (GameObject)Instantiate(FileMessagePrefab, FilesDisplay.position, Quaternion.identity, FilesDisplay);
            FileLog.gameObject.name = fileNo.ToString();
            FileLog.transform.Find("Title").GetComponent<Text>().text = d.title;
            FileLog.transform.Find("Items/Author").GetComponent<Text>().text = d.author;
            FileLog.transform.Find("Items/Desc").GetComponent<Text>().text = d.diagramQuestion;
            Button fileLogButton = FileLog.GetComponent<Button>();
            ColorBlock buttonColorBlock = fileLogButton.colors;
            buttonColorBlock.normalColor = colours[colorNum % colours.Count];
            fileLogButton.colors = buttonColorBlock;
            fileLogButton.onClick.AddListener(delegate { diagramSelected(fileLogButton.transform); });
            colorNum++;
            fileNo++;
        }
    }

    public void diagramSelected(Transform button)
    {
        GlobalValues.selectedDiagram = DiagramFiles[int.Parse(button.gameObject.name)];
        Debug.Log(GlobalValues.selectedDiagram.title);
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




