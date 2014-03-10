using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A simple mario like brick, showing how you can respond to hitting something with characters head.
/// This brick wont break but will instead "bobble" when you hit it. It can optionally spawn an object
/// like a coin.
///
/// When an enemy stands on the platform and it is hit from below the enemy will be killed.
/// </summary>
public class UnBreakableBrick : Platform {
	
	/// <summary>
	/// The object activated on hit.
	/// </summary>
	public GameObject spawnGameObject;
	/// <summary>
	/// If the object is a rigidbody we will add some force to the
	/// object to add a little juice.
	/// </summary>
	public Vector3 spawnForce;

	/// <summary>
	/// How long after spawning should we wait before turning on physics. This delay
	/// gives object a chance to ensure its outside of the boundaries of the brick.
	/// </summary>
	public float spawnActivateTime = 0.5f;

	/// <summary>
	/// Change the material to this after hit so we know the object has spawned.
	/// </summary>
	public Material hitMaterial;

	private bool isActive = true;
	private bool hasSpawned = false;

	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		// Hitting from below (i.e. a headbutt)
		if (isActive && collider.direction == RC_Direction.UP) {
			StartCoroutine(DoHit());
			if (spawnGameObject != null && !hasSpawned) StartCoroutine(DoSpawn());
			else if (spawnGameObject != null) DoBobble();
		}
		// Kill enemies above 
		else if (!isActive && collider.direction == RC_Direction.DOWN && character is IEnemy) {
			((IEnemy)character).Kill();
		}
	}
	
	private IEnumerator DoHit(){
		isActive = false;
		// Bobble the brick when it gets headbutted.
		Vector3 pos = myTransform.position;
		float velocity = 2.0f;
		myTransform.Translate(0.0f, velocity * Time.deltaTime, 0.0f);
		velocity += Physics.gravity.y * Time.deltaTime;
		yield return null;
		while (myTransform.position.y - pos.y > 0.0f){
			myTransform.Translate(0.0f, velocity * Time.deltaTime, 0.0f);
			velocity += Physics.gravity.y * Time.deltaTime;
			yield return null;
		}
		myTransform.position = pos;
		isActive = true;
	}
	
	private IEnumerator DoSpawn() {
		hasSpawned = true;
		renderer.material = hitMaterial;
		spawnGameObject.renderer.enabled = true;
		if (spawnGameObject.rigidbody != null) {
			spawnGameObject.rigidbody.useGravity = true;
			spawnGameObject.rigidbody.AddForce(spawnForce, ForceMode.VelocityChange);
		}
		// We need to wait for a little while before activating the collider so the spawned
		// object has a chance to get outside the brick.
		yield return new WaitForSeconds (spawnActivateTime);
		spawnGameObject.collider.enabled = true;
	}

	/// <summary>
	/// After an object is spawned it looks kinda cool if we ccan bounce it around. NOTE: You might need to
	/// remove this if you aren't just sending an object straight up in to the air.
	/// </summary>
	private void DoBobble() {
		if (spawnGameObject.rigidbody != null) {
			spawnGameObject.rigidbody.AddForce(spawnForce / 2.0f, ForceMode.VelocityChange);
		}
	}

}
