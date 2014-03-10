using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles characters that don't have symmetrical colliders
/// by flipping the colliders based on the direction the character it facing.
/// It is important that you use the same method of checking direciton here
/// that you use in your animator.
/// 
/// You can extend this class to provide alternate functionality.
/// </summary>
public class DirectionChecker : MonoBehaviour {

	/// <summary>
	/// The starting direction, if your character starts
	/// facing left or right make sure this is set to 1 for
	/// right or -1 for left.
	/// </summary>
	public int startingDirection = 1;

	/// <summary>
	/// The current direction being faced. 0 = NONE, 1 = RIGHT
	/// -1 = LEFT.
	/// </summary>
	virtual public int CurrentDirection {
		get; private set;
	}

	/// <summary>
	/// Call to recalculate the direction.
	/// </summary>
	/// <returns><c>true</c>, if colliders were switched, <c>false</c> otherwise.</returns>
	virtual public bool UpdateDirection (RaycastCharacterController character) {
		int newDirection = 0;

		
		// Always return ledge hang dir - NEW
		if (character.IsLedgeHanging) {
			if (character.LedgeHangDirection == RC_Direction.RIGHT) newDirection = 1;
			if (character.LedgeHangDirection == RC_Direction.LEFT) newDirection =  -1;
		}
		if (character.characterInput.x > 0.0f) newDirection =  1;
		if (character.characterInput.x < 0.0f) newDirection =  -1;
		//if (character.Velocity.x > 0.0f) newDirection =  1;
		//if (character.Velocity.x < 0.0f) newDirection =  -1;
		
		// If PULLING then return oppoiste direction
		if (character.State == CharacterState.PULLING) newDirection *= -1;

		// Special case for startup
		if (CurrentDirection == 0 && newDirection != 0) {
			if (newDirection != startingDirection) SwitchColliders(character);
		}

		if (newDirection != 0 && newDirection != CurrentDirection) {
			SwitchColliders(character);
			CurrentDirection = newDirection;
			return true;
		}
		return false;
	}

	virtual protected void SwitchColliders(RaycastCharacterController character) {
		character.SwitchColliders ();
	}

	void Start() {
		if (startingDirection != 1 && startingDirection != -1) {
			Debug.LogError ("Incorrect starting position, setting to 1 (RIGHT)");
			startingDirection = 1;
		}
	}
}
