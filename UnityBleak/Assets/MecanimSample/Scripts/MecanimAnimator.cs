using UnityEngine;
using System.Collections;

public class MecanimAnimator : MonoBehaviour {
	
	public RaycastCharacterController controller;
	public Animator animator;
	private CharacterState currentState;
	private bool isFirstFrame;
	private int currentDirection;
	private float timer;
	private bool doReset;
	private Quaternion targetRotation;
	
	void Start(){
		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
		animator.feetPivotActive = 1.0f;
	}
	
	void Update() {
		timer += RaycastCharacterController.FrameTime;
		animator.SetFloat("VelocityX", Mathf.Abs(controller.Velocity.x));
		animator.SetFloat("VelocityY", Mathf.Abs(controller.Velocity.y));
		animator.SetBool("FirstFrame", isFirstFrame);
		animator.SetFloat("Timer", timer);
		if (isFirstFrame) isFirstFrame = false;
		CheckDirection();
		transform.localRotation = Quaternion.RotateTowards (transform.localRotation, targetRotation, Time.deltaTime * 400.0f);
	}
	
	void LateUpdate() {
		if (doReset) {
			animator.GetBoneTransform(HumanBodyBones.Spine).parent.parent.localPosition = Vector3.zero;
			// Depending on structure you may need to use this line instead
			// animator.GetBoneTransform(HumanBodyBones.Spine).parent.localPosition = Vector3.zero;
			doReset = false;
		}
	}
	
	/// <summary>
	/// Respond to an animation event.
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	/// <param name='previousState'>
	/// Previous state.
	/// </param>
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		currentState = state;
		animator.SetInteger("State", (int)state);
		animator.SetInteger("PreviousState", (int)previousState);
		isFirstFrame = true;
		timer = 0.0f;
		// Add any special case code here.
		switch (state) {
			case CharacterState.LEDGE_CLIMB_FINISHED: doReset = true; break;
		}
		switch (previousState) {
			case CharacterState.CLIMB_TOP_OF_LADDER_UP:  doReset = true; break;
		}
	}
	
	protected void CheckDirection() {
		int tmpFirection = currentDirection;
		currentDirection = controller.CurrentDirection;
		// Rope states
		if (currentState == CharacterState.ROPE_HANGING ||
		    currentState == CharacterState.ROPE_CLIMBING){
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 00.0f, 0.0f);
			
		}
		// Climbing states
		else if (currentState ==  CharacterState.CLIMBING ||
		         currentState ==  CharacterState.HOLDING ||
				 currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_UP ||
				 currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_DOWN) {
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 0.0f, 0.0f);
		}
		// Wall slide
		else if (currentState == CharacterState.WALL_SLIDING) {
			// You might need to switch 270 and 90 for other values depending on orientation of your model
			if (controller.CurrentDirection > 0 ) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 270.0f, 0.0f);
			} else if (controller.CurrentDirection < 0) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, -270.0f, 0.0f);
			}	
		}
		// Directional states
		else {
			
			// You might need to switch 270 and 90 for other values depending on orientation of your model
			if (controller.CurrentDirection > 0 ) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, -270.0f, 0.0f);
			} else if (controller.CurrentDirection < 0) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 270.0f, 0.0f);
			}
		}
	}
}