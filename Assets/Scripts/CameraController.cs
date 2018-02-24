using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
	public GameObject	playerCamera;
	public GameObject	previewCamera;

	void Start ()
	{
		playerCamera.SetActive(false);
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			playerCamera.SetActive(true);
			previewCamera.SetActive(false);
		}
	}
}
