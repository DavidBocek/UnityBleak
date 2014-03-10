using UnityEngine;
using System.Collections;
/**
 * Default animator for Solar Boy
 */ 
public class AlienAnimator2DTK: MonoBehaviour {
	
	public RaycastCharacterController controller;
	public tk2dSpriteAnimator playerSprite;
	
	private int currentDirection;
	private string suffix = "";

	void Start() {
		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
	}
	
	void Update ()
	{
		if (controller.characterInput.x != 0)
			CheckDirection ();
		
	}
	
	public void PowerUp() {
		suffix = "-Powered";
		playerSprite.Play("Change");
	}
	
	
	public void PowerDown() {
		suffix = "";
		playerSprite.Play("Change");
	}

	/// <summary>
	/// Respond to an animation event.
	/// </summary>
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		if (!playerSprite.IsPlaying("Change")) {	
			switch (state) {	
				case CharacterState.IDLE: Idle(); break;    
				case CharacterState.SLIDING: Slide(previousState); break;    
				case CharacterState.WALKING: Walk(); break; 
				case CharacterState.RUNNING: Walk(); break;  
				case CharacterState.JUMPING: Jump(); break; 
				case CharacterState.AIRBORNE: Jump(); break; 
				case CharacterState.FALLING: Fall(); break; 
				case CharacterState.DOUBLE_JUMPING: Jump(); break;  
				case CharacterState.WALL_JUMPING: Jump(); break;    
				case CharacterState.CROUCHING: Crouch(); break;    
				case CharacterState.CROUCH_SLIDING: Crouch(); break;    
				case CharacterState.STUNNED: Stun(); break;    
			}
		}
	}
	
	protected void Idle () {
		playerSprite.Play(  "Idle" + suffix);
		CheckDirection();
	}
	
	protected void Slide (CharacterState previousState) {
		if (previousState == CharacterState.AIRBORNE || previousState == CharacterState.FALLING) {
			playerSprite.Play("Idle" + suffix);
		}
	}
	
	protected void Walk () {
		playerSprite.Play("Walk" + suffix);
		CheckDirection();
	}
	
	protected void Crouch () {
		playerSprite.Play("Crouch" + suffix);
		CheckDirection();
	}
	
	protected void Jump() {
		playerSprite.Play("Jump" + suffix);
		CheckDirection();
	}
	
	protected void Fall() {
		playerSprite.Play("Fall" + suffix);
		CheckDirection();
	}
	
	
	protected void Stun () {
		playerSprite.Play("Stun" + suffix);
		CheckDirection();
	}

	protected void CheckDirection ()
	{
		if (controller.CurrentDirection != currentDirection) {
			currentDirection = controller.CurrentDirection;
			
			if (currentDirection > 0) {
				playerSprite.transform.localScale = new Vector3 (1, 1, 1);
				
			} else if (currentDirection < 0) {
				playerSprite.transform.localScale = new Vector3 (-1, 1, 1);
				
			}   
		}
	}
	
}
