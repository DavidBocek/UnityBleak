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
public class UnbreakableBrick2DTK : Platform {
	
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
	/// Sprite that represents this brick.
	/// </summary>
	public tk2dSprite sprite;

	/// <summary>
	/// The name of the sprite to replace with on hit. Ignored if null or 0 length.
	/// </summary>
	public string hitSpriteName;
	
	private bool isActive = true;
	private bool hasSpawned = false;
	
	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		// Hitting from below (i.e. a headbutt)
		if (isActive && collider.direction == RC_Direction.UP) {
			StartCoroutine(DoHit());
			if (spawnGameObject != null && !hasSpawned) DoSpawn();
		}
		// Kill enemies above 
		else if (!isActive && collider.direction == RC_Direction.DOWN && character is IEnemy) {
			((IEnemy)character).KillFromBelow(spawnForce.y);
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
	
	private void DoSpawn() {
		hasSpawned = true;
		if (hitSpriteName != null && hitSpriteName.Length > 0) sprite.SetSprite(sprite.GetSpriteIdByName(hitSpriteName));
		spawnGameObject.SendMessage("Spawn", spawnForce, SendMessageOptions.DontRequireReceiver);
	}
	
}
