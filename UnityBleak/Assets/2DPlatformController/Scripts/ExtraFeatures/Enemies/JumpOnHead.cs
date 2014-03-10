using UnityEngine;
using System.Collections;

/// <summary>
/// Attach a trigger for the head of the enemy and then add this script to it
/// to kill the enemy when you jump on him.
/// </summary>
public class JumpOnHead : MonoBehaviour {

	public IEnemy me;
	
	void Start () {
		if (transform.parent != null) me = (IEnemy) transform.parent.gameObject.GetComponent(typeof(IEnemy));
	}

	void OnTriggerEnter(Collider other) {
		HitBox health = other.gameObject.GetComponent<HitBox>();
		if (health != null) me.KillFromAbove(health, collider);
	}
}
