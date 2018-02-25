using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
	public GameObject	playerCamera;
	public GameObject	previewCamera;

	CinemachineBrain	brain;

	bool				cameraSwitched = false;

	void Start ()
	{
		playerCamera.SetActive(false);
		brain = GetComponentInChildren< CinemachineBrain >();
	}
	
	void Update ()
	{
		if (!brain.IsBlending && brain.ActiveVirtualCamera.Name == "PlayerCamera" && !cameraSwitched)
		{
			GameManager.instance.Play();
			cameraSwitched = true;
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			playerCamera.SetActive(true);
			previewCamera.SetActive(false);
		}
	}
}
