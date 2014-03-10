using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all paltforms.
/// </summary>
public class Platform : MonoBehaviour {
	
	public Vector3 velocity {get; protected set;}
	public Transform myTransform {get; protected set;}
	
	/// <summary>
	/// If you override and return true for this, your platform will stop the character moving in the
	/// x direction. Instead you will need to the move the character from your platform code. See the 
	/// ropes classes as an example.
	/// </summary>
	virtual public bool overrideX {get {return false;} }
	
	/// <summary>
	/// If you override and return true for this, your platform will stop the character moving in the
	/// y direction. Instead you will need to the move the character from your platform code. See the
    /// ropes classes as an example.
	/// </summary>
	virtual public bool overrideY {get {return false;} }
	
	/// <summary>
	/// If you override and return true for this, your platform will stop the character moving in the
	/// x direction. instead you will need to the move character. See the ropes classes as an example.
	/// </summary>
	virtual public bool overrideAnimation {get {return false;} }
	
	void Start () {
		myTransform = transform;
		DoStart();
	}
	
	void Update(){
		Move();
		DoUpdate();
	}
	
	/// <summary>
	/// Override and add custom initialisation here.
	/// </summary>
	virtual protected void DoStart() {
	}
	
	/// <summary>
	/// Override with custom move code.
	/// </summary>
	virtual protected void Move() {
		myTransform.Translate(velocity * RaycastCharacterController.FrameTime);
	}
	
	/// <summary>
	/// Override with custom update code here.
	/// </summary>
	virtual protected void DoUpdate() {
	}
	
	/// <summary>
	/// This is called when a platform is hit. Override to implement platform behaviour.
	/// </summary>
	/// <param name='collider'>
	/// The collider that did the hitting.
	/// </param>
	/// <param name='character'>
	/// The character that did the hitting.
	/// </param>
	virtual public void DoAction(RaycastCollider collider, RaycastCharacterController character) {
		// Do nothing
	}
	
	/// <summary>
	/// Does this platform want to have this platform become the characters parent. Used for moving platforms.
	/// </summary>
	/// <returns>
	/// Return a transform if you want to reparent the character.
	/// </returns>
	virtual public Transform ParentOnStand(RaycastCharacterController character) {
		return null;	
	}
	
	/// <summary>
	/// Gets the animation for the character. Only called if you have set overrideAnimation to true.
	/// </summary>
	/// <returns>
	/// The animation state.
	/// </returns>
	virtual public CharacterState GetAnimationState(RaycastCharacterController character) {
		return CharacterState.IDLE;
	}
}
