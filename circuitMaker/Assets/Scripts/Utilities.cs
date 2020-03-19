using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public class Pair<A, B>
    {
        public Pair()
        {
        }

        public Pair(A a, B b)
        {
            this.a = a;
            this.b = b;
        }

        public A a { get; set; }
        public B b { get; set; }

    };

    public enum CircuitComponentName{
        Cell = 0,
        Light = 1,
        Resister = 2,
        Switch = 3,

        Wire = 4

    }
    public class values
    {


        public Pair<float, bool> Current;
        public Pair<float, bool> Voltage;
        public Pair<float, bool> Resistance;

        public values()
        {
            Current = new Pair<float, bool>(0f, true);
            Voltage = new Pair<float, bool>(0f, true);
            Resistance = new Pair<float, bool>(0f, true);
        }

        public void updateValue(char c, float newValue)
        {
            switch (c)
            {
                case 'v':
                    Voltage.a = newValue;
                    break;
                case 'c':
                    Current.a = newValue;
                    break;
                case 'r':
                    Current.a = newValue;
                    break;
                default:
                    Debug.LogError("updateValue unknown " + c);
                    break;
            }
        }

        public void updateValue(char c, bool newValue)
        {
            switch (c)
            {
                case 'v':
                    Voltage.b = newValue;
                    break;
                case 'c':
                    Current.b = newValue;
                    break;
                case 'r':
                    Current.b = newValue;
                    break;
                default:
                    Debug.LogError("updateValue unknown " + c);
                    break;
            }
        }
    }



}
