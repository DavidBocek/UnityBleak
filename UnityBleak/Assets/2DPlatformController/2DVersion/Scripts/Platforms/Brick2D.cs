using UnityEngine;
using System.Collections;

/// <summary>
/// A simple mario like brick, showing how you can respond to hitting something with characters head.
/// </summary>
public class Brick2D : Platform2D {
	
	public ParticleSystem particles;
	public BoxCollider2D myCollider;
	public MeshRenderer myRenderer;

	override public void DoAction(RaycastCollider2D collider, RaycastCharacterController2D character) {
		// Hitting from below
		if (collider.direction == RC_Direction.UP) {
			if (particles != null) particles.Play();
			myCollider.enabled = false;
			myRenderer.enabled = false;
		}
	}

}
