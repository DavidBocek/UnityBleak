using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BleakInventoryManager : MonoBehaviour {

	public GameObject inventory;
	public GameObject scrapCounter;
	public GameObject screwCounter;
	public GameObject gearCounter;

	public List<GameObject> inventoryItems;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetPickup(int type){
		switch (type){
		case 0:
			scrapCounter.GetComponent<PickupCounter>().Add(1);
			break;
		case 1:
			screwCounter.GetComponent<PickupCounter>().Add(1);
			break;
		case 2:
			gearCounter.GetComponent<PickupCounter>().Add(1);
			break;
		default:
			throw new UnityException("attempted to call GetPickup with a pickup value that was not valid");
		}
	}

	public void GetItem(GameObject obj){
		inventoryItems.Add(obj);
	}

	public void RemoveItem(GameObject obj){
		inventoryItems.Remove(obj);
	}

	public void UseItem(string itemType){
		if (itemType == "grimm_vial"){
			Messenger.Broadcast<Vector3>("GrimmVialUsed",transform.position);
		}
	}

	public List<string> GetInventoryNames(){
		List<string> inventoryItemNames = new List<string>();
		foreach (GameObject obj in inventoryItems){
			inventoryItemNames.Add(obj.GetComponent<Item>().name);
		}
		return inventoryItemNames;
	}

	public bool IsInventoryFull(){
		if (inventoryItems.Count >= 5){
			return true;
		}
		else return false;
	}
}
