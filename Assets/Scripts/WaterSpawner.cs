using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
	public GameObject	waterPrefab;
	public float		spawnPerSeconds = 10;
	public float		spawnForce = 5;

	void Start ()
	{
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn()
	{
		while (true)
		{
			yield return new WaitForSeconds(1 / spawnPerSeconds);
			var g = GameObject.Instantiate(waterPrefab, transform.position, Quaternion.identity);
			g.transform.localScale = Vector3.one * .10f;
			g.GetComponent< Rigidbody2D >().AddForce(transform.right * spawnForce, ForceMode2D.Force);
		}
	}
}
