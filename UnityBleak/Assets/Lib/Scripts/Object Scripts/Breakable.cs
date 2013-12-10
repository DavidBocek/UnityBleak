using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
	
	public bool needsSlam;
	public bool breakableUp;
	public bool breakableDown;
	public tk2dAnimatedSprite anim;
	public float breakTime;
	public bool isBreaking {get; set;}

	private float breakTimer = 0.0f;

	void Start(){
		isBreaking = false;
	}

	void Update(){
		if (isBreaking){
			if (breakTimer < breakTime){
				breakTimer += Time.deltaTime;
				Debug.Log ("breaking...");
			} else {
				transform.gameObject.SetActive(false);
				Debug.Log ("broken!");
			}
		}
	}

	public void Break(){
		isBreaking = true;
		//anim.Play("break");
		Debug.Log("BREAK!");
	}

}
