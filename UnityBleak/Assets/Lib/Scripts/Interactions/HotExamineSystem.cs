using UnityEngine;
using System.Collections;

public class HotExamineSystem : MonoBehaviour 
{
	private bool bShowGUI = false;
	public AudioClip examine;
	private bool hasPlayed = false;
	public string dialogue;
	//public float textWidth = 200;
	//public float textHeight = 40;
	public GUIText thewords;
	public GameObject indicator;
	
	void OnTriggerEnter2D(Collider2D other) 
	{ 
		bShowGUI = true;
		indicator.gameObject.renderer.material.color = Color.red;
		//thewords.enabled = true;
	}
	void OnTriggerExit2D(Collider2D other) 
	{ 
		bShowGUI = false;
		indicator.gameObject.renderer.material.color = Color.yellow;
		thewords.enabled = false;
		audio.Stop();
	}
	void OnGUI() 
	{ 
		if(bShowGUI) //DrawButton/Label etc
		{
			thewords.enabled = true;
			thewords.text = dialogue;
			//GUI.Label (new Rect (Screen.width/2-textWidth/2, Screen.height/4, textWidth, textHeight), dialogue);
			if(Input.GetKeyDown(KeyCode.E))
			{
				if(hasPlayed == false)
				{
					hasPlayed = true;
					audio.clip = examine;
					audio.Play();
				}
				else
					hasPlayed = false;
			}
		}
	}

}
