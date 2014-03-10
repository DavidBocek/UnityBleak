using UnityEngine;
using System.Collections;

/// <summary>
/// A mushroom, when collected and invokes the Collect method on the HitBox class.
/// Requries a HitBox on the character :)
/// </summary>
public class Mushroom2DTK : Collectable {
	
	/// <summary>
	/// My sprite, as a variable so we can seperate the renderer and the trigger.
	/// </summary>
	public tk2dSprite sprite;
	
	public float grownPosition;

	public void Spawn(Vector3 spawnForce) {
		StartCoroutine (Grow());
	}
	
	void OnTriggerEnter(Collider other) {
		HitBox collector = other.gameObject.GetComponent<HitBox>();
		if (collector != null) {
			collector.Collect(this);
			Collect();
		}
	}
	
	private IEnumerator Grow() {
		while (transform.localPosition.y < grownPosition) {
			transform.Translate(0.0f, 2 * Time.deltaTime, 0.0f);
			yield return true;
		}
		collider.enabled = true;
	}
	
	private void Collect() {
		// You could destroy but here we are just going to turn off rendering and collision
		sprite.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		if (collider != null) collider.enabled = false;
		if (rigidbody != null) rigidbody.useGravity = false;
	}
}
