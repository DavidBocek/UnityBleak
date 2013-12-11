using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
	
	public bool needsSlam;
	public bool breakableUp;
	public bool breakableDown;
	private tk2dSpriteAnimator anim;
	public AudioClip platform;
	public float breakTime;
	public bool isBreaking {get; set;}

	private float breakTimer = 0.0f;

	void Start(){
		isBreaking = false;
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	void Update(){
		if (isBreaking){
			if (breakTimer < breakTime){
				breakTimer += Time.deltaTime;
				/Debug.Log ("breaking...");
				audio.PlayOneShot(platform);
				anim.Play("Breaking");
			} else {
				transform.gameObject.SetActive(false);
				//Debug.Log ("broken!");
			}
		}
	}

	public void Break(){
		isBreaking = true;
		//Debug.Log("BREAK!");
	}

}
