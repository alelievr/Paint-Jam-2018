using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBumper : MonoBehaviour
{
	public AudioSource boingSound;
	[Space]
	public float	bumperPower = 1;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player" && other.gameObject.layer == LayerMask.NameToLayer("PlayerFoot"))
		{
			Rigidbody2D rb = other.GetComponent< Rigidbody2D >();

			Vector2 v = rb.velocity;
			v.y = 0;
			rb.velocity = v;
			boingSound.Play();
			rb.AddForce(Vector2.up * bumperPower, ForceMode2D.Impulse);
		}
	}
}
