using UnityEngine;
using System.Collections;

public class BleakController : MonoBehaviour {
	
	//============ standard vars =============
	//public vars
	public CharacterController controller;
	public Rigidbody rigidBody;
	public BoxCollider boxCollider;
	public int runSpeed = 350;
	public float joggingMultiplier = .6f;
	public int jumpSpeed = 250;
	public int maxFallSpeed = 400;
	public int gravityAcceleration = 600;
	public float slamMultiplayer;
	public bool canControl = true;
	public Transform startPoint;
	public float fallKillSpeed;
	public float timeUntilRun = 1.75f;
	public float bottomOffsetPushIn;
	public float sideOffsetPushIn;
	
	public const float SMALL_JUMP_TIME = .06f;
	public const float MED_JUMP_TIME = .08f;
	public const float LARGE_JUMP_TIME = .12f;
	
	//private vars
	private bool idleSwitch = true;
	private float idleDelay	= 0f;
	private float runDelay = 0f;
	private float slamDelay = 0f;
	private float jumping = 0f;
	private bool slamming = false;
	private bool slamLanding = false;
	private Vector3 velocity = new Vector3 (0,0,0);
	private Vector3 moveDelta = new Vector3 (0,0,0);
	private bool facing = true;
	private bool isGrounded;
	private Climbable attachedClimbObject;
	private KeyCode actionButton = KeyCode.E;
	
	private const bool LEFT = false;
	private const bool RIGHT = true;
	
	

	//=========== states =========
	private short state;
	
	public const short STATE_NORMAL = 0;
	public const short STATE_CLIMBING_LEFT = 1;
	public const short STATE_CLIMBING_RIGHT = 2;
	public const short STATE_DYING = 3;
	public const short STATE_DEAD = 4;
	public const short STATE_HURTING = 5;
	public const short STATE_HURT = 6;
	public const short STATE_FORCE_MOVE = 7;	public Transform forceMoveDestination;	public float forceMoveSpeed;
	
	
	//=========== rays ===========
	//bottom
	private Ray leftRayBottom;
	private Ray centerRayBottom;
	private Ray rightRayBottom;
	private float rayLengthBottom;
	private RaycastHit rayHitInfoBottom;
	private Vector3 bottomOffset;
	//sides
	private Ray topRayRight;
	private Ray centerRayRight;
	private Ray bottomRayRight;
	private RaycastHit rayHitInfoRight;
	private Ray topRayLeft;
	private Ray centerRayLeft;
	private Ray bottomRayLeft;
	private RaycastHit rayHitInfoLeft;
	private float rayLengthSide;
	private Vector3 sideOffset;
	
	private float deltaY;
	private float deltaX;
	
	private float dt;
	private float fdt;
	// Use this for initialization
	void Start () {
		transform.position = startPoint.position;
		
		rayLengthBottom = (boxCollider.size.y / 2f)+1f;
		rayLengthSide = (boxCollider.size.x / 2f)+1f;
		
		bottomOffset = new Vector3(boxCollider.size.x/2-bottomOffsetPushIn,0,0);
		sideOffset = new Vector3(0,boxCollider.size.y/2-sideOffsetPushIn,0);
		
		leftRayBottom = new Ray(transform.position - bottomOffset + boxCollider.center,Vector3.down);
		centerRayBottom = new Ray(transform.position + boxCollider.center,Vector3.down);
		rightRayBottom = new Ray(transform.position + bottomOffset + boxCollider.center, Vector3.down);
		topRayRight = new Ray(transform.position + sideOffset + boxCollider.center,Vector3.right);
		centerRayRight = new Ray(transform.position + boxCollider.center,Vector3.right);
		bottomRayRight = new Ray(transform.position - sideOffset + boxCollider.center,Vector3.right);
		topRayLeft = new Ray(transform.position + sideOffset + boxCollider.center,Vector3.left);
		centerRayLeft = new Ray(transform.position + boxCollider.center,Vector3.left);
		bottomRayLeft = new Ray(transform.position - sideOffset + boxCollider.center,Vector3.left);
	}
	
	// Update is called once per frame
	float tempVelVert;
	void Update () {
		switch (state){
		case STATE_NORMAL:
			velocity.x = 0;
			dt = Time.deltaTime;
			UpdateRays(dt);
			if (canControl){
				//enable gravity if no raycasts return true from the three downward rays
				if (!Physics.Raycast(leftRayBottom,out rayHitInfoBottom,rayLengthBottom) && !Physics.Raycast(centerRayBottom,out rayHitInfoBottom,rayLengthBottom) && !Physics.Raycast(rightRayBottom,out rayHitInfoBottom,rayLengthBottom)){
					tempVelVert = velocity.y - gravityAcceleration * dt;
					velocity.y = Mathf.Max(tempVelVert, -maxFallSpeed);
					isGrounded = false;
				//otherwise deal with collisions from the top if the object is not ignoring them
				} else if (Physics.Raycast(leftRayBottom,out rayHitInfoBottom,rayLengthBottom) || Physics.Raycast(centerRayBottom,out rayHitInfoBottom,rayLengthBottom) || Physics.Raycast(rightRayBottom,out rayHitInfoBottom,rayLengthBottom)){
					bool collidableDown = true;
					bool collidableUp = true;
					bool aboveCollider = true;
					if (rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>()!=null){ 
						collidableDown = !rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreUp();
						collidableUp = !rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreDown();
						//have to check that the object is below bleak, or it will catch the raycasts which start from the center y line of bleak's collider
						aboveCollider = (rayHitInfoBottom.transform.position.y+rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2 + rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().center.y) <= (transform.position.y - boxCollider.size.y/2 + boxCollider.center.y);
						Debug.Log ("above collider: "+aboveCollider+" object: "+(rayHitInfoBottom.transform.position.y+rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2-5)+" player: "+(transform.position.y - boxCollider.size.y/2));
						//Debug.Log("object collider y/2: "+(rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2)+" player collider y/2: "+(boxCollider.size.y/2));
					}
					Debug.DrawRay(leftRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
					Debug.DrawRay(centerRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
					Debug.DrawRay(rightRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
					//falling
					if (collidableDown && aboveCollider && velocity.y <= 0){
						HandleBottomCollision(rayHitInfoBottom,dt);	
					} else {
						tempVelVert = velocity.y - gravityAcceleration * dt;
						velocity.y = Mathf.Max(tempVelVert, -maxFallSpeed);
					}
				}
				UpdateInputNormal(dt);
				UpdateIdle(dt);
				UpdateLeftRight(dt);
			} else {
				//accel y is 0, so velocity.y shouldn't change. walking up stairs and such
				//DEPRECATED canControl? using FORCE_MOVE and death states should replace this i think?
			}
			UpdateRotationNormal(dt);
			UpdatePositionChangeNormal(dt);
			break;
		case STATE_CLIMBING_LEFT:
			UpdateRays (dt);
			if (Input.GetKey (KeyCode.W)){
				velocity.y = attachedClimbObject.climbSpeed;
			} else if (Input.GetKey(KeyCode.S)){
				velocity.y = -attachedClimbObject.climbSpeed * 2/3;
			}
			UpdatePositionChangeClimbing(dt);
			break;
		case STATE_CLIMBING_RIGHT:
			UpdateRays (dt);
			if (Input.GetKey (KeyCode.W)){
				velocity.y = attachedClimbObject.climbSpeed;
			} else if (Input.GetKey(KeyCode.S)){
				velocity.y = -attachedClimbObject.climbSpeed * 2/3;
			}
			UpdatePositionChangeClimbing(dt);
			break;
		case STATE_FORCE_MOVE:
			UpdateForceMove(dt);
			break;
		case STATE_HURT:
			transform.position = startPoint.position;
			SetState(STATE_NORMAL);
			break;
		case STATE_DEAD:
			if (Input.GetKeyDown(KeyCode.R)){
				transform.position = startPoint.position;
				numLives = 3;
				SetState(STATE_NORMAL);
			}
			break;
		}
		if (Debug.isDebugBuild)
			UpdateDebug(dt);
	}
	
	void FixedUpdate(){
		fdt = Time.fixedDeltaTime;
		switch (state){
		case STATE_NORMAL:
			UpdateJump(fdt);
			break;
		}
	}
		
	
	/// <summary>
	/// Updates the idle animations.
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateIdle(float dt){
		if (jumping>=0){
			//handle idle animations
			if (idleDelay >= 7)
				idleDelay = 0;
			else{
				if (Input.GetAxisRaw("Horizontal") == 0 && !Input.GetKey(KeyCode.Space) && idleDelay <=6){
					//play idle1
					idleDelay += dt;
					runDelay = 0;
				} else if (Input.GetAxisRaw("Horizontal") == 0 && !Input.GetKey(KeyCode.Space)){
					switch (idleSwitch){
					case true:
						//play idle2
						idleSwitch=false;
						break;
					case false:
						//play idle3
						idleSwitch=true;
						break;
					}
					idleDelay += dt;
					runDelay = 0;
				}
			}
		}
	}
	
	/// <summary>
	/// Updates movement while in a force move
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateForceMove(float dt){
		if ((transform.position - forceMoveDestination.position).sqrMagnitude >= .25f){
			velocity = (forceMoveDestination.position-transform.position).normalized * forceMoveSpeed;
		} else {
			Messenger.Broadcast<Transform,BleakController>("CompletedForceMove",forceMoveDestination,this);
		}
	}
	
	/// <summary>
	/// Updates x movement based on controls.
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateLeftRight(float dt){
		bool obstructedRight = false;
		bool obstructedLeft = false;
		float directionInt = Input.GetAxisRaw("Horizontal");
		if (directionInt < 0)
			facing = LEFT;
		if (directionInt >0)
			facing = RIGHT;
		
		//raycast 3 rays from the right to check collisions
		if (Physics.Raycast(topRayRight,out rayHitInfoRight, rayLengthSide) || Physics.Raycast(centerRayRight,out rayHitInfoRight, rayLengthSide) || Physics.Raycast(bottomRayRight,out rayHitInfoRight, rayLengthSide)){
			bool collidableRight = true;
			if (rayHitInfoRight.transform.gameObject.GetComponent<IgnoreCollisions>()) 
				collidableRight = !rayHitInfoRight.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreLeft();
			if (collidableRight){
				HandleSideCollision(rayHitInfoRight,dt);
				HandleRightCollision(rayHitInfoRight,dt);
				obstructedRight = true;
			} else { obstructedRight = false; }
		} else
			obstructedRight = false;
		
		//raycast 3 rays from the left to check collisions
		if (Physics.Raycast(topRayLeft,out rayHitInfoLeft, rayLengthSide) || Physics.Raycast(centerRayLeft,out rayHitInfoLeft, rayLengthSide) || Physics.Raycast(bottomRayLeft,out rayHitInfoLeft, rayLengthSide)){
			bool collidableLeft = true;
			if (rayHitInfoLeft.transform.gameObject.GetComponent<IgnoreCollisions>())
				collidableLeft = !rayHitInfoLeft.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreRight();
			if (collidableLeft){
				HandleSideCollision(rayHitInfoLeft,dt);
				HandleLeftCollision(rayHitInfoLeft,dt);
				obstructedLeft = true;
			} else { obstructedLeft = false; }
		} else
			obstructedLeft = false;
		
		
		//player is hitting direction
		if (directionInt != 0){
			idleDelay = 0;
			if (runDelay < timeUntilRun){
				if ((facing && !obstructedRight) || (!facing && !obstructedLeft)) velocity.x = runSpeed * joggingMultiplier * directionInt;
				if (jumping != -1){
					//play jogging anim
				}
				runDelay += dt;
			} else {
				if ((facing && !obstructedRight) || (!facing && !obstructedLeft)) velocity.x = runSpeed * directionInt;
				if (jumping != -1){
					//play running anim
				}
			}
		} else {	//otherwise horizontal direction isn't being applied
			runDelay = 0;
		}
	}
	
	
	/// <summary>
	/// Updates jump logic (including slams)
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateJump(float dt){
		
		//jumping is 0 when on the ground, >0 for a split second while the jump impulse is occuring, and -1 after that before he hits the ground (so that you can't jump again in air)
		if (Input.GetKey(KeyCode.Space) && jumping >= 0){
			//play jump sound
			jumping += dt;
			if (jumping >= LARGE_JUMP_TIME)
					jumping = -1;
		} else {
			jumping = -1;
		}
		if (jumping > 0){
			//Debug.Log ("jumping next: "+jumping);
			if (jumping < SMALL_JUMP_TIME){
				velocity.y = jumpSpeed*2/3;
			}
			else if (jumping < MED_JUMP_TIME){
				velocity.y = jumpSpeed*3/4;
			}
			else{
				velocity.y = jumpSpeed;
			}
		}
		//reset jump component, this should only run in the frame after bleak hits the ground.
		if (isGrounded && jumping == -1){
			if (slamming == true)
				slamLanding = true;
			slamming = false;
			jumping = 0;
		}
		
		//beginning a slam
		if (jumping == -1 && Input.GetKeyDown(KeyCode.LeftShift)){
			velocity.y = -jumpSpeed * slamMultiplayer;
			slamming = true;
			//play slamming animation and sound
		}
		
		//landing from a slam (not used right now)
		if (slamLanding){
			slamLanding=false;
			slamDelay=0;
			runDelay=0;
		}
	}
	
	
	/// <summary>
	/// Handles input while in the normal state
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateInputNormal(float dt){
		if (Input.GetKeyDown(actionButton)){
			//send action message on the position, listeners will handle the action if the position is within their bounds
			Messenger.Broadcast<Transform,BleakController>("BleakActionActivate",transform,this);
		}
	}
	
	
	
	/// <summary>
	/// Updates rotation
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateRotationNormal(float dt){
		if (!facing && transform.rotation.y <= 0.1f){
			transform.Rotate(0,180,0);
		}
		if (facing && transform.rotation.y >= .9f){
			transform.Rotate (0,180,0);
		}
	}
	
	
	/// <summary>
	/// Updates the position change.
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdatePositionChangeNormal(float dt){
		velocity.z=0;
		moveDelta = velocity * dt;
		RaycastHit interpolateInfo;
		
		//== deal with interpolate collisions down ==
		deltaY = moveDelta.y;
		if (deltaY < 0){
			Vector3 deltaYRayOriginLeft = new Vector3(transform.position.x-boxCollider.size.x/2+bottomOffsetPushIn,transform.position.y - boxCollider.size.y/2,transform.position.z);
			Vector3 deltaYRayOriginCenter = new Vector3(transform.position.x,transform.position.y - boxCollider.size.y/2,transform.position.z);
			Vector3 deltaYRayOriginRight = new Vector3(transform.position.x+boxCollider.size.x/2-bottomOffsetPushIn,transform.position.y - boxCollider.size.y/2,transform.position.z);
			if (Physics.Raycast(deltaYRayOriginLeft,Vector3.down,out interpolateInfo, -(deltaY)) || Physics.Raycast(deltaYRayOriginCenter,Vector3.down,out interpolateInfo, -(deltaY)) || Physics.Raycast(deltaYRayOriginRight,Vector3.down,out interpolateInfo, -(deltaY))){			
				moveDelta.y = -interpolateInfo.distance;
				//Debug.Log ("overmovement detected! setting new movement distance from: "+deltaY+" to: "+interpolateInfo.distance+" movedelta.y becoming: "+moveDelta.y);
			}
		}
		//== deal with interpolate collisions sideways ==
		deltaX = moveDelta.x;
		if(deltaX != 0){
			Vector3 deltaXRayOriginRightTop = new Vector3(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn,transform.position.z);
			Vector3 deltaXRayOriginRightCenter = new Vector3(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y,transform.position.z);
			Vector3 deltaXRayOriginRightBottom = new Vector3(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn,transform.position.z);
			Vector3 deltaXRayOriginLeftTop = new Vector3(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn,transform.position.z);
			Vector3 deltaXRayOriginLeftCenter = new Vector3(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y,transform.position.z);
			Vector3 deltaXRayOriginLeftBottom = new Vector3(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn,transform.position.z);
			if ((Physics.Raycast(deltaXRayOriginRightTop,Vector3.right,out interpolateInfo,Mathf.Abs(deltaX)) || Physics.Raycast(deltaXRayOriginRightCenter,Vector3.right,out interpolateInfo,Mathf.Abs(deltaX)) || Physics.Raycast(deltaXRayOriginRightBottom,Vector3.right,out interpolateInfo,Mathf.Abs(deltaX))) && deltaX > 1)
				moveDelta.x = interpolateInfo.distance;
			else if ((Physics.Raycast(deltaXRayOriginLeftTop,Vector3.left,out interpolateInfo,Mathf.Abs(deltaX)) || Physics.Raycast(deltaXRayOriginLeftCenter,Vector3.left,out interpolateInfo,Mathf.Abs(deltaX)) || Physics.Raycast(deltaXRayOriginLeftBottom,Vector3.left,out interpolateInfo,Mathf.Abs(deltaX))) && deltaX < -1)
				moveDelta.x = -interpolateInfo.distance;
		}
		controller.Move (moveDelta);
	}
	
	/// <summary>
	/// Updates the position change climbing.
	/// </summary>
	/// <param name='dt'>
	/// time since last frame (in seconds).
	/// </param>
	void UpdatePositionChangeClimbing(float dt){
		velocity.z = 0;
		moveDelta = velocity * dt;
		RaycastHit interpolateInfo;
		Vector3 deltaXRayOriginRightTop = new Vector3(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn,transform.position.z);
		Vector3 deltaXRayOriginRightCenter = new Vector3(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y,transform.position.z);
		Vector3 deltaXRayOriginRightBottom = new Vector3(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn,transform.position.z);
		Vector3 deltaXRayOriginLeftTop = new Vector3(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn,transform.position.z);
		Vector3 deltaXRayOriginLeftCenter = new Vector3(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y,transform.position.z);
		Vector3 deltaXRayOriginLeftBottom = new Vector3(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn,transform.position.z);
		if (Physics.Raycast(deltaXRayOriginRightTop,Vector3.right,out interpolateInfo,1f) || Physics.Raycast(deltaXRayOriginRightCenter,Vector3.right,out interpolateInfo,1f) || Physics.Raycast(deltaXRayOriginRightBottom,Vector3.right,out interpolateInfo,1f))
			if (interpolateInfo.transform.gameObject.GetComponent<KillZones>().HasKillLeft ())
				Damage();
			else if (interpolateInfo.transform.gameObject.GetComponent<Climbable>() == null)
				SetState(STATE_NORMAL);
		else if (Physics.Raycast(deltaXRayOriginLeftTop,Vector3.left,out interpolateInfo,1f) || Physics.Raycast(deltaXRayOriginLeftCenter,Vector3.left,out interpolateInfo,1f) || Physics.Raycast(deltaXRayOriginLeftBottom,Vector3.left,out interpolateInfo,1f))
			if (interpolateInfo.transform.gameObject.GetComponent<KillZones>().HasKillRight ())
				Damage();
			else if (interpolateInfo.transform.gameObject.GetComponent<Climbable>() == null)
				SetState(STATE_NORMAL);
		else{
			moveDelta.x *=2;
			moveDelta.y += 5f;
			SetState(STATE_NORMAL);
		}
		
		controller.Move (moveDelta);
	}
	
	/// <summary>
	/// Updates the rays used for raycasting to check collisions
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateRays(float dt){
		leftRayBottom.origin=transform.position - bottomOffset + boxCollider.center;
		centerRayBottom.origin=transform.position + boxCollider.center;
		rightRayBottom.origin=transform.position + bottomOffset + boxCollider.center;
		
		topRayRight.origin=transform.position+sideOffset+boxCollider.center;
		centerRayRight.origin=transform.position+boxCollider.center;
		bottomRayRight.origin=transform.position-sideOffset+boxCollider.center;
		
		topRayLeft.origin=transform.position+sideOffset+boxCollider.center;
		centerRayLeft.origin=transform.position+boxCollider.center;
		bottomRayLeft.origin=transform.position-sideOffset+boxCollider.center;
	}
	
	
	/// <summary>
	/// Update step for any debugging happening
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateDebug(float dt){
		Debug.DrawRay(transform.position - bottomOffset + boxCollider.center,Vector3.down*rayLengthBottom);
		Debug.DrawRay(transform.position + boxCollider.center,Vector3.down*rayLengthBottom);
		Debug.DrawRay(transform.position + bottomOffset + boxCollider.center, Vector3.down*rayLengthBottom);
		if (Input.GetKeyDown (KeyCode.Q))
			Debug.Break();
	}
	
	
	/// <summary>
	/// Handles a collision on the bottom side
	/// </summary>
	/// <param name='hitInfo'>
	/// Hit info used to check distance and what was hit
	/// </param>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void HandleBottomCollision(RaycastHit hitInfo, float dt){
		if (velocity.y > Mathf.Abs(fallKillSpeed) && !IsSlamming()) Damage ();
		else if (velocity.y < 0) velocity.y = 0;
		/*if (hitInfo.distance < (rayLengthBottom - 1f) && jumping == 0 && hitInfo.collider!=controller.collider){
			Vector3 correctionDelta = Vector3.zero;
			correctionDelta.y = rayLengthBottom - hitInfo.distance;
			if (correctionDelta.y > .5f){
				Debug.Log ("sitting under ground, correcting! distance: "+hitInfo.distance+" desired distance: "+rayLengthBottom+" correcting by: "+correctionDelta.y);
				controller.Move(correctionDelta);
			}
		}*/
		isGrounded = true;
		//kill zones component check, to see if bleak should die touching this
		KillZones killZones = hitInfo.transform.gameObject.GetComponent<KillZones>();
		if (killZones){
			if (killZones.HasKillUp()){
				Damage();
			}
		}
		//bounce off component check, to see if bleak should spring off this
		BounceOffTop bounceOff = hitInfo.transform.gameObject.GetComponent<BounceOffTop>();
		if (bounceOff && IsSlamming() && !bounceOff.IsSprung()){
			SpringJump(bounceOff);
			bounceOff.SetSprung(true);
			//spring animation message
		}
		//back and forth knock over, to see if bleak should knock this down by jumping on it
		BackAndForthMovement backAndForth = hitInfo.transform.gameObject.GetComponent<BackAndForthMovement>();
		if (backAndForth){
			backAndForth.KnockDown();
		}
	}
	
	void HandleSideCollision(RaycastHit hitInfo, float dt){
	}
	
	void HandleRightCollision(RaycastHit hitInfo, float dt){
		KillZones killZones = hitInfo.transform.gameObject.GetComponent<KillZones>();
		if (killZones){
			if (killZones.HasKillLeft())
				Damage();
		}
		Climbable climbable = hitInfo.transform.gameObject.GetComponent<Climbable>();
		if (climbable){
			SetState (STATE_CLIMBING_RIGHT);
			attachedClimbObject = climbable;
		}
	}
	
	void HandleLeftCollision(RaycastHit hitInfo, float dt){
		KillZones killZones = hitInfo.transform.gameObject.GetComponent<KillZones>();
		if (killZones){
			if (killZones.HasKillRight())
				Damage();
		}
		Climbable climbable = hitInfo.transform.gameObject.GetComponent<Climbable>();
		if (climbable){
			SetState (STATE_CLIMBING_LEFT);
			attachedClimbObject = climbable;
		}
	}
	
	static public int numLives = 3;
	void Damage(){
		Debug.Log ("apply damage!");
		if (numLives > 0){
			//hurt should reset to a checkpoint and graphically change bleak to look more battered
			SetState (STATE_HURT);
			numLives--;
		} else {
			//dead is like a game over and should switch to a menu or move the player back to the beginning of the level
			SetState(STATE_DEAD);
		}
	}
	
	void SpringJump(BounceOffTop bounceOffInfo){
		velocity.y = bounceOffInfo.springPower;
	}
	
	
	//============== Accessor Methods ==================
	public bool IsSlamming(){
		return slamming;
	}
	
	public void SetState(short state){
		this.state = state;
	}
}

