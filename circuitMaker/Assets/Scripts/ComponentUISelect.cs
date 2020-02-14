using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentUISelect : MonoBehaviour
{
    public CircuitComponentBlueprint cell;
    // Update is called once per frame
	BuildManager buildManager;

	void Start ()
	{
		buildManager = BuildManager.instance;
	}


	public void SelectCellToBuild ()
	{
		Debug.Log("Cell Selected");
		buildManager.SelectCoponentToBuild(cell);
	}
}