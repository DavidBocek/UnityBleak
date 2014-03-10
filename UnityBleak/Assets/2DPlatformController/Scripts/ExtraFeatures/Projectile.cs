using UnityEngine;
using System.Collections;

/// <summary>
/// A very simple projectile that moves across the screen and triggers
/// the damage method on a HitBox if it collides with one.
/// </summary>
public class Projectile : MonoBehaviour {

	public float speed = -4.0f;
	public float minX = - 50.0f;

	void Update () {
		transform.Translate(speed * RaycastCharacterController.FrameTime, 0,0 );
		if (transform.position.x < minX) GameObject.Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		HitBox health = other.gameObject.GetComponent<HitBox>();
		if (health != null) health.Damage(1);

		GameObject.Destroy(gameObject);
	}
}
