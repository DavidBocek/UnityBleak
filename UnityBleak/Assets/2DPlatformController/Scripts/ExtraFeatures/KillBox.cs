using UnityEngine;
using System.Collections;

/// <summary>
/// Respawn the player when they touch this. Its a pretty simple 
/// implementation. In a real game you probably wont to invoke a death 
/// method which does all the things required for death (lives, scores, 
/// effects, sounds, etc)
/// </summary>
public class KillBox : Platform {
	
	/// <summary>
	/// Trigger the respawn.
	/// </summary>
	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		character.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
	}
}
