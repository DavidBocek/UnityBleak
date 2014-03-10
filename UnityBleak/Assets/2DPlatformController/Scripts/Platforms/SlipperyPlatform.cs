using UnityEngine;
using System.Collections;

/// <summary>
/// A platform which reduces drag, making the character slide around (i.e ice). Could also be
/// used to increase drag (sticky mud or long grass).
/// </summary>
public class SlipperyPlatform : Platform {
	
	public float drag;
	
	private bool alreadyAdded;
	
	override protected void DoUpdate(){
		alreadyAdded = false;	
	}
	
	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		// If we are standing on this
		if (!alreadyAdded && collider.direction == RC_Direction.DOWN) {
			character.SetDrag(drag);
			alreadyAdded = true;
		}
	}
}
