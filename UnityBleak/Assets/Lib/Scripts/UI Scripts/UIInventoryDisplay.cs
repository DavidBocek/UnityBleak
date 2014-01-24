using UnityEngine;
using System.Collections;

public class UIInventoryDisplay : MonoBehaviour {

	public tk2dSprite displayLocation0;
	public tk2dSprite displayLocation1;
	public tk2dSprite displayLocation2;
	public tk2dSprite displayLocation3;
	public tk2dSprite displayLocation4;

	private BleakInventoryManager inventoryManager;

	// Use this for initialization
	void Start () {
		inventoryManager = GameObject.FindWithTag("Player").GetComponent<BleakInventoryManager>();
		displayLocation0.gameObject.SetActive(false);
		displayLocation1.gameObject.SetActive(false);
		displayLocation2.gameObject.SetActive(false);
		displayLocation3.gameObject.SetActive(false);
		displayLocation4.gameObject.SetActive(false);
	}
	
	IEnumerator Open(){
		yield return new WaitForSeconds(1.5f);
		int loc = 0;
		for (int i=0; i<inventoryManager.inventoryItems.Count; i++){
			Item item = inventoryManager.inventoryItems[i].GetComponent<Item>();
			switch (loc){
			case 0:
				displayLocation0.gameObject.SetActive(true);
				displayLocation0.SetSprite(item.iconSpriteName);
				break;
			case 1:
				displayLocation1.gameObject.SetActive(true);
				displayLocation1.SetSprite(item.iconSpriteName);
				break;
			case 2:
				displayLocation2.gameObject.SetActive(true);
				displayLocation2.SetSprite(item.iconSpriteName);
				break;
			case 3:
				displayLocation3.gameObject.SetActive(true);
				displayLocation3.SetSprite(item.iconSpriteName);
				break;
			case 4:
				displayLocation4.gameObject.SetActive(true);
				displayLocation4.SetSprite(item.iconSpriteName);
				break;
			}
		}
	}

	void Close(){
		displayLocation0.gameObject.SetActive(false);
		displayLocation1.gameObject.SetActive(false);
		displayLocation2.gameObject.SetActive(false);
		displayLocation3.gameObject.SetActive(false);
		displayLocation4.gameObject.SetActive(false);
	}

	public void UISelect(int index){
		if (inventoryManager.inventoryItems.Count <= index){
			return;
		}
		inventoryManager.UseItem(inventoryManager.inventoryItems[index].GetComponent<Item>().itemType);
		GetComponent<BleakUIElement>().CloseTree ();
		inventoryManager.RemoveItem(inventoryManager.inventoryItems[index]);
	}

	void RemoveSpriteAtLocation(int index){
		switch (index){
		case 0:
			displayLocation0.gameObject.SetActive(true);
			break;
		case 1:
			displayLocation1.gameObject.SetActive(true);
			break;
		case 2:
			displayLocation2.gameObject.SetActive(true);
			break;
		case 3:
			displayLocation3.gameObject.SetActive(true);
			break;
		case 4:
			displayLocation4.gameObject.SetActive(true);
			break;
		}
	}
}
