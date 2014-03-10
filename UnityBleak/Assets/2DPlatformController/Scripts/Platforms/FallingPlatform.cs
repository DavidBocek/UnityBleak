using UnityEngine;
using System.Collections;

/// <summary>
/// Platform that falls after you touch it. The fall is delayed by the
/// value of fallDelay.
/// </summary>
public class FallingPlatform : Platform {
	
	public float fallDelay = 1.0f;
	public float velocityForFall = -10;
	private bool fallStarted;
	
	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		if (collider.direction == RC_Direction.DOWN && !fallStarted) {
			fallStarted = true;
			StartCoroutine(Fall());
		}
	}

	override public Transform ParentOnStand (RaycastCharacterController character) {
		if (rigidbody.velocity.y > velocityForFall) {
			return myTransform;
		}
		if (character.transform.parent != null) {
			character.Velocity = new Vector2 (character.Velocity.x, rigidbody.velocity.y);
			character.transform.parent = null;
		}
		return null;
	}

	private IEnumerator Fall() {
		yield return new WaitForSeconds(fallDelay);
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
	}
}
