using UnityEngine;
using System.Collections;

/// <summary>
/// A simple character input with additional conditions for swimming. Arrows to move, left SHIFT to run, SPACE to jump and swim
/// In this controller you cannot run while swimming, to determine if swimming it reuqires a reference to the character controller.
/// </summary>
public class SwimmingCharacterInput : RaycastCharacterInput
{
	public RaycastCharacterController controller;
	public bool alwaysRun;
	/// <summary>
	/// If true holding the swim button will keep doing a swim stroke.
	/// </summary>
	public bool autoSwim;
	private int movingDirection;
	
	void Start () {
		if (controller == null) {
			controller = GetComponent<RaycastCharacterController>();
		}
	}

	void Update ()
	{
		
		if (Input.GetKey (KeyCode.R)) {
			Application.LoadLevel (0);
		}
		
		jumpButtonHeld = false;
		jumpButtonDown = false;
		x = 0;
		y = 0;
		
		if (Input.GetKey ("right") && !Input.GetKey ("left")) {
			x = 0.5f;
			movingDirection = 1;
		} else if (Input.GetKey ("left") && !Input.GetKey ("right")) {
			x = -0.5f;
			movingDirection = -1;
		} else if (Input.GetKey ("right") && Input.GetKey ("left")) {
			x = movingDirection / 2.0f;
		}
		
		// Shift to run
		if (!controller.IsSwimming && (alwaysRun || Input.GetKey (KeyCode.LeftShift))) {
			x *= 2;
		}
		
		if (Input.GetKey("up") ) {
			y = 1;
		} else if (Input.GetKey("down") ) {
			y = -1;
		}
		
		if (Input.GetKey(KeyCode.Space) ) {
			jumpButtonHeld = true;
			if (Input.GetKeyDown(KeyCode.Space)) {
				jumpButtonDown = true;
				swimButtonDown = true;		
			} else {
				jumpButtonDown = false;		
				swimButtonDown = autoSwim;
			}
		} else {
			jumpButtonDown = false;
			swimButtonDown = false;
		}
	}
	
}

