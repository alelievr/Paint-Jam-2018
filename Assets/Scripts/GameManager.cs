using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
	public	enum		GameState
	{
		Play,
		Pause,
		End
	}

	public static GameManager	instance;
	public GameObject			menuPause;

	public	int			level;
	public GameState	gameState;

	void Awake()
	{
		instance = this;
	}

	void Start ()
	{
		Analytics.CustomEvent("level_start", new Dictionary< string, object >()
		{
			{"name", SceneManager.GetActiveScene().name}
		});
		gameState = GameState.Pause;
		menuPause.SetActive(false);
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.R))
			Restart();
	}

	public void Play()
	{
		gameState = GameState.Play;
	}

	public void Restart() {
		Analytics.CustomEvent("level_fail", new Dictionary< string, object >()
		{
			{"name", SceneManager.GetActiveScene().name}
		});
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Pause () {
		if (gameState != GameState.Pause) {
			gameState = GameState.Pause;
			menuPause.SetActive(true);
		}
		else {
			gameState = GameState.Play;
			menuPause.SetActive(false);
		}
	}

	public void Win () {
		Analytics.CustomEvent("level_complete", new Dictionary< string, object >()
		{
			{"name", SceneManager.GetActiveScene().name}
		});
		gameState = GameState.End;
		Debug.Log("Level" + (level + 1).ToString());
		StartCoroutine(LoadNext());
	}

	public void LoadLevel (string lvl) {
		SceneManager.LoadScene("Level" + (int.Parse(lvl) + 1));
	}
	
	IEnumerator LoadNext () {
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("Level" + (level + 1).ToString());
	}
}
