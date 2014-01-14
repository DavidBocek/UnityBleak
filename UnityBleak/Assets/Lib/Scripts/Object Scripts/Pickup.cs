using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	
	public int type;

	public void Grab(GameObject obj){
		gameObject.SetActive(false);
		obj.GetComponent<BleakInventoryManager>().GetPickup(type);
	}
}
