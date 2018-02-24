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

	public void Win ()
	{
		gameState = GameState.End;
		LoadNext();
	}
	
	IEnumerator LoadNext()
	{
		yield return new WaitForSeconds(5f);
		SceneManager.LoadScene("Level" + (level + 1).ToString());
	}
}
