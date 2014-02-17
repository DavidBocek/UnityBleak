using UnityEngine;
using System.Collections;

public class DestroyOnTriggerEnter : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Destroy(gameObject);
	}

}
