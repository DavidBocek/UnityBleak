using UnityEngine;
using System.Collections;

/// <summary>
/// Add this to a character to enable the character to take fall damage.
/// </summary>
public class FallDamage : MonoBehaviour {
	
	/// <summary>
	/// Character taking the damage.
	/// </summary>
	public RaycastCharacterController controller;

	/// <summary>
	/// How fast we need to be falling before we take fall damage.
	/// </summary>
	public float minSpeedForAction = -8.0f;

	/// <summary>
	/// What action to take when a fall occurs.
	/// </summary>
	public FallDamageAction fallDamageAction;

	/// <summary>
	/// How long to stun the character after a fall.
	/// </summary>
	public float stunTime = 1.0f;

	private bool isFalling;
	private float fallSpeed;

	void Start() {
		if (controller == null) controller = gameObject.GetComponent<RaycastCharacterController>();
		if (controller == null) {
			Debug.LogError("FallDamage script not attached to a character");
		} else {
			// Register listeners
			controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
		}
	}
	
	void Update() {
		// keep track of the velocity each frame.
		if (isFalling && controller.Velocity.y < fallSpeed) fallSpeed = controller.Velocity.y;
	}

	/// <summary>
	/// Respond to an animation event. We use this to track when the character is falling
	/// and when the character lands.
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	/// <param name='previousState'>
	/// Previous state.
	/// </param>
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		if (isFalling) {
			switch (state) {
				case CharacterState.IDLE: Land(); break;	
				case CharacterState.WALKING: Land(); break;	
				case CharacterState.RUNNING: Land(); break;	
				case CharacterState.SLIDING: Land(); break;	
				case CharacterState.FALLING: break;	
				default: isFalling = false; break;
			}
		}
		else if (state == CharacterState.FALLING) isFalling = true;
	}
	
	private void Land () {
		isFalling = false;
		if (fallSpeed <= minSpeedForAction) {
			switch (fallDamageAction) {
				case FallDamageAction.STUN: controller.Stun(stunTime); break;
				case FallDamageAction.SEND_MESSAGE_DIE: controller.SendMessage("Die", null,SendMessageOptions.DontRequireReceiver );  break;
				case FallDamageAction.SEND_MESSAGE_HIT: controller.SendMessage("Hit", fallSpeed, SendMessageOptions.DontRequireReceiver); break;
				case FallDamageAction.SIMPLE_HEALTH_DAMAGE: controller.GetComponent<SimpleHealth>().FallDamage(fallSpeed); break;
			}
		}
		fallSpeed = 0.0f;
	}

	void OnDestroy() {
		if (controller != null) controller.CharacterAnimationEvent -= new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
	}
}

public enum FallDamageAction {
  	// Stun the character
	STUN,
	// Send a message called "Die"
	SEND_MESSAGE_DIE,
	// Send a message called "Hit"
	SEND_MESSAGE_HIT,
	// Use the supplied Simple Health script to take damage
	SIMPLE_HEALTH_DAMAGE
}