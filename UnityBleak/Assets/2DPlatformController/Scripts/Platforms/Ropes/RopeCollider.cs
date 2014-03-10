using UnityEngine;
using System.Collections;

/// <summary>
/// Rope platform. Attach this to every collider used in a rope. Note that rope
/// colliders must be the child of a rigidbody.
/// </summary>
public class RopeCollider : Rope {

	private Rigidbody myRigidbody;
	private float centreDelta;
	private Vector3 swingForce;
	
	override public bool overrideX {get {return true;} }
	override public bool overrideY{get {return true;} }
	override public bool overrideAnimation{get {return true;} }
	
	override protected void DoStart() {
		myRigidbody = transform.parent.gameObject.rigidbody;
	}
	
	
	override protected void Move() {
		// Dont move let the physics system do it
	}
	
	override public Transform ParentOnStand(RaycastCharacterController character) {
		if (character.StartedClimbing) {
			// Not on the same rope
			if (!(character.MyParent is Rope && ((Rope)character.MyParent).control == control)) {
				control.LastSwingDirection = 0.0f;
				myRigidbody.AddForce(character.Velocity.x * 0.5f, 0.0f, 0.0f, ForceMode.VelocityChange);
				character.Velocity = Vector2.zero;
				// Centre on rope
				float delta = myTransform.position.x - character.transform.position.x;
				character.transform.Translate(delta, 0.0f, 0.0f, Space.Self);
			}
			return myRigidbody.transform;
		}
		return null;
	}
	
	void FixedUpdate() {
		velocity = myRigidbody.velocity;
		myRigidbody.AddForce(swingForce, ForceMode.Force);
		swingForce *= 0.985f;
	}
	
	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		if (character.StartedClimbing) {
			// Swing rope
			if (character.characterInput.x > 0.0f && control.LastSwingDirection <= 0.0f) {
				swingForce = new Vector3(Mathf.Abs (Mathf.Cos(Mathf.Deg2Rad * myTransform.rotation.eulerAngles.z)) * character.climbing.ropeSwingForce, 0.0f, 0.0f);
				control.LastSwingDirection = 1;
				control.hasSwung = true;
			} else if (character.characterInput.x < 0.0f && control.LastSwingDirection >= 0.0f) {
				swingForce = new Vector3(Mathf.Abs (Mathf.Cos(Mathf.Deg2Rad * myTransform.rotation.eulerAngles.z)) * character.climbing.ropeSwingForce * -1, 0.0f, 0.0f);	
				control.LastSwingDirection = -1;
				control.hasSwung = true;
			}
			// Move up and down rope
			if (!control.hasClimbed) {
				Vector2 climbForce = Vector2.zero;
				if (character.characterInput.y  > 0.0f) {
					control.hasClimbed = true;
					climbForce.y = character.climbing.speed * RaycastCharacterController.FrameTime;
				} else if (character.characterInput.y < 0.0f) {
					control.hasClimbed = true;
					climbForce.y = -1 * character.climbing.speed * RaycastCharacterController.FrameTime;
				}
				character.transform.Translate(0.0f, Vector2.Dot(climbForce, myTransform.rotation * Vector2.up), 0.0f, Space.Self);
				float delta = myTransform.position.x - character.transform.position.x;
				character.transform.Translate(RaycastCharacterController.FrameTime * delta, 0.0f, 0.0f, Space.Self);
			}	
		} 
	}
	
	override public CharacterState GetAnimationState(RaycastCharacterController character) {
		if (control.canClimb && control.hasClimbed) return CharacterState.ROPE_CLIMBING;
		if (control.hasSwung) return CharacterState.ROPE_SWING;
		return CharacterState.ROPE_HANGING;
	}
}
