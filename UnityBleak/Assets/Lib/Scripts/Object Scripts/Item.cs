using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public string iconSpriteName;
	public string itemType;
	public AudioClip taken;
	
	public void Grab(GameObject obj){
		//AudioSource.PlayClipAtPoint(taken, transform.position,1);
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<Collider2D>().enabled = false;
		if (!obj.GetComponent<BleakInventoryManager>().IsInventoryFull()){
			obj.GetComponent<BleakInventoryManager>().GetItem(gameObject);
		}
	}

}
