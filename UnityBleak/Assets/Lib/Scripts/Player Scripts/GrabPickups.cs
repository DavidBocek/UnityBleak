﻿using UnityEngine;
using System.Collections;

public class GrabPickups : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Pickup pickup = other.gameObject.GetComponent<Pickup>();
		if (pickup){
			GetComponentInChildren<SkeletonAnimation>().state.SetAnimation(0,"pick up",false);
			pickup.Grab(gameObject);
		}
	}

}
