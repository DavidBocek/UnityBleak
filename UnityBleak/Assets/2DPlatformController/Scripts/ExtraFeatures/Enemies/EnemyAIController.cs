using UnityEngine;
using System.Collections;

/// <summary>
/// An enemy AI which tries to follow the player. Its built using the hack until it works philosophy of AI design,
/// so the code might be a little hard to follow. Currently it doesn't cater for sense distance nor do path finding.
/// Specifically it doesn't plan a route to a player it just tries to get on to higher platforms if the player is higher
/// and lower platforms if the player is lower.
///
/// It takes a map of the level geometry (expected to be box colliders) and will only consider geometry added to its list.
/// YOu can use this to somewhat restrict the places it will move to (by only including a subset of level geometry).
/// </summary>
public class EnemyAIController : RaycastCharacterInput, IEnemy {

	public BoxCollider[] geometry;
	public RaycastCharacterController me;
	public float xDelta = 0.5f;
	public float yDelta = 0.25f;
	public float jumpHeight;
	public float jumpDistance;
	public float decideTime = 0.5f;

	
	public RaycastCharacterController targetController;

	private Transform target;
	
	private float goalPosition;
	private float moveDirection;

	
	private bool fall;
	private float fallDirection;
	
	void Start() {
		target = targetController.transform;
		InvokeRepeating("Decide", decideTime, decideTime);
	}

	// Update is called once per frame
	void Decide () {

		x = 0.0f; y = 0.0f;
		jumpButtonDown = false; jumpButtonHeld = false;

		// If we are in the air keep our direction consistent
		if (me.State == CharacterState.JUMPING || me.State == CharacterState.FALLING || me.State == CharacterState.AIRBORNE) {
			x = moveDirection / 1.1f;
		} else {
			AIAction currentAction = DecideOnAction();
			// Else apply the AI action
			switch (currentAction) {
				case (AIAction.JUMP_TO_PLATFORM) :
 					if (Mathf.Abs (goalPosition - transform.position.x) < xDelta) {
						x = moveDirection / 1.1f;
						jumpButtonDown = true;
					} else {
						if (goalPosition > transform.position.x) {
							x = 1.0f / 1.1f;
						} else {
							x = -1.0f / 1.1f;
						}
					}
					break;
				case (AIAction.JUMP_AT_CHARACTER) :
					x = moveDirection;
					jumpButtonDown = true;
					break;
				case (AIAction.FALL_OFF_LEDGE) :
					x = moveDirection;
					break;
				case (AIAction.WALK_TOWARDS_TARGET) :
					x = moveDirection;
					break;
			}
		}
	}

	private AIAction DecideOnAction() {

		// Try to climb higher if character is higher than us and on a platform and we are on a platform
		BoxCollider climbTarget = null;
		float climbTargetPosition = transform.position.x;
		BoxCollider current = null;
		foreach (RaycastCollider feetCollider in me.feetColliders) {
			RaycastHit collision = feetCollider.GetCollision(1 << me.backgroundLayer | 1 << me.passThroughLayer, 0.01f);
			if (collision.collider != null && collision.collider is BoxCollider) {
				current = (BoxCollider) collision.collider;
				break;
			} 
		}

		if (target.position.y > transform.position.y && 
		    (targetController.GroundedFeetCount > 0 ||  target.position.y > transform.position.y + yDelta + jumpHeight) && 
		    (me.State != CharacterState.JUMPING && me.State != CharacterState.FALLING && me.State != CharacterState.AIRBORNE)) {
			
			// Try to find a higher platform that we can get to.
			if (current != null) {
				float highest = -9999;
				float closest = 9999;
				foreach (BoxCollider c in geometry) {
					if (c != current && c.bounds.max.y < current.bounds.max.y + jumpHeight) {
						if (c.bounds.min.x > current.bounds.min.x && c.bounds.min.x < current.bounds.max.x + jumpDistance) {
							if (c.bounds.max.y > highest) {
								climbTargetPosition = Mathf.Min(current.bounds.max.x, c.bounds.min.x - (jumpDistance / 2.0f));
								climbTarget = c;
								highest = c.bounds.max.y;
								closest = Mathf.Abs (climbTargetPosition - transform.position.x);
								moveDirection = 1;
							}
						} 
						if (c.bounds.max.x < current.bounds.max.x && c.bounds.max.x > current.bounds.min.x - jumpDistance) {
							if (c.bounds.max.y > highest) {
								climbTargetPosition = Mathf.Max(current.bounds.min.x, c.bounds.max.x + (jumpDistance / 2.0f));
								climbTarget = c;
								highest = c.bounds.max.y;
								moveDirection = -1;
							}
							// Make sure we jump using the closest path 
							else if (climbTarget == c && closest > Mathf.Abs (Mathf.Max(current.bounds.min.x, c.bounds.max.x + (jumpDistance / 2.0f)) - transform.position.x)) {
								climbTargetPosition = Mathf.Max(current.bounds.min.x, c.bounds.max.x + (jumpDistance / 2.0f));
								moveDirection = -1;
							}
						}
					}
				}
			}
		}
		
		if (climbTarget != null) {
			goalPosition = climbTargetPosition;
			return AIAction.JUMP_TO_PLATFORM;
		}
		
		// Get direction to enemy
		float enemyDirection;
		if (target.position.x > transform.position.x) {
			enemyDirection = 1.0f;
		} else {
			enemyDirection = -1.0f;
		}

		// Character is above us but not on a platform and not too far away
		if (targetController.GroundedFeetCount == 0 && target.position.y > transform.position.y && Mathf.Abs( target.position.x - transform.position.x) < xDelta * 3.0f) {
			moveDirection = 0;
			return AIAction.JUMP_AT_CHARACTER;
		}

		// If character lower than us walk towards a ledge if we can find one to fall from
		if (current != null && target.position.y < transform.position.y + yDelta) {
			foreach (BoxCollider c in geometry) {
				if (c.bounds.max.x < current.bounds.max.x && c.bounds.max.x > current.bounds.min.x && c.bounds.max.y < current.bounds.max.y) {
					moveDirection = -1.0f;
					return AIAction.FALL_OFF_LEDGE;
				} else if (c.bounds.min.x < current.bounds.max.x && c.bounds.min.x > current.bounds.min.x && c.bounds.max.y < current.bounds.max.y) {
					moveDirection = 1.0f;
					return AIAction.FALL_OFF_LEDGE;
				}
			}
		}

		// Walk towards target
		if (Mathf.Abs (target.position.x - transform.position.x) > xDelta) {
			moveDirection = enemyDirection;
			return AIAction.WALK_TOWARDS_TARGET;
		}

		return 	AIAction.NONE;
	}

	void OnTriggerEnter(Collider other) {
		HitBox health = other.gameObject.GetComponent<HitBox>();
		if (health != null) health.Damage(1);
	}
	
	private enum AIAction {
		NONE,
		WALK_TOWARDS_TARGET,
		FALL_OFF_LEDGE,
		JUMP_TO_PLATFORM,
		JUMP_AT_CHARACTER
	}

	public void Kill() {
		Destroy(gameObject);
	}

	public void KillFromAbove(HitBox other, Collider me) {
		Kill ();
	}

	public void KillFromBelow(float force) {
		Kill ();
	}
}
