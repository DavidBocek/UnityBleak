using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for ladder colliders. Extend this to create your own ladder functions.
/// </summary>
public abstract class Ladder : Platform {

	/// <summary>
	/// The control for the ladder.
	/// </summary>
	public LadderControl control;	

	/// <summary>
	/// Pass through value from the control.
	/// </summary>
	public float LedgeClimbHeight {
		get { return control.LedgeClimbHeight;}
	}
}
