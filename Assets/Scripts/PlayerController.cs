using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
	public float		speed = 1;
	public PlayerHead	head;
	
	[Space]
	public float		hitDetectDistance = .12f;

	[Space]
	public float		timeBeforeStoppedDeath = 1;

	[HideInInspector]
	public bool			dead;

	[Space]
	public bool			direction = true;
	float				directionMultiplier
	{
		get
		{
			if (stop || dead)
				return 0;
			return (direction ? 1 : -1);
		}
	}

	public bool			stop = false;

	new Rigidbody2D	rigidbody;
	new Collider2D	collider;

	RaycastHit2D[]	results = new RaycastHit2D[4];
	ContactFilter2D	contactFilter = new ContactFilter2D();

	void Start ()
	{
		rigidbody = GetComponent< Rigidbody2D >();
		collider = GetComponent< CircleCollider2D >();

		contactFilter.useTriggers = false;
	}
	
	void Update ()
	{
		if (head.hit && !dead)
			Die();
	}

	void FixedUpdate()
	{
		Move();

		stop = false;
		
		int nCollision = collider.Cast(Vector2.right * directionMultiplier, contactFilter, results, .2f);

		for (int i = 0; i < nCollision; i++)
		{
			var col = results[i];

			if (col.collider.tag == "Player" || col.collider is TilemapCollider2D)
				continue ;
			else
			{
				stop = true;
				StartCoroutine("DieIfStopped");
				break ;
			}
		}

		if (stop == false)
			StopCoroutine("DieIfStopped");
	}

	IEnumerator DieIfStopped()
	{
		yield return new WaitForSeconds(timeBeforeStoppedDeath);

		if (stop == false && !dead)
			Die();
	}

	void Die()
	{
		dead = true;
		Debug.Log("DEAD !");
	}

	void Move()
	{
		Vector2 v = rigidbody.velocity;

		v.x = speed * directionMultiplier;

		rigidbody.velocity = v;
	}
}
