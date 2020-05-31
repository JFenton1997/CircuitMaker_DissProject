using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Utilities namespace used as a generic classes, structs and enums used thoughout the system.
/// </summary>
namespace Utilities
{


    [System.Serializable]
    /// <summary>
    /// DiagramComponent data required for both AVOW and Circuits to function
    /// </summary>
    public class DiagramComponent
    {
        public string name; // name for labelling 
        public ComponentType type;
        public List<DiagramComponent> Aconnections, Bconnections; // storing inputs and outputs
        public Dictionary<ComponentParameter, Pair> Values; // perameter as Key (voltage, current, resistance) 
                                                            // Pair as FLOAT value and BOOL hidden (if shown in problem) 
        public Direction direction; // direction used to calculate what is input and output, used by circuit diagrams

        public DiagramComponent() // constructor, on component build
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
    /// <summary>
    /// GENERIC Pair Class, C# dont have PAIRS built.
    /// </summary>
    /// <typeparam name="A">generic type  A</typeparam>
    /// <typeparam name="B">generic type B</typeparam>
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
    /// <summary>
    /// pairs for component values, UNITY dont show GENERIC classes in inspector, non generic used for debugging
    /// </summary>
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
    /// <summary>
    /// specific types, uses enum
    /// </summary>
    public enum ComponentType
    {
        UNTYPED = 0, // used for non functional components ( used in GENS)
        CELL = 1,
        LIGHT = 2,
        RESISTOR = 3,
        SWITCH = 4 //not implemented (futureproofing)

    }



    [System.Serializable]
    /// <summary>
    /// from old system of building circuits, needs to be removed
    /// </summary>
    public class CircuitComponentBlueprint
    {
        public GameObject prefab;
        public ComponentType type;

    }

/// <summary>
/// ENUM for specific paramters of a component

/// </summary>
    public enum ComponentParameter
    {
        VOLTAGE,
        CURRENT,
        RESISTANCE

    }

/// <summary>
/// direction enum showing on 2 possible values
/// </summary>
    public enum Direction
    {
        A_to_B,
        B_to_A
    }

/// <summary>
/// used to store problem data, reading and writing to a CSV.
/// </summary>
    public struct DiagramInstanceData
    {
        public Dictionary<int, List<DiagramComponent>> diagramData;
        public string title;
        public string author;
        public string diagramQuestion;
        public bool[] diagramEnabled;
        public float scale; // used for avows

        public DiagramInstanceData(string title, string author, string diagramQuestion, bool[] diagramEnabled, float scale, Dictionary<int, List<DiagramComponent>> diagramData)
        {
            this.title = title;
            this.author = author;
            this.diagramQuestion = diagramQuestion;
            this.diagramData = diagramData;
            this.diagramEnabled = diagramEnabled;
            this.scale = scale;




        }
    }

/// <summary>
/// error storage struct to create error messages for the user
/// </summary>
    public struct DiagramError
    {
        public string errorName;
        public string errorDiscription;
        public DiagramComponent component;


/// <summary>
/// constructor for diagram error
/// </summary>
/// <param name="name">diagram error name</param>
/// <param name="desc">the description of the error</param>
        public DiagramError(string name, string desc)
        {
            errorName = name;
            errorDiscription = desc;
            component = null;

        }

        /// <summary>
        /// constructor with a component, used to highlight error components
        /// </summary>
        /// <param name="name">error name</param>
        /// <param name="desc">error desc</param>
        /// <param name="component">component related to error</param>
        /// <param name="allComponents">main list containing the list</param>

        public DiagramError(string name, string desc, DiagramComponent component, object allComponents)
        {
            errorName = name;
            errorDiscription = desc;
            this.component = component;
            setErrorColor(allComponents);

        }

/// <summary>
/// for highlights errors
/// </summary>
/// <param name="allComponents">list of all components to use to set error</param>
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

/// <summary>
/// class for extra methods
/// </summary>
    public static class ExtraUtilities
    {
        /// <summary>
        /// a equals methods which allows for a tolarance of error
        /// </summary>
        /// <param name="a">value A</param>
        /// <param name="b">value B</param>
        /// <param name="tolarance">tolarance value + -</param>
        /// <returns></returns>

        public static bool isEqualWithTolarance(float a, float b, float tolarance)
        {
            if (b + tolarance >= a && b - tolarance <= a)
            {
                return true;
            }
            else return false;
        }





    }


/// <summary>
/// used to filter for specific types of solve problems
/// </summary>
    public enum DiagramFilter{
        CIRCUIT_TO_CIRCUIT = 0,
        CIRCUIT_TO_AVOW = 1,
        AVOW_TO_CIRCUIT =2,
        AVOW_TO_AVOW = 3
        
    }
}











