using UnityEngine;
using System.Collections;
using static Utilities;


public class ComponentUISelect : MonoBehaviour
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