using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float		speed = 1;
	public PlayerHead	head;

	[HideInInspector]
	public bool			dead;

	new Rigidbody2D	rigidbody;

	void Start ()
	{
		rigidbody = GetComponent< Rigidbody2D >();
	}
	
	void Update ()
	{
		if (head.hit && !dead)
		{
			dead = true;
			Debug.Log("DEAD !");
		}
	}

	void FixedUpdate()
	{
		Vector2 v = rigidbody.velocity;

		v.x = speed;

		rigidbody.velocity = v;
	}
}
