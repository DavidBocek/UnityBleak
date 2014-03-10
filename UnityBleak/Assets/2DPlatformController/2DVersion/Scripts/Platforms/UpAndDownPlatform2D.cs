using UnityEngine;
using System.Collections;

/// <summary>
/// A platform that moves up and down. Note that it returns true for parent on stand.
/// </summary>
public class UpAndDownPlatform2D : Platform2D {
	
	public float maxHeight;
	public float minHeight;
	public float speed;
	
	override protected void DoStart() {
		if (speed < 0.0f) {
			velocity = new Vector3(0.0f, speed, 0.0f);
			speed *= -1;
		} else {
			velocity = new Vector3(0.0f, speed, 0.0f);
		}
	}
	
	override protected void DoUpdate () {
		if (myTransform.position.y >= maxHeight) {
			myTransform.position = new Vector3(myTransform.position.x, maxHeight, myTransform.position.z);
			velocity = new Vector3(0.0f, -1 * speed, 0.0f);
		} else if (myTransform.position.y <= minHeight) {
			myTransform.position = new Vector3(myTransform.position.x, minHeight, myTransform.position.z);
			velocity = new Vector3(0.0f, speed, 0.0f);
		}
	}
	
	override public Transform ParentOnStand(RaycastCharacterController2D character) {
		return myTransform;	
	}
}
