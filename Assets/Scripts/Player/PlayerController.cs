using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
	public float		speed = 1;
	public PlayerHead	head;

	public Collider2D	groundCollider;
	
	[Space]
	public float		hitDetectDistance = .12f;
	public float		jumpDetectDistance = .2f;
	public float		jumpTimeout = .5f;
	public float		jumpPower = .5f;

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
			if (stop || dead || paused)
				return 0;
			return (direction ? 1 : -1);
		}
	}

	public bool			stop = false;

	[Space]
	public float		groundRaycast = .1f;
	public float		middleRaycast = .2f;
	public float		headRaycast = .4f;

	bool			paused
	{
		get
		{
			return (GameManager.instance != null && GameManager.instance.gameState == GameManager.GameState.Pause);
		}
	}

	bool			canJump = true;

	new Rigidbody2D	rigidbody;
	new Collider2D	collider;

	RaycastHit2D[]	results = new RaycastHit2D[4];
	Collider2D[]	overlapResults = new Collider2D[10];
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

		if (WillJump() && IsGrounded())
			Jump();

		// DetectCollisions();
	}

	void Jump()
	{
		if (canJump == false)
			return ;
		
		rigidbody.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
		
		canJump = false;
		StartCoroutine(ResetCanJump());
	}

	IEnumerator ResetCanJump()
	{
		yield return new WaitForSeconds(jumpTimeout);
		canJump = true;
	}

	bool WillJump()
	{
		RaycastHit2D hitGround, hitMid, hitHead;
		Vector2 pos = transform.position;
		Vector2 playerOffset = new Vector2(.15f * directionMultiplier, 0);
		Vector2 groundPos = pos + new Vector2(0, groundRaycast) + playerOffset;
		Vector2 midPos = pos + new Vector2(0, middleRaycast) + playerOffset;
		Vector2 headPos = pos + new Vector2(0, headRaycast) + playerOffset;

		int layerMask = 1; //Default

		hitGround = Physics2D.Raycast(groundPos, Vector2.right * directionMultiplier, jumpDetectDistance, layerMask);
		Debug.DrawRay(groundPos, Vector3.right * directionMultiplier * jumpDetectDistance, Color.red);
		hitMid = Physics2D.Raycast(midPos, Vector2.right * directionMultiplier, jumpDetectDistance, layerMask);
		Debug.DrawRay(midPos, Vector3.right * directionMultiplier * jumpDetectDistance, Color.red);
		hitHead = Physics2D.Raycast(headPos, Vector2.right * directionMultiplier, jumpDetectDistance, layerMask);
		Debug.DrawRay(headPos, Vector3.right * directionMultiplier * jumpDetectDistance, Color.red);

		// Debug.Log("hitHead: " + hitHead.collider);
		// Debug.Log("hitMid: " + hitMid.collider);
		// Debug.Log("hitGround " + hitGround.collider);

		if (hitHead.collider != null)
			return false;
		return hitGround.collider != null || hitMid.collider != null;
	}

	bool IsGrounded()
	{
		if (!canJump)
			return false;
		
		if (rigidbody.velocity.y > .1f || rigidbody.velocity.y < -.1f)
			return false;
		
		int n = groundCollider.OverlapCollider(contactFilter, overlapResults);

		bool ground = false;
		for (int i = 0; i < n; i++)
		{
			var col = overlapResults[i];

			if (col.tag == "Player")
				continue ;
			ground = true;
		}

		return ground;
	}

	void DetectCollisions()
	{
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
