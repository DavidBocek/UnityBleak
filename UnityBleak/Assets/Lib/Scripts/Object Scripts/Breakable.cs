using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
	
	public bool needsSlam;
	public bool breakableUp;
	public bool breakableDown;
	private tk2dSpriteAnimator anim;
	public AudioClip breakingSound;
	public float breakTime;
	public bool isBreaking {get; set;}
	public AudioClip standingSound;
	public AudioClip jumpingSound;

	private bool hasBreakingSound;
	private bool hasStandingSound;
	private bool hasJumpingSound;
	private float breakTimer = 0.0f;

	void Start(){
		if (breakingSound != null) hasBreakingSound = true;
		if (standingSound != null) hasStandingSound = true;
		if (jumpingSound != null) hasJumpingSound = true;
		isBreaking = false;
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	void Update(){
		if (isBreaking){
			if (breakTimer < breakTime){
				breakTimer += Time.deltaTime;
				//Debug.Log ("breaking...");
				anim.Play("Breaking");
			} else {
				transform.gameObject.SetActive(false);
				//Debug.Log ("broken!");
			}
		}
	}

	public void Break(){
		isBreaking = true;
		if (hasBreakingSound) audio.PlayOneShot(breakingSound);
		//Debug.Log("BREAK!");
	}

	public void JumpSound(){
		if (isBreaking) return;
		if (hasJumpingSound) audio.PlayOneShot(jumpingSound);
	}

	private bool playingSound = false;
	public void StandSound(){
		if (isBreaking) return;
		if (!playingSound) StartCoroutine("cPlayStandingSound");
	}

	IEnumerator cPlayStandingSound(){
		playingSound = true;
		audio.PlayOneShot(standingSound);
		yield return new WaitForSeconds(standingSound.length+.5f);
		playingSound = false;
	}

}
