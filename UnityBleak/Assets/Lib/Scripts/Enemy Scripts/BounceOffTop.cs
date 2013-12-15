using UnityEngine;
using System.Collections;
using Spine;
using System;

public class BounceOffTop : MonoBehaviour {
	
	public int springPower;
	public float springResetDelay;

	SkeletonAnimation skeletonAnimation;

	private float springResetTimer;
	private bool isSprung;
	private float dt;
	
	void Start(){
		springResetTimer = springResetDelay;
		skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
	}
	
	public bool IsSprung(){
		return isSprung;
	}
	
	public void SetSprung(bool b){
		isSprung = b;
	}

	public void PlaySpringAnimation(){
		skeletonAnimation.state.AddAnimation(0,"spring",false,0.1f);
	}
	
	void Update(){
		dt = Time.deltaTime;
		if (isSprung){
			if (springResetTimer <= 0){
				isSprung = false;
				springResetTimer = springResetDelay;
			} else {
				springResetTimer -= dt;
			}
		}
	}
}
