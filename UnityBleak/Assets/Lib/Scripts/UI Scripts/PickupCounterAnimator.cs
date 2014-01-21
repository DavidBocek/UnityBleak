using UnityEngine;
using System.Collections;

public class PickupCounterAnimator : MonoBehaviour {

	private int curValue = 0;
	private bool playingAnimation = false;
	private tk2dSpriteAnimator sprite;

	// Use this for initialization
	void Awake () {
		sprite = GetComponent<tk2dSpriteAnimator>();
	}
	
	public void ChangeCounterToValue(int newValue){
		if (newValue != curValue){
			//set up the transition array which we will iterate through to get the counter to animate through numbers to the correct number
			int[] transitions;
			if (newValue != 0 || curValue != 9){
				transitions = new int[Mathf.Abs(newValue-curValue)];
				for (int i=0; i<Mathf.Abs(newValue-curValue); i++){
					transitions[i] = curValue+i+1;
				}
				foreach (int i in transitions){
					StartCoroutine(PlayAnimation(curValue,i));
					curValue = i;
				}
			} else {
				StartCoroutine(PlayAnimation(9,0));
				curValue = 0;          
			}
		}
	}


	//TODO: we don't deal with the animation of decreasing right now, since we don't decrease in the demo.
	IEnumerator PlayAnimation(int cur, int next){
		sprite.Play(cur.ToString()+"to"+next.ToString());
		yield return new WaitForSeconds(.2f);
	}

}
