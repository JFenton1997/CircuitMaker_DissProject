using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Utilities
{
    [System.Serializable]
    public class Pair<A, B>
    {
        public Pair(){}
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
        public Pair(){}
        public Pair(float value, bool hidden)
        {
            this._value = value;
            this._hidden = hidden;
        }


        public float value { get{return _value;} set{_value = value;}}
        public bool hidden { get{ return _hidden;} set{_hidden = value;}}



    };

    [System.Serializable]
    public enum ComponentType{
        UNTYPED = 0,
        CELL = 1,
        LIGHT = 2,
        RESISTER = 3,
        SWITCH = 4

    }


    public enum ComponentParameter{
        VOLTAGE,
        CURRENT,
        RESISTANCE

    }

    public enum Direction{
        A_to_B,
        B_to_A
    }




}
