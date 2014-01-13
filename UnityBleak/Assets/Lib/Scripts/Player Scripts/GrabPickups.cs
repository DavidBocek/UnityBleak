using UnityEngine;
using System.Collections;

public class GrabPickups : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Pickup pickup = other.gameObject.GetComponent<Pickup>();
		if (pickup){
			pickup.Grab(gameObject);
		}
	}

}
