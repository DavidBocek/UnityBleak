using UnityEngine;
using System.Collections;

public class LerpToPoint : MonoBehaviour {
	
	public float activateRadius;
	public Transform activateLocation;
	public Transform destinationLocation;
	
	// Use this for initialization
	void Start () {
		Messenger.AddListener<Transform,BleakController>("BleakActionActivate",HandleBleakAction);
		Messenger.AddListener<Transform,BleakController>("CompletedForceMove",HandleFinishMove);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void HandleBleakAction(Transform actionPos, BleakController controller){
		if ((actionPos.position - activateLocation.position).magnitude <= activateRadius){
			controller.SetState (BleakController.STATE_FORCE_MOVE);
			controller.forceMoveDestination = destinationLocation;
		}
	}
	
	void HandleFinishMove(Transform dest, BleakController controller){
		if ((dest.position - destinationLocation.position).sqrMagnitude <= .1f){
			controller.SetState(BleakController.STATE_NORMAL);
			Messenger.Broadcast<LerpToPoint>("LerpToPointCompletionAction",this);
		}
	}
}
