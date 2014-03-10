using UnityEngine;
using System.Collections;

/// <summary>
/// A platform that moves left and right. Note that it returns true for parent on stand.
/// </summary>
public class BackAndForthPlatform2D : Platform2D {
	
	public float leftBound;
	public float rightBound;
	public float speed;
	
	override protected void DoStart() {
		if (speed < 0.0f) {
			velocity = new Vector3(speed, 0.0f, 0.0f);
			speed *= -1;
		} else {
			velocity = new Vector3(speed, 0.0f, 0.0f);
		}
	}
	
	override protected void DoUpdate () {
		if (myTransform.position.x >= rightBound) {
			myTransform.position = new Vector3(rightBound, myTransform.position.y, myTransform.position.z);
			velocity = new Vector3(-1 * speed, 0.0f, 0.0f);
		} else if (myTransform.position.x <= leftBound) {
			myTransform.position = new Vector3(leftBound, myTransform.position.y, myTransform.position.z);
			velocity = new Vector3(speed, 0.0f, 0.0f);
		}
	}
	
	override public Transform ParentOnStand(RaycastCharacterController2D character) {
		return myTransform;	
	}
}
