using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour
{
	public float		speed;
	public float		turbulances = 1;
	public float		speedRange = .2f;
	public float		detectDist = 5;

	Vector2				direction;

	float				x1;
	float				x2;

	void Start ()
	{
		direction = Vector2.right;
		x1 = Random.Range(-1000f, 1000f);
		x2 = Random.Range(-1000f, 1000f);
	}
	
	void FixedUpdate ()
	{
		float d1 = Mathf.PerlinNoise(x1, Time.timeSinceLevelLoad * turbulances);
		float d2 = Mathf.PerlinNoise(x2, Time.timeSinceLevelLoad * turbulances);

		direction = new Vector2(d1 - .5f, d2 - .5f).normalized;
		direction *= 1 + (Mathf.PingPong(Time.timeSinceLevelLoad, speedRange * 2) - speedRange);

		var hit = Physics2D.Raycast(transform.position, direction, detectDist, 1);

		if (hit.collider != null)
		{
			direction *= (hit.distance / detectDist) - .1f;
			Debug.DrawRay(transform.position, direction, Color.red, .1f);
		}

		transform.position += (Vector3)direction * speed * 0.01f;
	}
}
