using UnityEngine;
using System.Collections;

/// <summary>
/// A 2D Toolkit version of the respawn point which switches the sprite.
/// </summary>
public class RespawnFlag2DTK : RespawnPoint {
	public bool isStartingPoint;

	public tk2dSprite sprite;
	public string flagUpSprite;
	public string flagDownSprite;

	void Awake() {
		if (isStartingPoint) RespawnPoint.currentRespawnPoint = this;
	}

	/// <summary>
	/// Stand on a respawn point to activate it. You could play a particle effect of something here.
	/// </summary>
	override public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		if (RespawnPoint.currentRespawnPoint is RespawnFlag2DTK) ((RespawnFlag2DTK)RespawnPoint.currentRespawnPoint).Down ();
		if (collider.direction == RC_Direction.DOWN) RespawnPoint.currentRespawnPoint = this;
		sprite.SetSprite(sprite.GetSpriteIdByName(flagUpSprite));
	}

	/// <summary>
	/// Puts the flag down.
	/// </summary>
	public void Down(){
		sprite.SetSprite(sprite.GetSpriteIdByName(flagDownSprite));
	}
}
