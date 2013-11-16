using UnityEngine;
using System.Collections;

public class ChangeLevels : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Messenger.AddListener<LerpToPoint>("LerpToPointCompletionAction",HandleLerpToPointCompletion);
	}
	
	void HandleLerpToPointCompletion(LerpToPoint completedEvent){
		//change levels
	}
}
