using UnityEngine;
using System.Collections;

public class BarrelPush : MonoBehaviour {

	public float pushForce = 1.0f;
	public int characterLayer;
	public float sleepVelocity = 1.0f;

	// Use this for initialization
	void OnCollisionEnter(Collision info) {
		if (Mathf.Abs (rigidbody.velocity.x) < sleepVelocity  && info.gameObject.layer == characterLayer) {
			if (info.gameObject.transform.position.x > transform.position.x) {
				rigidbody.AddForce(-1 * pushForce, 0, 0, ForceMode.VelocityChange);
			} else {
				rigidbody.AddForce(pushForce, 0, 0, ForceMode.VelocityChange);
			}
		}
	}
}
