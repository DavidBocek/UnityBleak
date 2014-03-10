using UnityEngine;
using System.Collections;

/// <summary>
/// Animator for controlling 2D sprites using Unitys built in sprite animatino system.
/// </summary>
public class Unity2DSpriteAnimator2D : MonoBehaviour {
	
	public RaycastCharacterController2D controller;
	public Animator animator;

	protected int currentDirection;
	protected CharacterState currentState;
	protected bool isFirstFrame;

	private float timer;
	private bool doReset;

	void Start(){
		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController2D.CharacterControllerEventDelegate (CharacterAnimationEvent);
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
		currentDirection = controller.CurrentDirection;
		// Rope states
		if (currentState == CharacterState.ROPE_HANGING ||
		    currentState == CharacterState.ROPE_CLIMBING) {
			// No need to rotate, stay in existing direction
		}
		// Climbing states
		else if (currentState ==  CharacterState.CLIMBING ||
		         currentState ==  CharacterState.HOLDING ||
				 currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_UP ||
				 currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_DOWN) {
			// No need to rotate, stay in existing direction
		}
		// Wall slide
		else if (currentState == CharacterState.WALL_SLIDING) {
			if (controller.CurrentDirection > 0 ) {
				transform.localScale = new Vector3(1,1,1);
			} else if (controller.CurrentDirection < 0) {
				transform.localScale = new Vector3(-1,1,1);
			}	
		}
		// Directional states
		else {
			if (controller.CurrentDirection > 0 ) {
				transform.localScale = new Vector3(1,1,1);
			} else if (controller.CurrentDirection < 0) {
				transform.localScale = new Vector3(-1,1,1);
			}
		}
	}
}