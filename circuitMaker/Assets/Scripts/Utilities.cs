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
        private float _value;
        [SerializeField]
        private bool _hidden;
        public Pair() { }
        public Pair(float value, bool hidden)
        {
            this._value = value;
            this._hidden = hidden;
        }


        public float value { get { return _value; } set { _value = value; } }
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
        public bool[] diagramEnabled;

        public DiagramInstanceData(string title, string author, string diagramQuestion, bool[] diagramEnabled, Dictionary<int, List<DiagramComponent>> diagramData)
        {
            this.title = title;
            this.author = author;
            this.diagramQuestion = diagramQuestion;
            this.diagramData = diagramData;
            this.diagramEnabled = diagramEnabled;




        }
    }

    public struct DiagramError
    {
        public string errorName;
        public string errorDiscription;
        public DiagramComponent component;



        public DiagramError(string name, string desc)
        {
            errorName = name;
            errorDiscription = desc;
            component = null;

        }

        public DiagramError(string name, string desc, DiagramComponent component, object allComponents)
        {
            errorName = name;
            errorDiscription = desc;
            this.component = component;
            setErrorColor(allComponents);

        }

        public void setErrorColor(object allComponents)
        {
            DiagramComponent component = this.component;
            if (allComponents is List<CircuitComponent>)
            {
                List<CircuitComponent> allComponentsCircuit = (List<CircuitComponent>)allComponents;
                allComponentsCircuit.Find(x => x.component == component).toErrorColor();
            }
            else if (allComponents is List<AvowComponent>)
            {
                List<AvowComponent> allComponentsCircuit = (List<AvowComponent>)allComponents;
                allComponentsCircuit.Find(x => x.component == component).ColorToParam(Color.red);

            }


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


    public enum DiagramFilter{
        CIRCUIT_TO_CIRCUIT = 0,
        CIRCUIT_TO_AVOW = 1,
        AVOW_TO_CIRCUIT =2,
        AVOW_TO_AVOW = 3
        
    }
}











