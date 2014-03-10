using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/**
 * A sample animation classes that works with the supplied
 * "Hero" 3d Model.
 */

public class HeroAnimator2D : MonoBehaviour {
	
	public RaycastCharacterController2D controller;

	/// <summary>
	///  Change this to false if you don't wont to see wlak style events while swimming.
	/// </summary>
	public bool playWalkAnimationsWhileSwimming = true;

	private Quaternion targetRotation;
	private CharacterState currentState;
	private CharacterState previousState;
	private Vector3 rootOffset;

	void Start(){
		// Set all animations to loop
   		animation.wrapMode = WrapMode.Loop;
   		// except a few
		if (animation["jump"] != null) {
   			animation["jump"].wrapMode = WrapMode.Once;
		}
		if (animation ["slide3"] != null) {
			animation["slide3"].wrapMode = WrapMode.Once;
		}
		if (animation ["ledge_climb"] != null) {
			animation["ledge_climb"].wrapMode = WrapMode.Clamp;
		}
		if (animation["stun2"] != null) {
			animation["stun2"].wrapMode = WrapMode.Once;
		}
		if (animation["crouch"] != null) {
			animation["crouch"].wrapMode = WrapMode.Clamp;
		}

		// Store root offset, we use this to reset position after animations this is only here to deal with a few rogue animations changing our root position
		// you probably wont need it!
		rootOffset = transform.localPosition;

		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController2D.CharacterControllerEventDelegate (CharacterAnimationEvent);
	}

	void Update() {
		CheckDirection ();
		// targetRotation = targetRotation * 
		//transform.localRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, targetRotation.eulerAngles.y, 0.0f);

		transform.localRotation = Quaternion.RotateTowards (transform.localRotation, targetRotation, Time.deltaTime * 400.0f);		
		//transform.rotation = Quaternion.RotateTowards (transform.localRotation, targetRotation, Time.deltaTime * 400.0f);
		// transform.localRotation = Quaternion.Euler (Quaternion.Inverse(transform.parent.rotation).eulerAngles.z, result.eulerAngles.y, 0.0f);
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
		this.previousState = previousState;
		transform.localPosition = rootOffset;
		switch (state) {
			case CharacterState.IDLE: Idle(previousState); break;	
			case CharacterState.WALKING: Walk(); break;	
			case CharacterState.RUNNING: Run(); break;	
			case CharacterState.SLIDING: Slide(); break;	
			case CharacterState.JUMPING: Jump(); break;	
			case CharacterState.AIRBORNE: Airborne(); break;	
			case CharacterState.FALLING: Fall(); break;	
			case CharacterState.DOUBLE_JUMPING: Jump(); break;	
			case CharacterState.WALL_JUMPING: Jump(); break;	
			case CharacterState.HOLDING: Hold(previousState); break;	
			case CharacterState.CLIMBING: Climb(); break;	
			case CharacterState.CLIMB_TOP_OF_LADDER_UP: ClimbTopUp(); break;	
			case CharacterState.CLIMB_TOP_OF_LADDER_DOWN: ClimbTopDown(); break;	
			case CharacterState.LEDGE_HANGING: LedgeHang(); break;
			case CharacterState.LEDGE_CLIMBING: LedgeClimb(); break;
			case CharacterState.LEDGE_CLIMB_FINISHED: Idle (previousState); break;
			case CharacterState.ROPE_CLIMBING: RopeClimb(); break;
			case CharacterState.ROPE_SWING: RopeSwing();break;
			case CharacterState.ROPE_HANGING: RopeHang();break;
			case CharacterState.WALL_SLIDING: WallSlide(); break;
			case CharacterState.CROUCHING: Crouch(); break;
			case CharacterState.CROUCH_SLIDING: CrouchSlide(); break;
			case CharacterState.STUNNED: Stunned(previousState); break;
			case CharacterState.PUSHING: Push(); break;
			case CharacterState.PULLING: Pull(); break;
		}
	}
	
	protected void Idle (CharacterState previousState) {
		animation.CrossFade ("idle");
		CheckDirection();
	}
	
	protected void Walk ()
	{
		if (playWalkAnimationsWhileSwimming || !controller.IsSwimming) {
			animation.CrossFade ("run");
		} else {
			animation.CrossFade("fall");
		}
		CheckDirection();
	}

	protected void Run ()
	{
		if (playWalkAnimationsWhileSwimming || !controller.IsSwimming) {
			animation.CrossFade ("run");
		} else {
			animation.CrossFade("fall");
		}
		CheckDirection();
	}

	protected void Slide ()
	{
		if (playWalkAnimationsWhileSwimming || !controller.IsSwimming) {
			animation.CrossFade ("slide3");
		} else {
			animation.CrossFade("fall");
		}
		CheckDirection();
	}

	protected void Jump() {
		animation.CrossFade("jump");
		CheckDirection();
	}

	protected void Airborne() {
		animation.CrossFade("airborne");
		CheckDirection();
	}


	protected void Fall() {
		animation.CrossFade("fall");
		CheckDirection();
	}
	
	protected void Hold(CharacterState previousState) {
		if (previousState != CharacterState.CLIMBING) animation.CrossFade ("climb");
		animation["climb"].speed = 0;
		animation["ledge_climb"].speed = 0;
		if (!animation.IsPlaying("ledge_climb")) {
			transform.localPosition= new Vector3(0, -0.75f, -1);
		}
	}
	
	protected void Climb() {
		animation["climb"].speed = 1;
		animation.CrossFade("climb");
		transform.localPosition = new Vector3(0, -0.75f,-1);
	}

	protected void ClimbTopUp() {
		animation["ledge_climb"].speed = 1;
		if ( animation["ledge_climb"].normalizedTime < 0.4f)  animation["ledge_climb"].normalizedTime = 0.4f;
		animation.CrossFade("ledge_climb");
	}

	protected void ClimbTopDown() {
		transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		animation["ledge_climb"].speed = -1;
		animation["ledge_climb"].normalizedTime = 0.9f;
		// if ( animation["ledge_climb"].normalizedTime < 0.4f)  animation["ledge_climb"].normalizedTime = 0.4f;
		animation.Play("ledge_climb");
	}

	protected void LedgeHang() {
		animation.CrossFade("ledge_hang");
	}

	protected void LedgeClimb() {
		animation["ledge_climb"].speed = 1;
		animation.Play("ledge_climb");
	}

	protected void RopeHang() {
		animation.CrossFade("rope_hang");
	}

	protected void RopeSwing() {
		// No animation here as yet
	}

	protected void RopeClimb() {
		animation.CrossFade("rope_climb");
	}
	
	protected void WallSlide() {
		animation.CrossFade("wallslide");
	}
	
	protected void Crouch() {
		animation.CrossFade("crouch3");
	}

	protected void CrouchSlide() {
		animation.CrossFade("groundslide");
	}

	protected void Push() {
		animation.CrossFade ("push");
	}
	
	protected void Pull() {
		animation.CrossFade ("pull");
	}

	protected void Stunned(CharacterState previousState) {
		// Don't play animation if it doesn't fit (for example while climbing)
		// in a finished game we might have animations for different kinds of hit
		if (previousState != CharacterState.ROPE_HANGING &&
		    previousState != CharacterState.ROPE_CLIMBING &&
		    previousState != CharacterState.ROPE_SWING &&
		    previousState != CharacterState.HOLDING &&
		    previousState != CharacterState.CLIMBING) {
			animation.CrossFade("stun2");
		}
	}

	protected void CheckDirection() {
		// Rope states
		if (currentState == CharacterState.ROPE_HANGING ||
		    currentState == CharacterState.ROPE_CLIMBING || 
		    (currentState == CharacterState.STUNNED  && (
			previousState == CharacterState.ROPE_HANGING ||
			previousState == CharacterState.ROPE_CLIMBING ))){
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 90.0f, 0.0f);

		}
		else if (currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_UP || currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_DOWN ||
		         currentState ==  CharacterState.HOLDING && (previousState == CharacterState.CLIMB_TOP_OF_LADDER_UP || previousState == CharacterState.CLIMB_TOP_OF_LADDER_DOWN)) {
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 0.0f, 0.0f);
		}
		// Climbing states
		else if (currentState ==  CharacterState.CLIMBING ||
		   		 currentState ==  CharacterState.HOLDING ) {
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, -90.0f, 0.0f);
		}

		// Directional states
		else {
			// You might need to switch 270 and 90 for other values depending on orientation of your model
			if (controller.CurrentDirection > 0 ) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 90.0f, 0.0f);
			} else if (controller.CurrentDirection < 0) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, -90.0f, 0.0f);
			}
		}
	}
}