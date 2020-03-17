using UnityEngine;
using System.Collections;


public class ComponentUISelect : MonoBehaviour
{
    public  CircuitComponentBlueprint Cell;
	public  CircuitComponentBlueprint Wire;


	BuildManager buildManager;

	void Start ()
	{
		buildManager = BuildManager.instance;
	}


	public void SelectCellToBuild ()
	{
		buildManager.SelectCoponentToBuild(Cell);
	}

	public void SelectWireToBuild (){
		buildManager.SelectCoponentToBuild(Wire);
	}
    public CircuitComponentBlueprint getWire() => Wire;
}