using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract class for character input, extend this to provide your own input.
/// </summary>
public abstract class RaycastCharacterInput : MonoBehaviour {
	
	/// <summary>
	/// The x movement. Use 1.0f or -1.0f to run. Smaller values to walk.
	/// </summary>
	virtual public float x{get; protected set;}
	
	/// <summary>
	/// The y movement. 1.0f = up, -1.0f = down.
	/// </summary>
	virtual public float y{get; protected set;}
	
	/// <summary>
	/// Is the jump button being held down.
	/// </summary>
	virtual public bool jumpButtonHeld{get; protected set;}
	
	/// <summary>
	/// Was the jump button pressed this frame.
	/// </summary>
	virtual public bool jumpButtonDown{get; protected set;}

	/// <summary>
	/// Was the swim button pressed this frame.
	/// </summary>
	virtual public bool swimButtonDown{get; protected set;}

	/// <summary>
	/// Is the command for passing down through a passthrough platform being sent.
	/// </summary>
	virtual public bool dropFromPlatform{get; protected set;}

	/// <summary>
	/// Stop jump from happening, useful for platforms that don't let you jump (or special jump behaviour).
	/// </summary>
	virtual public void CancelJump() {
		jumpButtonDown = false; 
		jumpButtonHeld = false;
	}
}