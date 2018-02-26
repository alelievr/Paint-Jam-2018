using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
	public GameObject	blueScreen;
	public GameObject	spaceToStart;

	void Start()
	{
		DisplaySpaceToStart();
	}

	public void			DisplayBlueScreen()
	{
		blueScreen.SetActive(true);
	}

	public void			DisplaySpaceToStart()
	{
		spaceToStart.SetActive(true);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
		{
			blueScreen.SetActive(false);
			spaceToStart.SetActive(false);
		}
	}
}
