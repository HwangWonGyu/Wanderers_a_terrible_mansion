using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemGenerator))]
public class ItemGeneratorEditor : Editor
{

	public override void OnInspectorGUI()
	{
		ItemGenerator itemGenerator = (ItemGenerator)target;
		base.OnInspectorGUI();
		if (GUILayout.Button("Make"))
		{
			itemGenerator.MakeItem();
		}
		if (GUILayout.Button("Remove"))
		{
			itemGenerator.DeleteItem();
		}
		
	}
	
}
