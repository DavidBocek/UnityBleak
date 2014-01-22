using UnityEngine;
using System.Collections;

public class GrabItems : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Item item = other.gameObject.GetComponent<Item>();
		if (item){
			GetComponent<BleakController>().PickUpItem(item);
		}
	}

}
