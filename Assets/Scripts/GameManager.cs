using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public	enum		GameState
	{
		Play,
		Pause,
		End
	}

	public static GameManager	instance;

	public	int			level;
	public GameState	gameState;

	void Awake()
	{
		instance = this;
	}

	void Start ()
	{
		gameState = GameState.Pause;
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		
		if (gameState == GameState.Play)
		{
		  	
		}
	}

	public void Play()
	{
		gameState = GameState.Play;
	}

	public void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Win () {
		gameState = GameState.End;
		Debug.Log("Level" + (level + 1).ToString());
		StartCoroutine(LoadNext());
	}
	
	IEnumerator LoadNext() {
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("Level" + (level + 1).ToString());
	}
}
