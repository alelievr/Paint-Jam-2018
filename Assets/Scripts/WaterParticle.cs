using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticle : MonoBehaviour
{

	public float		lifeTime = 40;

	void Start ()
	{
		StartCoroutine(DespawnAfter(lifeTime));
	}

	IEnumerator	DespawnAfter(float secs)
	{
		yield return new WaitForSeconds(secs);

		float t = Time.time;
		float startScale = transform.localScale.x;

		while (Time.time - t < 1)
		{
			float a = (1 - (Time.time - t)) * startScale;
			transform.localScale = Vector3.one * a;
		}
	}
	
	void OnBecameInvisible()
	{
		Destroy(gameObject);
	}
}
