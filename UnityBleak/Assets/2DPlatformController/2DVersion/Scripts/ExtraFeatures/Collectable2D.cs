using UnityEngine;
using System.Collections;

/// <summary>
/// A simple collectable item, it plays a particle effect 
/// when collected and invokes the Collect method on the HitBox class.
/// Requries a HitBox on the character :)
/// </summary>
public class Collectable2D : MonoBehaviour {

	/// <summary>
	/// My renderer, as a variable so we can seperate the renderer and the trigger.
	/// For example see the coin in the Mario demo.
	/// </summary>
	public SpriteRenderer myRenderer;

	/// <summary>
	/// The particle system to play on hit.
	/// </summary>
	public ParticleSystem particles;

	void OnTriggerEnter2D(Collider2D other) {
		HitBox2D collector = other.gameObject.GetComponent<HitBox2D>();
		if (collector != null) {
			collector.Collect(this);
			if (particles != null) particles.Play ();
			// You could destroy but here we are just going to turn off rendering and collision
			// This makes it easier to work with the particle system too
			myRenderer.enabled = false;
			if (myRenderer.gameObject.collider2D != null) myRenderer.gameObject.collider2D.enabled = false;
			if (myRenderer.gameObject.rigidbody2D != null) myRenderer.gameObject.rigidbody2D.isKinematic = true;
			collider2D.enabled = false;
			if (rigidbody2D != null) rigidbody2D.isKinematic = true;
		}
	}
}
