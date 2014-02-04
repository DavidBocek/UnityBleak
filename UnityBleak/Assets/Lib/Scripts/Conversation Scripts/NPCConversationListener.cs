using UnityEngine;
using System.Collections;

public class NPCConversationListener : MonoBehaviour {

	public float maxDistance;

	// Use this for initialization
	void Start () {
		Messenger.AddListener<Transform,BleakController>("BleakInteractionActivate",HandleConversationStart);
		Messenger.AddListener<Transform,BleakControllerTopDown>("BleakInteractionActivateTopDown",HandleConversationStartTopDown);
	}

	void OnDestroy(){
		Messenger.RemoveListener<Transform,BleakController>("BleakInteractionActivate",HandleConversationStart);
		Messenger.RemoveListener<Transform,BleakControllerTopDown>("BleakInteractionActivateTopDown",HandleConversationStartTopDown);
	}

	void HandleConversationStart(Transform trans, BleakController controller){
		HandleConversationStartGameObject(trans.position,controller.gameObject);
	}

	void HandleConversationStartTopDown(Transform trans, BleakControllerTopDown controller){
		HandleConversationStartGameObject(trans.position,controller.gameObject);
	}

	void HandleConversationStartGameObject(Vector3 pos, GameObject playerObj){
		if (Vector3.Distance(pos,transform.position)  <= maxDistance){
			playerObj.GetComponent<ConversationStartStopManager>().ConversationActivate(gameObject);
		}
	}
}
