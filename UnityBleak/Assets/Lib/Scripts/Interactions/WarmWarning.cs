using UnityEngine;
using System.Collections;

public class WarmWarning : MonoBehaviour 
{
	private bool bShowGUI = false;
	public GUIText thewords;
	public GameObject indicator;
	//public float textWidth = 4;
	//public float textHeight = 20;
	
	void OnTriggerEnter2D(Collider2D other) 
	{ 
		bShowGUI = true;
		indicator.gameObject.renderer.material.color = Color.yellow;
		thewords.enabled = true;
	}
	void OnTriggerExit2D(Collider2D other) 
	{ 
		bShowGUI = false;
		indicator.gameObject.renderer.material.color = Color.clear;
		thewords.enabled = false;
	}
	void OnGUI() 
	{ 
		if(bShowGUI) //DrawButton/Label etc
		{
			thewords.text = "Warm";
			//GUI.Label (new Rect (Screen.width/2-textWidth/2, Screen.height/4, textWidth, textHeight), "Warm");
		}
	}
	/*void Update()
	{
		if(bShowGUI)
		{
			indicator.renderer.material.color = new Color(1,0.5,0.4);
		}
		else
			indicator.renderer.material.color = new Color(148,191,146);
	
	}*/
}