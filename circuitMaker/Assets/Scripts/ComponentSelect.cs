using UnityEngine;
using System.Collections;
using Utilities;


public class ComponentSelect : MonoBehaviour
{

    public CircuitComponentBlueprint Cell;

    public CircuitComponentBlueprint Wire;

    public CircuitComponentBlueprint Resistor;




    BuildManager buildManager;

    void Start()
    {
        buildManager = BuildManager.instance;
    }


    public void SelectCellToBuild()
    {
        buildManager.SelectCoponentToBuild(Cell);
    }

    public void SelectWireToBuild()
    {
        buildManager.SelectCoponentToBuild(Wire);
    }

    public void SelectResistorToBuild()
    {
        buildManager.SelectCoponentToBuild(Resistor);
    }
}