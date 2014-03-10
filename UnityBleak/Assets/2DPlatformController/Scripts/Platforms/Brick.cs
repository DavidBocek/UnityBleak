using UnityEngine;
using System.Collections;

/// <summary>
/// A simple mario like brick, showing how you can respond to hitting something with characters head.
/// </summary>
public class Brick : Platform {
	
	public ParticleSystem particles;
	public BoxCollider myCollider;
	public MeshRenderer myRenderer;

	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		// Hitting from below
		if (collider.direction == RC_Direction.UP) {
			if (particles != null) particles.Play();
			myCollider.enabled = false;
			myRenderer.enabled = false;
		}
	}

}
