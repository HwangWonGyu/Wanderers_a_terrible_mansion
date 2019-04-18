using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(EnemyController))]
public class FieldIOfViewEditor : Editor {

	private void OnSceneGUI()
	{
		EnemyController fow = (EnemyController)target;
		Handles.color = Color.white;
		Handles.DrawWireArc(fow.viewTransform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
		Handles.color = Color.blue;
		Handles.DrawWireArc(fow.viewTransform.position, Vector3.up, Vector3.forward, 360, fow.hearRadius);
		Handles.color = Color.white;

		Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
		Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

		Handles.DrawLine(fow.viewTransform.position, fow.viewTransform.position + viewAngleA * fow.viewRadius);
		Handles.DrawLine(fow.viewTransform.position, fow.viewTransform.position + viewAngleB * fow.viewRadius);

		Handles.color = Color.red;
		if (fow.player != null)
		{
			Handles.DrawLine(fow.viewTransform.position, fow.player.position); 
		}

	}
	
}
