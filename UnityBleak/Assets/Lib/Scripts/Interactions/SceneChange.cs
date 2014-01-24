using UnityEngine;
using System.Collections;

public class SceneChange : MonoBehaviour {

	public string sceneName;
	private bool OnDoor = false;

void OnTriggerEnter2D(Collider2D other)
	{
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