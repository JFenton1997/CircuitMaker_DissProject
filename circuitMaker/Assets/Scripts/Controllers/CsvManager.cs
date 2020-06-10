using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utilities;
using System;

/// <summary>
/// class for reading and writing diagram instance Data
/// </summary>
public class CsvManager : MonoBehaviour
{
    private string fileExtension = ".csv"; //file type created
    public string filePath = "Assets/DiagramFiles/"; //original file path used for debugging

    public string testNameRead; //test file for debugging
 
    List<string> toWrite; //string list (item per line ) to be written to a file
    Dictionary<int, List<DiagramComponent>> diagramData; //diagram data to be written or read
    List<DiagramComponent> createdComponents; //list of component created

/// <summary>
///     used to write a singular file, used mainly for debugging purposes
/// </summary>
/// <param name="diagramData">diagram data to be witten</param>
/// <param name="author">author of the diagram</param>
/// <param name="diagramTitle">title of the diagram</param>
/// <param name="diagramQuestion">question/desction of the problem to be solved</param>
/// <param name="scale">scale used for avow diagram generation</param>
/// <param name="diagramEnabled">bool[4], c>c , c>a , a>c , a>a</param>
/// <returns> bool if the file was successfully witten to a csv</returns>
    public bool WriteDigram(Dictionary<int, List<DiagramComponent>> diagramData, String author, String diagramTitle, String diagramQuestion, float scale, bool[] diagramEnabled)
    {
        DiagramInstanceData diagram = new DiagramInstanceData(diagramTitle, author, diagramQuestion, diagramEnabled, scale, diagramData);

        return (writeDataToCsv(diagram));
    }

/// <summary>
/// used for debugging to read a hardCoded file set as public parameters
/// </summary>
/// <returns> diagramInstanceData</returns>
    public DiagramInstanceData testRead()
    {
       return ReadFile(filePath +testNameRead, "JamesTest"+fileExtension).a;


    }

/// <summary>
/// Main method used to read files
/// reads all file in directory set in globalValues
/// returns all valid csv files in the directory after filter is applied
/// </summary>
/// <param name="filter">the type of problems to be returned</param>
/// <returns>diagramInstance data to be used to generate and show problem and a string containing time the file was saved</returns>
    public List<Pair<DiagramInstanceData,string>> GetAllFilesType(DiagramFilter filter)
    {
        List<Pair<DiagramInstanceData,string>> diagramsToReturn = new List<Pair<DiagramInstanceData, string>>();//initialize return list
        List<String> files = new List<string>(Directory.GetFiles(@GlobalValues.workingDirectory+"/", "*_*.csv")); //get a list of all file names in the directory following the 
                                                                                                                // following the correct format 
    //for each file
        foreach (String filename in files)
        {
            try
            {
                diagramsToReturn.Add(ReadFile(filename));//read file 

            }
            catch (System.Exception e)//if file is invalid and breads reader, report and move on
            {
                Debug.Log(filename + " invalid Structure");
            }
        }
        switch(filter){ //filter all return diagramInstances
            case DiagramFilter.CIRCUIT_TO_CIRCUIT:
                return diagramsToReturn.FindAll(x => x.a.diagramEnabled[0]);

            case DiagramFilter.CIRCUIT_TO_AVOW:
                return diagramsToReturn.FindAll(x => x.a.diagramEnabled[1]);

            case DiagramFilter.AVOW_TO_CIRCUIT:
                return diagramsToReturn.FindAll(x => x.a.diagramEnabled[2]);

            case DiagramFilter.AVOW_TO_AVOW:
                return diagramsToReturn.FindAll(x => x.a.diagramEnabled[3]);

            default:
                Debug.LogError("INVALID FILTER");
                return diagramsToReturn;



        }



        







    }


    /// <summary>
    /// writes a digramInstanceData to a file
    /// </summary>
    /// <param name="diagram">diagram created from a problem builder</param>
    /// <returns> bool to show if the file was successfully saved</returns>
    public bool writeDataToCsv(DiagramInstanceData diagram)
    {
        filePath = GlobalValues.workingDirectory+"/"; //get filepath
        toWrite = new List<string>(); 
        this.diagramData = diagram.diagramData;
        writeTitleBar(diagram.title, diagram.author, diagram.diagramQuestion, diagram.diagramEnabled, diagram.scale); //write titlebar
        writeComponentBar(); //write component bar
        //for each layer, write each component in order with it values
        foreach (var d in diagramData)
        {
            foreach (DiagramComponent e in d.Value)
            {
                addComponentRecord(e); 
            }
        }
        return writeFile(diagram.title, diagram.author);
    }



/// <summary>
/// used to write the first line into the csv
/// </summary>
/// <param name="title">diagram title</param>
/// <param name="author">diagram author</param>
/// <param name="diagramQuestion">question/description</param>
/// <param name="diagramEnabled"> problem the diagram is enabled for </param>
/// <param name="scale">scale value used by avow generation</param>
    private void writeTitleBar(string title, string author, string diagramQuestion, bool[] diagramEnabled, float scale)
    {//create string
        string recordData = title + "," + author + "," + diagramQuestion + "," + diagramEnabled[0] + "," + diagramEnabled[1] + ","
        + diagramEnabled[2] + "," + diagramEnabled[3] + ","+scale.ToString() +","+ System.DateTime.Now + ",\0";
        toWrite.Add(recordData);
    }


    /// <summary>
    /// write the list of component names following structure, using NULL to separate layers
    /// </summary>
    public void writeComponentBar()
    {
        string recordData = "";
        foreach (var d in diagramData)
        {
            foreach (DiagramComponent e in d.Value)
            {
                recordData += e.name + ",";
            }
            recordData += "\0,";
        }
        recordData += "\0";
        toWrite.Add(recordData);
    }

/// <summary>
/// filling a row filled with DiagramComponent data
/// </summary>
/// <param name="d">diagramData to write value for into a line </param>
    private void addComponentRecord(DiagramComponent d)
    {
        string recordData = d.name + "," + (int)d.type + "," + (int)d.direction + ",";
        foreach (var c in d.Values)
        {
            recordData += c.Value.value + "," + c.Value.hidden + ",";
        }
        foreach (string n in d.Aconnections.ConvertAll(i => i.name))
        {
            recordData += n + ",";
        }
        recordData += "\0,";
        foreach (string n in d.Bconnections.ConvertAll(i => i.name))
        {
            recordData += n + ",";
        }
        recordData += "\0";
        toWrite.Add(recordData);
    }

/// <summary>
/// used to write a List<String> into a csv, line by line with a streamWriter
/// </summary>
/// <param name="title">title of diagram used to make filename</param>
/// <param name="author">author of the diagram used to make filename</param>
/// <returns>bool if the file was successfully saved</returns>
    private bool writeFile(string title, string author)
    {
        try
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filePath+ title + "_" + author + fileExtension, false))
            {
                foreach (string record in toWrite)
                {
                    file.WriteLine(record);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

/// <summary>
/// reading a file from a given title and author
/// </summary>
/// <param name="title">title in file to read</param>
/// <param name="author">author of file to read</param>
/// <returns></returns>
    public Pair<DiagramInstanceData, string> ReadFile(string title, string author)
    {
        return ReadFile(title + "_" + author);

    }

/// <summary>
/// from a given filename read the csv and returns diagramInstanceData and time the file was created
/// </summary>
/// <param name="filename">name of file to read</param>
/// <returns></returns>
    public Pair<DiagramInstanceData, string> ReadFile(string filename)
    {
        filePath = GlobalValues.workingDirectory+"/"; //get directory
        //initializing values.
        string author = "";
        string title = "";
        string question = "";
        string time = "";
        bool[] diagramEnabled = new bool[4];
        float scale = 1;
        createdComponents = new List<DiagramComponent>();
        diagramData = new Dictionary<int, List<DiagramComponent>>();
        
        
        int lineNumber = 0;
        using (var reader = new StreamReader(@filename))
        {
            while (!reader.EndOfStream) //while a line still to read
            {
                var line = reader.ReadLine();

                var record = line.Split(','); //split each line by cells ( , )
                if (lineNumber == 0)//if line number is 0, the value will be diagramInstanceData data
                {
                    author = record[1];
                    title = record[0];
                    question = record[2];
                    diagramEnabled[0] = (bool.Parse(record[3]));
                    diagramEnabled[1] = (bool.Parse(record[4]));
                    diagramEnabled[2] = (bool.Parse(record[5]));
                    diagramEnabled[3] = (bool.Parse(record[6]));
                    scale = float.Parse(record[7]);
                    time = record[8];
                    
                }
                //if line is ==1 then the line will contain component row
                else if (lineNumber == 1)
                {
                    generateDiagramData(record);

                }
                else//if line >1 the must contain component values
                {
                    fillOutComponent(record);
                }
                lineNumber++;
                if (lineNumber > 1000) //if more than 1000lines , then likely to be infinite loop from reader and file is invalid
                {
                    throw new Exception("Invalid Data, while infiloop, >1000 lines");
                }
            }

        }
        this.diagramData.Remove(diagramData.Count - 1); //remove last row as it will be empty 
        return new Pair<DiagramInstanceData, string>(new DiagramInstanceData(title, author, question, diagramEnabled, scale, this.diagramData),time);
    }


/// <summary>
/// construct all components in the components row and store them in diagram data in the correct location based on structure
/// </summary>
/// <param name="record">component line (linenumber 1) in csv split by " , "</param>
    private void generateDiagramData(string[] record)
    {
        int key = 0; //layer 
        List<DiagramComponent> layerComponents = new List<DiagramComponent>();
        foreach (string s in record)
        {
            if (s == "\0") //if null, start a new layer of diagram data and store current layerComponent int the diagramData
            {
                diagramData.Add(key, layerComponents);
                layerComponents = new List<DiagramComponent>();
                key++;
            }
            else
            { //construct diagramComponent, give it the correct name from csv and add to layer and created components
                DiagramComponent d = new DiagramComponent();
                d.name = s;
                layerComponents.Add(d);
                createdComponents.Add(d);

            }
        }
    }

/// <summary>
/// used to read a component values layer in the csv and fill the corresponding diagramComponent in the diagramData
/// </summary>
/// <param name="record">component values line (linenumber >1) in csv split by " , "</param>
    private void fillOutComponent(string[] record)
    {
        DiagramComponent d = createdComponents[createdComponents.ConvertAll(i => i.name).IndexOf(record[0])];
        d.type = (ComponentType)(Int32.Parse(record[1]));
        d.direction = (Direction)(Int32.Parse(record[2]));
        d.Values[(ComponentParameter)0].value = (float.Parse(record[3]));
        d.Values[(ComponentParameter)0].hidden = (bool.Parse(record[4]));
        d.Values[(ComponentParameter)1].value = (float.Parse(record[5]));
        d.Values[(ComponentParameter)1].hidden = (bool.Parse(record[6]));
        d.Values[(ComponentParameter)2].value = (float.Parse(record[7]));
        d.Values[(ComponentParameter)2].hidden = (bool.Parse(record[8]));
        int pointer = 9;
        while (record[pointer] != "\0")
        {
            // Debug.Log(createdComponents.ConvertAll(i => i.name).IndexOf(record[pointer]) + " " + record[pointer]);
            d.Aconnections.Add(createdComponents[createdComponents.ConvertAll(i => i.name).IndexOf(record[pointer])]);
            pointer++;
            if (pointer > 1000)
            {
                throw new Exception("Invalid Data, while infiloop");
            }
        }
        pointer++;
        while (record[pointer] != "\0")
        {
            d.Bconnections.Add(createdComponents[createdComponents.ConvertAll(i => i.name).IndexOf(record[pointer])]);
            pointer++;
            if (pointer > 1000)
            {
                throw new Exception("Invalid Data, while infiloop");
            }
        }
        return;





    }

}


