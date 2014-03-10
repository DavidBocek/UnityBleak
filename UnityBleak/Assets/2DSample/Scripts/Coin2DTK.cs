using UnityEngine;
using System.Collections;

/// <summary>
/// A simple collectable coin, when collected and invokes the Collect method on the HitBox class.
/// Requries a HitBox on the character :)
/// </summary>
public class Coin2DTK : Collectable {
	
	
	/// <summary>
	/// My sprite, as a variable so we can seperate the renderer and the trigger.
	/// </summary>
	public tk2dSprite sprite;
	
	private bool isBrickCoin = false;
	
	void Update() {
		if (isBrickCoin && rigidbody.velocity.y < 0.0f) Collect();
	}

	/// <summary>
	/// Coin spanws from inside a box and is automatically collected.
	/// </summary>
	public void Spawn(Vector3 spawnForce) {
		isBrickCoin = true;
		rigidbody.useGravity = true;
		rigidbody.AddForce(spawnForce, ForceMode.VelocityChange);
		StartCoroutine(DoAutoCollect ());
		HitBox box = GameObject.FindObjectOfType(typeof(HitBox)) as HitBox;
		if (box != null) box.Collect(this);
	}

	void OnTriggerEnter(Collider other) {
		HitBox collector = other.gameObject.GetComponent<HitBox>();
		if (collector != null) {
			collector.Collect(this);
			Collect();
		}
	}

	private IEnumerator DoAutoCollect() {
		while (sprite.color.a > 0.0f) {
			sprite.color = new Color(sprite.color.r, sprite.color.g,sprite.color.b, sprite.color.a - (3 * Time.deltaTime));
			yield return true;
		}
		Collect();
	}

	private void Collect() {
		// You could destroy but here we are just going to turn off rendering and collision
		sprite.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		isBrickCoin = false;
		if (collider != null) collider.enabled = false;
		if (rigidbody != null) rigidbody.useGravity = false;
	}
}
