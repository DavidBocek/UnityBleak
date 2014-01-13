using UnityEngine;
using System.Collections;

public class BleakInventoryManager : MonoBehaviour {

	public GameObject inventory;
	public GameObject scrapCounter;
	public GameObject screwCounter;
	public GameObject gearCounter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetPickup(short type){
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
}
