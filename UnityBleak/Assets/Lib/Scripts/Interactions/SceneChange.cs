using UnityEngine;
using System.Collections;

public class SceneChange : MonoBehaviour {

	public string sceneName;
	private bool OnDoor = false;

void OnTriggerEnter2D(Collider2D other)
	{
		GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().numLivesPlayer = GameObject.FindWithTag("Player").GetComponent<BleakController>().numLives;
		OnDoor = true;
	}
	
void Update () 
	{
		if(OnDoor == true)
		{
			Application.LoadLevel(sceneName);
		}
	}
}