using UnityEngine;
using System.Collections;

/// <summary>
/// A platform you can pass through and also drop back down through by pressing down (optionally down + jump).
/// </summary>
public class PassthroughPlatform2D : Platform2D {
	
	public float fallThroughTime = 1.5f;
	
	override public void DoAction(RaycastCollider2D collider, RaycastCharacterController2D character) {
		if (character.FallThroughTimer <= 0.0f && collider.direction == RC_Direction.DOWN && character.characterInput.dropFromPlatform) {
			character.FallThroughTimer = fallThroughTime;
		}
	}
}
