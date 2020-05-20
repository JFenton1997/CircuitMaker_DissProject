using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utilities;
using System;


public class CsvManager : MonoBehaviour
{
    public string fileExtension = ".csv";
    public string filePath = "Assets/DiagramFiles/";
    public String testNameWrite;
    public string testNameRead;
    public GenerateCircuit generate;
    
    public CircuitManager circuitManager;
    List<string> toWrite;
    Dictionary<int, List<DiagramComponent>> diagramData;
    List<DiagramComponent> createdComponents;

    public bool WriteDigram(Dictionary<int, List<DiagramComponent>> diagramData, String author, String diagramTitle)
    {
        DiagramInstanceData diagram = new DiagramInstanceData(diagramTitle, author,  diagramData);
        return (writeDataToCsv(diagram));
    }

    public void testRead(){
        generate.GenerateCircuitObject(ReadFile(testNameRead,"JamesTest").diagramData);
        

    }


    // Update is called once per frame
    public bool writeDataToCsv(DiagramInstanceData diagram)
    {
        toWrite = new List<string>();
        this.diagramData = diagram.diagramData;
        writeTitleBar(diagram.title, diagram.author);
        writeComponentBar();
        foreach (var d in diagramData)
        {
            foreach (DiagramComponent e in d.Value)
            {
                addComponentRecord(e);
            }
        }
        return writeFile(diagram.title, diagram.author);
    }




    private void writeTitleBar(string title, string author)
    {
        string recordData = title + "," + author + "," + System.DateTime.Now + ",\0";
        toWrite.Add(recordData);
    }
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

    private bool writeFile(string title, string author)
    {
        try
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filePath + title + "_" + author + fileExtension, false))
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

    public DiagramInstanceData ReadFile(string title, string author)
    {
        return ReadFile(title + "_" + author);

    }

    public DiagramInstanceData ReadFile(string filename)
    {
        string author="";
        string title="";
        createdComponents = new List<DiagramComponent>();
        diagramData = new Dictionary<int, List<DiagramComponent>>();

        int lineNumber = 0;
        using (var reader = new StreamReader(@filePath + filename + fileExtension))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var record = line.Split(',');
                if (lineNumber == 0)
                {
                    Pair<string, string> info = getTitleBar(record);
                    author = info.b;
                    title = info.a;
                }
                else if (lineNumber == 1)
                {
                    generateDiagramData(record);

                }
                else
                {
                    fillOutComponent(record);
                }
                lineNumber++;
                if (lineNumber > 1000)
                {
                    throw new Exception("Invalid Data, while infiloop, >1000 lines");
                }
            }

        }
        this.diagramData.Remove(diagramData.Count-1);
        return new DiagramInstanceData(title,author,this.diagramData);
    }


    private Pair<String, String> getTitleBar(string[] record)
    {
        return new Pair<String, String>(record[0], record[1]);

    }

    private void generateDiagramData(string[] record)
    {
        int key = 0;
        List<DiagramComponent> layerComponents = new List<DiagramComponent>();
        foreach (string s in record)
        {
            if (s == "\0")
            {
                diagramData.Add(key, layerComponents);
                layerComponents = new List<DiagramComponent>();
                key++;
            }
            else
            {
                DiagramComponent d = new DiagramComponent();
                d.name = s;
                layerComponents.Add(d);
                createdComponents.Add(d);

            }
        }
    }

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
            Debug.Log(createdComponents.ConvertAll(i => i.name).IndexOf(record[pointer])+ " "+ record[pointer]);
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


