using UnityEngine;
using System.Collections;

public class GrabItems : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Item item = other.gameObject.GetComponent<Item>();
		if (item){
			GetComponentInChildren<SkeletonAnimation>().state.SetAnimation(0,"pick up",false);
			item.Grab(gameObject);
		}
	}

}
