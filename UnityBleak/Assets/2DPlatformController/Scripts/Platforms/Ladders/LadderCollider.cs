using UnityEngine;
using System.Collections;

/// <summary>
/// Ladder collider. Attach this to every collider used in a ladder. This is like a rung of the ladder.
/// </summary>
public class LadderCollider : Ladder {
	
	private float centreDelta;
	
	override public bool overrideX {get {return !control.canMoveSideWays;} }
	override public bool overrideY{get {return false;} }
	
	override protected void Move() {
		// Dont move let the physics system do it
	}
	
	override public Transform ParentOnStand(RaycastCharacterController character) {
		if (character.StartedClimbing) {
			// Not on the same rope
			if (!(character.MyParent is Ladder && ((Ladder)character.MyParent).control == control)) {
				// Centre on rope
				if (control.snapToMiddle) {
					float delta = myTransform.position.x - character.transform.position.x;
					character.transform.Translate(delta, 0.0f, 0.0f, Space.Self);
				}
			}
			return transform;
		}
		return null;
	}

	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		if (character.StartedClimbing) {
			// Move off ladder
			if (control.dismountWithArrows && character.characterInput.x != 0.0f) {
				character.Dismount(this);
			}
		} 
	}

}
