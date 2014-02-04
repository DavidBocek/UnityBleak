using UnityEngine;
using System.Collections;

public class ConversationStartStopManager : MonoBehaviour {

	public BleakUIElement conversationUI;

	private BleakController controller;
	private BleakControllerTopDown controllerTD;

	// Use this for initialization
	void Start () {
		controller = GetComponent<BleakController>();
		controllerTD = GetComponent<BleakControllerTopDown>();
	}

	public void ConversationActivate(GameObject otherActorObj){
		StartCoroutine("BeginConversation",otherActorObj);
	}

	IEnumerator BeginConversation(GameObject otherActorObj){
		if (controller != null){
			controller.canControl = false;
		} else if (controllerTD != null){
			controllerTD.canControl = false;
		}
		conversationUI.OpenTree();
		yield return new WaitForSeconds(1.25f);
		otherActorObj.SendMessage("OnUse", this.transform, SendMessageOptions.DontRequireReceiver);
	}

	void OnConversationEnd(Transform otherActor){
		conversationUI.CloseTree();
		if (controller != null){
			controller.canControl = true;
		} else if (controllerTD != null){
			controllerTD.canControl = true;
		}
	}
}
