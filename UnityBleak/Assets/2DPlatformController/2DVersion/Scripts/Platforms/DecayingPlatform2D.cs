using UnityEngine;
using System.Collections;

/// <summary>
/// Platform that will fall if you stand on it for more than the decay time.
/// The timer resets each time to you leave the paltform so if you keep jumping
/// you can stay on the platform.
/// </summary>
public class DecayingPlatform2D : Platform2D {
	
	public float decayTime = 1.0f;
	private float standTime = 0.05f;
	private bool characterIsOnMe;
	private float decayTimer;
	private float standTimer;

	void Awake () {
		decayTimer = decayTime;
	}

	void Update() {
		if (characterIsOnMe) {
			standTimer -= Time.deltaTime;
			decayTimer -= Time.deltaTime;
			if (standTimer <= 0.0f) characterIsOnMe = false;
			if (decayTimer <= 0.0f) Fall ();
		} else {
			decayTimer = decayTime;
		}
	}

	override public void DoAction(RaycastCollider2D collider, RaycastCharacterController2D character) {
		if (collider.direction == RC_Direction.DOWN) {
			characterIsOnMe = true;
			standTimer = standTime;
		}
	}
	
	private void Fall() {
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
	}
}
