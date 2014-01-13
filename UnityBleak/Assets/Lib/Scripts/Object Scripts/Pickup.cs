using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	public short type;

	public void Grab(GameObject obj){
		gameObject.SetActive(false);
		obj.GetComponent<BleakInventoryManager>().GetPickup(type);
	}
}
