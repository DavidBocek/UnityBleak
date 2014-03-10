using UnityEngine;
using System.Collections;

/// <summary>
/// Very simple bouncy springboard platform. Boucnes a character with a fix force each time they stand on it.
/// </summary>
public class SpringboardPlatform : Platform {
	
	public float springForce = 20.0f;

	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		if (collider.direction == RC_Direction.DOWN) {
			character.Velocity = new Vector3(character.Velocity.x, springForce);
		}
	}
}
