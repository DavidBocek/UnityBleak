using UnityEngine;
using System.Collections;
 
/// <summary>
/// A simple mobile input controller which uses the Unity Standard Assets (Mobile) Joystick.
/// </summary>
public class StandardMobileInput : RaycastCharacterInput {
	
	public Joystick right;
	public Joystick left;
	
	private int tapCount;
	
	void Update () {
		jumpButtonDown = false; jumpButtonHeld = false;
		if (tapCount == right.tapCount && tapCount > 0) jumpButtonHeld = true;
		else if (right.tapCount > tapCount) jumpButtonDown = true;
			
	 	tapCount = right.tapCount;
	 
		// This allows for analogue input ... you could clamp it if you just want 1 and -1
		x = left.position.x;
		y = left.position.y; 
	}
}