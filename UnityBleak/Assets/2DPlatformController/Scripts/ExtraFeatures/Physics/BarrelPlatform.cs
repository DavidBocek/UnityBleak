using UnityEngine;
using System.Collections;

public class BarrelPlatform : Platform {

	public Rigidbody barrel;

	void Update() {
		velocity = barrel.velocity;
	}
	/// <summary>
	/// Does this platform want to have this platform become the characters parent. Used for moving platforms.
	/// </summary>
	override public Transform ParentOnStand(RaycastCharacterController character) {
		return myTransform;	
	}

}
