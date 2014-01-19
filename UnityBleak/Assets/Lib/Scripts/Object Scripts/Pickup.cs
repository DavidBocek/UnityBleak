using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	
	public int type;
	public AudioClip taken;

	public void Grab(GameObject obj){
		AudioSource.PlayClipAtPoint(taken, transform.position,1);
		gameObject.SetActive(false);
		obj.GetComponent<BleakInventoryManager>().GetPickup(type);
	}

}
