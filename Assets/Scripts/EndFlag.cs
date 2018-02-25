using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndFlag : MonoBehaviour {

	public	GameManager GM;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("arrived");
			SpriteRenderer sprite =  gameObject.GetComponent<SpriteRenderer>();
			sprite.enabled = false;
			GM.Win();
		}
	}
}
