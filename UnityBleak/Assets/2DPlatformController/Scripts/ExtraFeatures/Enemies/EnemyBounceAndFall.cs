using UnityEngine;
using System.Collections;
/// <summary>
/// This enemy uses only the MoveInX and MoveInY methods of the character controller. It behaves
/// much like Super Mario enemies in that it walks in a given direction unless
/// it runs in to something in which case it turns around.
///
/// This class is a good example of how you can extend/modify the core controller to create enemy (or 
/// alternatively player) variations.
/// </summary>
[RequireComponent (typeof(EnemyBounceAndFallInput))]
public class EnemyBounceAndFall : RaycastCharacterController , IEnemy {

	private bool hasHitPlayer;
	
	void Start() {
		characterInput = GetComponent<EnemyBounceAndFallInput>();
		// Stop all the fancy behaviour	
		crouch.canCrouch = false;	
		ledgeHanging.canLedgeHang = false;
		wall.canWallJump = false;
		wall.canWallSlide = false;
		climbing.allowClimbing = false;
	}


	/// <summary>
	/// We override the LateUpdate and provide our own simplified control sequence.
	/// </summary>
	void LateUpdate () {
		if (controllerActive) {
			frameTime = RaycastCharacterController.FrameTime;
			bool grounded = IsGrounded(groundedLookAhead);
			if (stunTimer > 0 ) {
				stunTimer -= frameTime;
				ForceSetCharacterState(CharacterState.STUNNED);
			} else {
				MoveInXDirection(grounded);
			}
			MoveInYDirection(grounded);
		}
		UpdateAnimation();
	}

	void OnTriggerEnter(Collider other) {
		HitBox health = other.gameObject.GetComponent<HitBox>();
		if (health != null && !hasHitPlayer) {
			health.Damage(1);
			if (health.simplehealth != null && health.simplehealth.Health < 1) hasHitPlayer = true;
			// Walk opposite dir
			((EnemyBounceAndFallInput)characterInput).direction *= -1;
		}
	}

	public void Kill() {
		hasHitPlayer = true;
		collider.enabled = false;
		StartCoroutine(DoDie ());
	}
	
	private IEnumerator DoDie() {
		Stun (((EnemyBounceAndFallInput)characterInput).stunTime);
		if (renderer != null) renderer.enabled = false;
		
		if (particleSystem != null) particleSystem.Play ();
		yield return new WaitForSeconds(((EnemyBounceAndFallInput)characterInput).stunTime);
		Destroy(gameObject);
	}

	public void KillFromAbove(HitBox other, Collider me) {
		if (!hasHitPlayer && other != null && other.simplehealth != null) {
			// If we can find a character controller 
			RaycastCharacterController hero = other.simplehealth.GetComponent<RaycastCharacterController>();
			if (hero != null) {
				me.collider.enabled = false;
				Kill();
				hero.Velocity = new Vector2(hero.Velocity.x, ((EnemyBounceAndFallInput)characterInput).bounceVelocity);
			}
		}
	}
	
	public void KillFromBelow(float force) {
		if (!hasHitPlayer) {
			Velocity = new Vector2(Velocity.x, 30.0f);
			Kill();
		}
	}
}