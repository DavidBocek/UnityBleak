using UnityEngine;
using System.Collections;

/// <summary>
/// Logs the animation state to the console.
/// </summary>
public class AnimationLogger2D : MonoBehaviour {
	
	public RaycastCharacterController2D controller;
	
	void Start(){
		if (controller == null) controller = GetComponent<RaycastCharacterController2D>();
		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController2D.CharacterControllerEventDelegate (CharacterAnimationEvent);
		
	}
	
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		Debug.Log(state + "(" + previousState + ")");	
	}
}
