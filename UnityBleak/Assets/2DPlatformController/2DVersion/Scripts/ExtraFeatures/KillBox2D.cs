using UnityEngine;
using System.Collections;

/// <summary>
/// Respawn the player when they touch this. Its a pretty simple 
/// implementation. In a real game you probably wont to invoke a death 
/// method which does all the things required for death (lives, scores, 
/// effects, sounds, etc)
/// </summary>
public class KillBox2D : Platform2D {
	
	/// <summary>
	/// Trigger the respawn.
	/// </summary>
	override public void DoAction(RaycastCollider2D collider, RaycastCharacterController2D character) {
		SimpleHealth2D health = character.GetComponent<SimpleHealth2D>();
		if (health != null && health.Health > 0) {
			health.Die ();
		}
	}
}
