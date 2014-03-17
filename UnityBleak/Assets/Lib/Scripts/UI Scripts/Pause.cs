using System;
using UnityEngine;

public class Pause : MonoBehaviour
{
	bool paused = false;
	public GUITexture pausetex;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
			paused = togglePause();
	}
	
	void OnGUI()
	{
		if(paused)
		{
			pausetex.enabled = true;
			GUILayout.Label("Pause");
			if(GUI.Button( new Rect(Screen.width/2-100,Screen.height*0.6f-25,200,50),"Quit"))
			{
				Application.Quit();
			}
			if(GUI.Button( new Rect(Screen.width/2-100,Screen.height*0.5f-25,200,50),"Return to Game"))
				paused = togglePause();
			
		}
		else
			pausetex.enabled = false;
	}
	
	bool togglePause()
	{
		if(Time.timeScale == 0f)
		{
			Time.timeScale = 1f;
			return(false);
		}
		else
		{
			Time.timeScale = 0f;
			return(true);    
		}
	}
}

/*if(GUI.Button( new Rect(Screen.width/2-45,Screen.height*.7-25,90,50),"Quit"))
{
	Application.Quit();
}*/