using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
	public GameObject	waterPrefab;
	public Vector2		spawnTimerRange = new Vector2(.2f, 1);
	public float		spawnForce = 5;

	void Start ()
	{
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(spawnTimerRange.x, spawnTimerRange.y));
			var g = GameObject.Instantiate(waterPrefab, transform.position, Quaternion.identity);
			g.GetComponent< Rigidbody2D >().AddForce(transform.right * spawnForce, ForceMode2D.Force);
		}
	}
}
