using UnityEngine;
using System.Collections;

public class ButtonCode : MonoBehaviour 
{
	//private bool bShowGUI = false;
	public AudioClip examine;
	private bool hasPlayed = false;
	private bool onbutton = false;
	//public string dialogue;
	//public float textWidth = 200;
	//public float textHeight = 40;
	//public GUIText thewords;
	public GameObject indicator;
	public GameObject fallinggrimm;
	public GameObject chainblock;
	void Start()
	{
		indicator.gameObject.renderer.material.color = Color.red;
	}
	void OnTriggerEnter2D(Collider2D other) 
	{ 
		onbutton = true;
		//thewords.enabled = true;
	}
	void Update()
	{
		//Debug.Log(onbutton);
		if(onbutton)
		{
			if(Input.GetKeyDown(KeyCode.E))
			{
				indicator.gameObject.renderer.material.color = Color.green;
				DestroyObject(fallinggrimm);
				DestroyObject(chainblock);
			}
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		onbutton = false;
	}
	/*void OnTriggerExit2D(Collider2D other) 
	{ 
		bShowGUI = false;
		indicator.gameObject.renderer.material.color = Color.yellow;
		thewords.enabled = false;
		audio.Stop();
	}*/
	
}
