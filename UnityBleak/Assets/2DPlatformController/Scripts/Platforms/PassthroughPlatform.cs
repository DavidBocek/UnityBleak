using UnityEngine;
using System.Collections;

public class PassthroughPlatform : Platform {
	
	public float fallThroughTime = 1.5f;
	public bool requireJump;
	
	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		if (character.FallThroughTimer <= 0.0f && collider.direction == RC_Direction.DOWN && character.characterInput.y < 0.0f && (!requireJump ||character.characterInput.jumpButtonDown)) {
			character.FallThroughTimer = fallThroughTime;
			character.characterInput.CancelJump();
		}
	}
}
