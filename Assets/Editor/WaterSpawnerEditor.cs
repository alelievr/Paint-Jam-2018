using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaterSpawner))]
public class WaterSpawnerEditor : Editor
{
	WaterSpawner	ws;

	public void OnEnable()
	{
		ws = (WaterSpawner)target;
	}

	void OnSceneGUI()
	{
		Quaternion rot = Quaternion.LookRotation(ws.transform.right);
		Handles.ArrowHandleCap(0, ws.transform.position, rot, 1, Event.current.type);
	}
}
