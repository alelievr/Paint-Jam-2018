using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
	[HideInInspector]
	public bool		hit = false;

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.tag != "Player")
			hit = true;
	}
}
