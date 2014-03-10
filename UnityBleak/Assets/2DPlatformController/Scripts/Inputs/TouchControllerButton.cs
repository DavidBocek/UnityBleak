using UnityEngine;
using System.Collections;

public class TouchControllerButton : MonoBehaviour {

	/// <summary>
	/// What typ eof button is this.
	/// </summary>
	public TouchControllerButtonType type;

	/// <summary>
	/// The texture when button is not being held.
	/// </summary>
	public Texture2D normalTexture;

	/// <summary>
	/// The texture when button is being held
	/// </summary>
	public Texture2D heldTexture;

	protected bool isHeld;

	/// <summary>
	/// Called when the button is being held. Use this to change look.
	/// </summary>
	virtual public void Held() {
		if (!isHeld) renderer.material.mainTexture = heldTexture;
		isHeld = true;
	}

	/// <summary>
	/// Called when the button is let go. Use this to change look.
	/// </summary>
	virtual public void Released() {
		renderer.material.mainTexture = normalTexture;
		isHeld = false;
	}
}


public enum TouchControllerButtonType {
	LEFT,
	RIGHT,
	UP,
	DOWN,
	RUN,
	JUMP
	// Add new types here for other buttons
}