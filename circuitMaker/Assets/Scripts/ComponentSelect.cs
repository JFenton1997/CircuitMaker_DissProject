using UnityEngine;
using System.Collections;
using Utilities;


public class ComponentSelect : MonoBehaviour
{
	[SerializeField]
    public  CircuitComponentBlueprint Cell;
	[SerializeField]
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
}