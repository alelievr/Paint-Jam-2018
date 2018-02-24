using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionInverter : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player" && other.gameObject.layer == LayerMask.NameToLayer("PlayerFoot"))
		{
			PlayerController pc = other.GetComponent< PlayerController >();

			pc.direction = !pc.direction;
		}
	}
}
