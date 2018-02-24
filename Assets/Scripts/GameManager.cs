using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public	int			level;
	public	enum		gameStates {Play, Pause, End};
	public gameStates	gameState;

	// Use this for initialization
	void Start () {
		gameState = gameStates.Pause;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameState == gameStates.Play) {
		  	
		}
	}

	public void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Win () {
		gameState = gameStates.End;
		Debug.Log("Level" + (level + 1).ToString());
		StartCoroutine(LoadNext());
	}
	
	IEnumerator LoadNext() {
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("Level" + (level + 1).ToString());
	}
}
