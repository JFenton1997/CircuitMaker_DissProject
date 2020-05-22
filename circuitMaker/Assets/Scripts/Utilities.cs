using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Utilities
{


    [System.Serializable]
    public class DiagramComponent
    {
        public string name;
        public ComponentType type;
        public List<DiagramComponent> Aconnections, Bconnections;
        public Dictionary<ComponentParameter, Pair> Values;
        public Direction direction;

        public DiagramComponent()
        {
            Aconnections = new List<DiagramComponent>();
            Bconnections = new List<DiagramComponent>();
            Values = new Dictionary<ComponentParameter, Pair>();
            Values.Add(ComponentParameter.VOLTAGE, new Pair(1, false));
            Values.Add(ComponentParameter.CURRENT, new Pair(1, false));
            Values.Add(ComponentParameter.RESISTANCE, new Pair(1, false));


        }



    }

    [System.Serializable]
    public class Pair<A, B>
    {
        public Pair() { }
        public Pair(A a, B b)
        {
            this.a = a;
            this.b = b;
        }
        [SerializeField]
        public A a { get; set; }
        [SerializeField]
        public B b { get; set; }


    };



    [System.Serializable]
    public class Pair
    {
        [SerializeField]
        private double _value;
        [SerializeField]
        private bool _hidden;
        public Pair() { }
        public Pair(double value, bool hidden)
        {
            this._value = value;
            this._hidden = hidden;
        }


        public double value { get { return _value; } set { _value = value; } }
        public bool hidden { get { return _hidden; } set { _hidden = value; } }



    };

    [System.Serializable]
    public enum ComponentType
    {
        UNTYPED = 0,
        CELL = 1,
        LIGHT = 2,
        RESISTOR = 3,
        SWITCH = 4

    }


    [System.Serializable]
    public class CircuitComponentBlueprint
    {
        public GameObject prefab;
        public ComponentType type;

    }



    public enum ComponentParameter
    {
        VOLTAGE,
        CURRENT,
        RESISTANCE

    }

    public enum Direction
    {
        A_to_B,
        B_to_A
    }

    public struct DiagramInstanceData
    {
        public Dictionary<int, List<DiagramComponent>> diagramData;
        public string title;
        public string author;
        public string diagramQuestion;
        public Pair<bool, bool> diagramEnabled;

        public DiagramInstanceData(string title, string author, string diagramQuestion, Pair<bool, bool> diagramEnabled, Dictionary<int, List<DiagramComponent>> diagramData)
        {
            this.title = title;
            this.author = author;
            this.diagramQuestion = diagramQuestion;
            this.diagramData = diagramData;
            this.diagramEnabled = diagramEnabled;




        }
    }

    public static class ExtraUtilities
    {

        public static bool isEqualWithTolarance(float a, float b, float tolarance)
        {
            if (b + tolarance >= a && b - tolarance <= a)
            {
                return true;
            }
            else return false;
        }





    }








}
