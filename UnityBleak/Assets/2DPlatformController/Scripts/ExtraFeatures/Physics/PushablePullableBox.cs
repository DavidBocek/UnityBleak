using UnityEngine;
using System.Collections;

/// <summary>
/// A pushable pullable/box that works like a box in a puzzle game. Pull requires a character with a box puller attached to it.
/// Boxes cannot be pushed or pulled if they have another box on top of them. This is an EXTRA FEATURE release which means it will work
/// in many situations but is not considered a robust solution for all possible scenarios.
///
/// These boxes DONT work with slopes or moving platforms.
///
/// Implementaiton wise the box is both a collider and a simplified character with its own raycast colliders, etc
/// </summary>
public class PushablePullableBox : Platform {

	/// <summary>
	/// The sides of the box (just like a character.
	/// </summary>
	public RaycastCollider[] sides; 

	/// <summary>
	/// The bottoms of the box (just like a character.
	/// </summary>
	public RaycastCollider[] bottom;

	/// <summary>
	/// The layer the box collides against.
	/// </summary>
	public int backgroundLayer = 0;

	/// <summary>
	/// The layer the object is moved to when it is being pushed/pulled. This allows the character to move through it.
	/// </summary>
	public int latchedLayer = 22;

	/// <summary>
	/// 
	/// </summary>
	public float groundedLookAhead = 0.05f;

	/// <summary>
	/// 
	/// </summary>
	public float minVelocityForUnLatch = 0.5f;

	/// <summary>
	/// The movement settings, see inline docs on movement details defintion for details.
	/// </summary>
	public MovementDetails movement;

	private RC_Direction direction;

	private RaycastCharacterController controller;

	private bool hasPulled;

	private Platform myParent;

	/// <summary>
	/// Gets or sets a value indicating whether this box has another box on top of it.
	/// </summary>
	public bool BoxOnTop {
		get; set;
	}

	/// <summary>
	/// Unity update hook.
	/// </summary>
	void Update () {
		BoxOnTop = false;
		velocity = new Vector2(0.0f, velocity.y);
		if (controller != null) {
			// If the controller is doing something other than walking/running/pushing/etc then UnLatch it
			if (controller.State != CharacterState.IDLE && controller.State != CharacterState.WALKING && controller.State != CharacterState.RUNNING && 
			    controller.State != CharacterState.PUSHING && controller.State != CharacterState.PULLING) {
				UnLatch();
			} 
			// If you pull a box then release then unlatch.
			else if (hasPulled && controller.characterInput.x == 0) {
				UnLatch();
			} 
			// If character is not grounded then unlatch.
			 else if (controller.GroundedFeetCount <= 1) {
				UnLatch();
			} 
			// If character is moving too fast then unlatch.
			else if (velocity.y > minVelocityForUnLatch || velocity.y  < (minVelocityForUnLatch * -1)) {
				UnLatch();
			} else {
				// TODO Why am I checking this twice?!?
				if (controller != null) {
					velocity = new Vector2(controller.Velocity.x, 0.0f);
					// Animation
					if (direction == RC_Direction.RIGHT && controller.characterInput.x > 0) controller.ForceSetCharacterState(CharacterState.PUSHING);
					else if (direction == RC_Direction.RIGHT && controller.characterInput.x < 0) {
						controller.ForceSetCharacterState(CharacterState.PULLING);
						hasPulled = true;
					}
					else if (direction == RC_Direction.LEFT && controller.characterInput.x < 0) controller.ForceSetCharacterState(CharacterState.PUSHING);
					else if (direction == RC_Direction.LEFT  && controller.characterInput.x > 0) {
						controller.ForceSetCharacterState(CharacterState.PULLING);
						hasPulled = true;
					}
				}
			}
		}
		
	}

	/// <summary>
	/// Moves during late update like a normal character.
	/// </summary>
	void LateUpdate() {
		bool grounded = IsGrounded(groundedLookAhead);
		if (!grounded) UnLatch();
		MoveInY(grounded);
		// Move in X
		myTransform.Translate (velocity.x * RaycastCharacterController.FrameTime, 0.0f, 0.0f);
		for (int i = 0; i < sides.Length; i++) {
			RaycastHit hitSides;
			hitSides = sides [i].GetCollision (1 << backgroundLayer | 1 << latchedLayer);
			if (hitSides.collider != null) {
				velocity = new Vector2(0.0f, velocity.y);
				float forceSide = (hitSides.normal * (sides [i].distance - hitSides.distance)).x;
				gameObject.layer = backgroundLayer;
				if (forceSide > movement.skinSize) {	
					myTransform.Translate(Mathf.Max(velocity.x * RaycastCharacterController.FrameTime * -1, forceSide), 0.0f, 0.0f);		
				} else if (-1 * forceSide > movement.skinSize) {		
						myTransform.Translate(Mathf.Min(velocity.x * RaycastCharacterController.FrameTime * -1, forceSide), 0.0f, 0.0f);
				}	
				break;
			}
		}
	}

	private void MoveInY (bool grounded) {

		// Limit Velocity
		if (velocity.y < movement.terminalVelocity)
			velocity = new Vector2(velocity.x, movement.terminalVelocity);
		
		// Apply velocity
		if ( (velocity.y > movement.skinSize || velocity.y * -1 > movement.skinSize)) {
			myTransform.Translate (0.0f, velocity.y * RaycastCharacterController.FrameTime, 0.0f, Space.World);
		}
		
		// Fall/Stop
		bool hasHitFeet = false;
		if (velocity.y <= 0.0f ) {
			float maxForce = 0.0f;
			GameObject hitGameObject = null;
			
			foreach (RaycastCollider feetCollider in bottom) {
				RaycastHit hitFeet = new RaycastHit ();
				float closest = float.MaxValue;
				RaycastHit[] feetCollisions = feetCollider.GetAllCollision (1 << backgroundLayer | 1 << latchedLayer);						
				// Get closest collision
				foreach (RaycastHit collision in feetCollisions) {
					if (collision.distance < closest) {
						hitFeet = collision;
						closest = collision.distance;
					}
					// Resting on a platform
					if (hitFeet.collider != null) {
						Platform platform = hitFeet.collider.gameObject.GetComponent<Platform> ();
						if (platform != null && feetCollider.distance >= hitFeet.distance) {
							if (platform is PushablePullableBox) {
								((PushablePullableBox)platform).BoxOnTop = true;
							}
							Transform parentPlatform = platform.ParentOnStand (null);
							if (parentPlatform != null) {
								myParent = platform;
								
								if (myTransform.parent != parentPlatform) {
									myTransform.parent = parentPlatform;
								}
								hitGameObject = hitFeet.collider.gameObject;
							}
						}
					}
				}
				
				float force = (hitFeet.normal * (feetCollider.distance - hitFeet.distance)).y;	
				
				// Get force to apply
				
				if (force > maxForce) {
					hasHitFeet = true;
					maxForce = force;
					hitGameObject = hitFeet.collider.gameObject;
				}
			}
			
			if (hasHitFeet) {
				if (myParent == null) {
					myTransform.Translate (0.0f, maxForce, 0.0f, Space.World);	
				}
				velocity = new Vector3(velocity.x, 0.0f);
				if (myParent != null && hitGameObject != myParent.gameObject) {
					myParent = null;
					myTransform.parent = null;
				}
				grounded = true;
			} else {
				if (myParent == null) {
					ApplyGravity ();
				} else if (!grounded) {
					ApplyGravity ();
					myParent = null;
					myTransform.parent = null;
				}	
			}

		} else {
			ApplyGravity ();
		}
		
	}

	/// <summary>
	/// Latch the specified controller on to the box.
	/// </summary>
	/// <param name="controller">Controller to latch.</param>
	/// <param name="direction">Direction of the latch.</param>
	private void Latch(RaycastCharacterController controller, RC_Direction direction) {
		BoxPuller puller = controller.GetComponent<BoxPuller>();
		if (IsGrounded(groundedLookAhead) && (puller == null || puller.CanLatch(this) )) {
			this.direction = direction;
			this.controller = controller;
			gameObject.layer = latchedLayer;
		}
	}
	
	/// <summary>
	/// Unlatch the latched controller
	/// </summary>
	private void UnLatch() {	
		if (controller != null) {
			BoxPuller puller = controller.GetComponent<BoxPuller>();
		  	if (puller != null) puller.ReleaseBox();
		}
		gameObject.layer = backgroundLayer;
		controller = null;
	}

	/// <summary>
	/// When hit from the sides latch the character (unless we have a box on top of us).
	/// </summary>
	override public void DoAction(RaycastCollider collider, RaycastCharacterController controller) {
		if (!BoxOnTop) {
			if (collider.direction == RC_Direction.LEFT) {
				Latch(controller, RC_Direction.LEFT);
			} else if (collider.direction == RC_Direction.RIGHT) {
				Latch(controller, RC_Direction.RIGHT);
			}
		} else {
			UnLatch();
		}
	}

	/// <summary>
	/// Determines whether this instance is grounded for the specified offset.
	/// </summary>
	public bool IsGrounded(float offset, bool includeClimables = true){
		foreach (RaycastCollider foot in bottom) {
			if (foot.IsColliding(1 << backgroundLayer , offset)) return true;
		}
		return false;
	}

	/// <summary>
	/// Applies the gravity.
	/// </summary>
	private void ApplyGravity () {
		velocity = new Vector2(velocity.x, velocity.y + (RaycastCharacterController.FrameTime * Physics.gravity.y));
	}
	
}
