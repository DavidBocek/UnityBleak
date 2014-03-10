using UnityEngine;
using System.Collections;

/// <summary>
/// A simple collectable item, it plays a particle effect 
/// when collected and invokes the Collect method on the HitBox class.
/// Requries a HitBox on the character :)
/// </summary>
public class Collectable : MonoBehaviour {

	/// <summary>
	/// My renderer, as a variable so we can seperate the renderer and the trigger.
	/// For example see the coin in the Mario demo.
	/// </summary>
	public Renderer myRenderer;

	/// <summary>
	/// The particle system to play on hit.
	/// </summary>
	public ParticleSystem particles;

	void OnTriggerEnter(Collider other) {
		HitBox collector = other.gameObject.GetComponent<HitBox>();
		if (collector != null) {
			collector.Collect(this);
			if (particles != null) particles.Play ();
			// You could destroy but here we are just going to turn off rendering and collision
			// This makes it easier to work with the particle system too
			myRenderer.enabled = false;
			if (myRenderer.collider != null) myRenderer.collider.enabled = false;
			if (myRenderer.rigidbody != null) myRenderer.rigidbody.useGravity = false;
			collider.enabled = false;
		}
	}
}
