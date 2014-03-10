using UnityEngine;
using System.Collections;

/// <summary>
/// The main control class for a ladder. Put this at the root of your ladder. Can
/// also be used for vine like climbable or extended for other climbable behaviour.
/// </summary>
public class LadderControl2D : MonoBehaviour {

	/// <summary>
	/// Does pressing left or right dismount the ladder or do you need to jump?
	/// </summary>
	public bool dismountWithArrows = true;

	/// <summary>
	/// Can the character move sideways on the ladder?
	/// </summary>
	public bool canMoveSideWays = false;

	/// <summary>
	/// Should the character nsap to the middle of the ladder?
	/// </summary>
	public bool snapToMiddle = true;

	/// <summary>
	/// Convenience method. Stop the character climbing the top of the ladder like a ledge climb.
	/// </summary>
	public bool disableLedgeClimb;
	
	/// <summary>
	/// At what point on the ladder do we trigger the special ledge climb animation state. Setting the value for this
	/// heavily depends on your animations. See the HeroSample for one way of doing it.
	/// </summary>
	public float ledgeClimbOffset;

	private float ledgeClimbHeight;	

#if UNITY_EDITOR
	void Awake() {
		if (canMoveSideWays && (dismountWithArrows || snapToMiddle)) Debug.LogWarning ("If you can move sideways on a ladder you should disable dismountWithArrows and snapToMiddle");
	}
#endif

	void Start() {
		ledgeClimbHeight = transform.position.y + ledgeClimbOffset;
	}
	
	/// <summary>
	/// Is the character at the appropriate height to trigger the climb top of ladder state.
	/// </summary>
	virtual public bool ShouldPlayLedgeClimb (RaycastCharacterController2D character) {
		return !disableLedgeClimb && character.transform.position.y > ledgeClimbHeight;
	}

	/// <summary>
	///  The height the climb to ladder top state is triggereda at. Used by the character controller rto ensure climb animation is correcty lined up.
	/// </summary>
	public float LedgeClimbHeight {
		get { return ledgeClimbHeight;}
	}
}
