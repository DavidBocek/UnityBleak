using UnityEngine;
using System.Collections;

/// <summary>
/// Respawn point, platform implementation.
/// </summary>
public class RespawnPoint2D : Platform2D {
	
	/// <summary>
	/// You might want to spawn in a different position to the platform,
	/// for example slightly above it. Use this for that.
	/// </summary>
	public Vector3 respawnPositionOffset;
	
	/// <summary>
	/// The current respawn point. This only supports one character for multiple characters
	/// you could store the reapwn point on the character or use a Dictionary that maps 
	/// characters to respawn points.
	/// </summary>
	protected static RespawnPoint2D currentRespawnPoint;
	
	/// <summary>
	/// Stand on a respawn point to activate it. You could play a particle effect of something here.
	/// </summary>
	override public void DoAction(RaycastCollider2D collider, RaycastCharacterController2D character) {
		if (collider.direction == RC_Direction.DOWN) currentRespawnPoint = this;
	}
	
	/// <summary>
	/// Respawn the character at the given point.
	/// </summary>
	public static void Respawn(RaycastCharacterController2D character) {
		if (currentRespawnPoint != null) {
			character.Velocity = Vector2.zero;
			character.transform.position = currentRespawnPoint.myTransform.position + currentRespawnPoint.respawnPositionOffset;
		}
	}
}