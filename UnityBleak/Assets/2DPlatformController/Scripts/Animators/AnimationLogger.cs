using UnityEngine;
using System.Collections;

/// <summary>
/// Logs the animation state to the console.
/// </summary>
public class AnimationLogger : MonoBehaviour {
	
	public RaycastCharacterController controller;
	
	void Start(){
		if (controller == null) controller = GetComponent<RaycastCharacterController>();
		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
		
	}
	
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		Debug.Log(state + "(" + previousState + ")");	
	}
}
