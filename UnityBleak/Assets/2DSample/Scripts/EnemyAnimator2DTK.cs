using UnityEngine;
using System.Collections;
/**
 * Default animator for Solar Boy
 */ 
public class EnemyAnimator2DTK: MonoBehaviour {
	
	public RaycastCharacterController controller;
	public tk2dSpriteAnimator playerSprite;
	public string enemyName;

	private int currentDirection;
	
	void Start() {
		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
	}
	
	void Update ()
	{
		if (controller.characterInput.x != 0)
			CheckDirection ();
		
	}
	
	
	/// <summary>
	/// Respond to an animation event.
	/// </summary>
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		
		switch (state) {	
			case CharacterState.IDLE: Idle(); break;     
			case CharacterState.WALKING: Walk(); break; 
			case CharacterState.RUNNING: Walk(); break;  
			case CharacterState.JUMPING: Fall(); break; 
			case CharacterState.AIRBORNE: Fall(); break; 
			case CharacterState.FALLING: Fall(); break; 
			case CharacterState.DOUBLE_JUMPING: Fall(); break;  
			case CharacterState.WALL_JUMPING: Fall(); break;    
			case CharacterState.STUNNED: Stun(); break;    
		}
		
	}
	
	protected void Idle () {
		playerSprite.Play(enemyName + "Idle");
		CheckDirection();
	}
	
	protected void Slide (CharacterState previousState) {
		if (previousState == CharacterState.AIRBORNE || previousState == CharacterState.FALLING) {
			playerSprite.Play(enemyName + "Idle");
		}
		
	}
	
	protected void Walk () {
		playerSprite.Play(enemyName + "Walk");
		CheckDirection();
	}
	
	protected void Stun () {
		playerSprite.Play(enemyName + "Stun");
		CheckDirection();
	}
	
	protected void Jump() {
		playerSprite.Play(enemyName + "Jump");
		CheckDirection();
	}
	
	protected void Fall() {
		playerSprite.Play(enemyName + "Fall");
		CheckDirection();
	}
	
	protected void CheckDirection ()
	{
		if (controller.CurrentDirection != currentDirection) {
			currentDirection = controller.CurrentDirection;
			// Directions are opposite to character simply because the sprites face the opposite way in the sprite sheet
			if (currentDirection > 0) {
				playerSprite.transform.localScale = new Vector3 (-1, 1, 1);
				
			} else if (currentDirection < 0) {
				playerSprite.transform.localScale = new Vector3 (1, 1, 1);
				
			}   
		}
	}
	
}
