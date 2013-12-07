using UnityEngine;
using System.Collections;

public class BleakController : MonoBehaviour {
	
	//============ standard vars =============
	//public vars
	public Rigidbody2D rigidBody;
	public BoxCollider2D boxCollider;
	public float runSpeed;
	public float joggingMultiplier;
	public float jumpSpeed;
	public float maxFallSpeed;
	public float gravityAcceleration;
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
	private Vector2 velocity = new Vector2 (0,0);
	private Vector2 moveDelta = new Vector2 (0,0);
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
	private Ray2D leftRayBottom;
	private Ray2D centerRayBottom;
	private Ray2D rightRayBottom;
	private float rayLengthBottom;
	private RaycastHit2D rayHitInfoBottom;
	private RaycastHit2D rayHitInfoLBottom;
	private RaycastHit2D rayHitInfoCBottom;
	private RaycastHit2D rayHitInfoRBottom;
	private Vector2 bottomOffset;
	//sides
	private Ray2D topRayRight;
	private Ray2D centerRayRight;
	private Ray2D bottomRayRight;
	private RaycastHit2D rayHitInfoRight;
	private Ray2D topRayLeft;
	private Ray2D centerRayLeft;
	private Ray2D bottomRayLeft;
	private RaycastHit2D rayHitInfoLeft;
	private float rayLengthSide;
	private Vector2 sideOffset;
	private Vector2 position2d;

	private float deltaY;
	private float deltaX;

	private float dt;
	private float fdt;
	

	// Use this for initialization
	void Start () {
		transform.position = startPoint.position;
		position2d = new Vector2(transform.position[0],transform.position[1]);
		boxCollider = transform.gameObject.GetComponent<BoxCollider2D>();
		rigidBody = transform.gameObject.GetComponent<Rigidbody2D>();

		rayLengthBottom = (boxCollider.size.y / 2f)+.05f;
		rayLengthSide = (boxCollider.size.x / 2f)+.05f;
		
		bottomOffset = new Vector2(boxCollider.size.x/2-bottomOffsetPushIn,0);
		sideOffset = new Vector2(0,boxCollider.size.y/2-sideOffsetPushIn);
		
		leftRayBottom = new Ray2D(position2d - bottomOffset + boxCollider.center,-Vector2.up);
		centerRayBottom = new Ray2D(position2d + boxCollider.center - new Vector2(0.0f,boxCollider.size.y/2f),-Vector2.up);
		rightRayBottom = new Ray2D(position2d + bottomOffset + boxCollider.center, -Vector2.up);
		topRayRight = new Ray2D(position2d + sideOffset + boxCollider.center,Vector2.right);
		centerRayRight = new Ray2D(position2d + boxCollider.center,Vector2.right);
		bottomRayRight = new Ray2D(position2d - sideOffset + boxCollider.center,Vector2.right);
		topRayLeft = new Ray2D(position2d + sideOffset + boxCollider.center,-Vector2.right);
		centerRayLeft = new Ray2D(position2d + boxCollider.center,-Vector2.right);
		bottomRayLeft = new Ray2D(position2d - sideOffset + boxCollider.center,-Vector2.right);
	}
	
	// Update is called once per frame
	float tempVelVert;
	void Update () {
		position2d.x = transform.position[0]; position2d.y = transform.position[1];
		switch (state){
		case STATE_NORMAL:
			velocity.x = 0;
			dt = Time.deltaTime;
			UpdateRays(dt);
			if (canControl){
				//enable gravity if no raycasts return true from the three downward rays
				//RaycastHit2D rayHitInfoLBottom; RaycastHit2D rayHitInfoCBottom; RaycastHit2D rayHitInfoRBottom;
				rayHitInfoLBottom = Physics2D.Raycast(leftRayBottom.origin, leftRayBottom.direction,rayLengthBottom);
				rayHitInfoCBottom = Physics2D.Raycast(centerRayBottom.origin,centerRayBottom.direction,rayLengthBottom);
				rayHitInfoRBottom = Physics2D.Raycast(rightRayBottom.origin,rightRayBottom.direction,rayLengthBottom);
				if (!rayHitInfoLBottom && !rayHitInfoCBottom && !rayHitInfoRBottom ){
				    tempVelVert = velocity.y - gravityAcceleration * dt;
					velocity.y = Mathf.Max(tempVelVert, -maxFallSpeed);
					isGrounded = false;
				//otherwise deal with collisions from the top if the object is not ignoring them
				} else if (rayHitInfoLBottom || rayHitInfoCBottom|| rayHitInfoRBottom){
					bool collidableDown = true;
					bool collidableUp = true;
					bool aboveCollider = true;
					if (rayHitInfoLBottom){ rayHitInfoBottom = rayHitInfoLBottom;}
					else if (rayHitInfoCBottom){ rayHitInfoBottom = rayHitInfoCBottom;}
					else if (rayHitInfoRBottom){ rayHitInfoBottom = rayHitInfoRBottom;}
					/*if (rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>()!=null){ 
						collidableDown = !rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreUp();
						collidableUp = !rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreDown();
						//have to check that the object is below bleak, or it will catch the raycasts which start from the center y line of bleak's collider
						aboveCollider = (rayHitInfoBottom.transform.position.y+rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2 + rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().center.y) <= (transform.position.y - boxCollider.size.y/2 + boxCollider.center.y);
						Debug.Log ("above collider: "+aboveCollider+" object: "+(rayHitInfoBottom.transform.position.y+rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2-5)+" player: "+(transform.position.y - boxCollider.size.y/2));
						//Debug.Log("object collider y/2: "+(rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2)+" player collider y/2: "+(boxCollider.size.y/2));
					}*/
					Debug.DrawRay(leftRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
					Debug.DrawRay(centerRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
					Debug.DrawRay(rightRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
					//falling
					if (/*collidableDown && aboveCollider && */velocity.y <= 0){
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
			UpdatePositionChangeNormal(fdt);
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
			rigidBody.velocity = velocity;
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
		RaycastHit2D rayHitInfoTRight = Physics2D.Raycast(topRayRight.origin,topRayRight.direction,rayLengthSide);
		RaycastHit2D rayHitInfoCRight = Physics2D.Raycast(centerRayRight.origin,centerRayRight.direction,rayLengthSide);
		RaycastHit2D rayHitInfoBRight = Physics2D.Raycast(bottomRayRight.origin,bottomRayRight.direction,rayLengthSide);
		if (rayHitInfoTRight || rayHitInfoCRight || rayHitInfoBRight){
			bool collidableRight = true;
			if (rayHitInfoTRight){ rayHitInfoRight = rayHitInfoTRight; }
			else if (rayHitInfoCRight){ rayHitInfoRight = rayHitInfoCRight; }
			else if (rayHitInfoBRight) { rayHitInfoRight = rayHitInfoBRight; }
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
		RaycastHit2D rayHitInfoTLeft = Physics2D.Raycast(topRayLeft.origin,topRayLeft.direction,rayLengthSide);
		RaycastHit2D rayHitInfoCLeft = Physics2D.Raycast(centerRayLeft.origin,centerRayLeft.direction,rayLengthSide);
		RaycastHit2D rayHitInfoBLeft = Physics2D.Raycast(bottomRayLeft.origin,bottomRayLeft.direction,rayLengthSide);
		if (rayHitInfoTLeft || rayHitInfoCLeft || rayHitInfoBLeft){
			bool collidableLeft = true;
			if (rayHitInfoTLeft){ rayHitInfoLeft = rayHitInfoTLeft; }
			else if (rayHitInfoCLeft){ rayHitInfoLeft = rayHitInfoCLeft; }
			else if (rayHitInfoBLeft) { rayHitInfoLeft = rayHitInfoBLeft; }
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
		moveDelta = velocity * dt;
		RaycastHit2D interpolateInfo;
		
		//== deal with interpolate collisions down ==
		deltaY = moveDelta.y;
		if (deltaY < 0){
			Vector2 deltaYRayOriginLeft = new Vector2(transform.position.x-boxCollider.size.x/2+bottomOffsetPushIn,transform.position.y - boxCollider.size.y/2);
			Vector2 deltaYRayOriginCenter = new Vector2(transform.position.x,transform.position.y - boxCollider.size.y/2);
			Vector2 deltaYRayOriginRight = new Vector2(transform.position.x+boxCollider.size.x/2-bottomOffsetPushIn,transform.position.y - boxCollider.size.y/2);

			RaycastHit2D deltaYRayHitL = Physics2D.Raycast(deltaYRayOriginLeft,-Vector2.up, -(deltaY));
			RaycastHit2D deltaYRayHitC = Physics2D.Raycast(deltaYRayOriginCenter,-Vector2.up, -(deltaY));
			RaycastHit2D deltaYRayHitR = Physics2D.Raycast(deltaYRayOriginRight,-Vector2.up, -(deltaY));

			if (deltaYRayHitL || deltaYRayHitC || deltaYRayHitR){
				if (deltaYRayHitL) {interpolateInfo = deltaYRayHitL;moveDelta.y = -interpolateInfo.fraction*(-deltaY);}
				else if (deltaYRayHitC) {interpolateInfo = deltaYRayHitC;moveDelta.y = -interpolateInfo.fraction*(-deltaY);}
				else if (deltaYRayHitR) {interpolateInfo = deltaYRayHitR;moveDelta.y = -interpolateInfo.fraction*(-deltaY);}
				//Debug.Log ("overmovement detected! setting new movement distance from: "+deltaY+" to: "+interpolateInfo.distance+" movedelta.y becoming: "+moveDelta.y);
			}
		}
		//== deal with interpolate collisions sideways ==
		deltaX = moveDelta.x;
		if(deltaX != 0){
			Vector2 deltaXRayOriginRightTop = new Vector2(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn);
			Vector2 deltaXRayOriginRightCenter = new Vector2(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y);
			Vector2 deltaXRayOriginRightBottom = new Vector2(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn);
			Vector2 deltaXRayOriginLeftTop = new Vector2(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn);
			Vector2 deltaXRayOriginLeftCenter = new Vector2(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y);
			Vector2 deltaXRayOriginLeftBottom = new Vector2(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn);

			RaycastHit2D deltaXRightT = Physics2D.Raycast(deltaXRayOriginRightTop,Vector2.right,Mathf.Abs(deltaX));
			RaycastHit2D deltaXRightC = Physics2D.Raycast(deltaXRayOriginRightCenter,Vector2.right,Mathf.Abs(deltaX));
			RaycastHit2D deltaXRightB = Physics2D.Raycast(deltaXRayOriginRightBottom,Vector2.right,Mathf.Abs(deltaX));

			RaycastHit2D deltaXLeftT = Physics2D.Raycast(deltaXRayOriginLeftTop,-Vector2.right,Mathf.Abs(deltaX));
			RaycastHit2D deltaXLeftC = Physics2D.Raycast(deltaXRayOriginLeftCenter,-Vector2.right,Mathf.Abs(deltaX));
			RaycastHit2D deltaXLeftB = Physics2D.Raycast(deltaXRayOriginLeftBottom,-Vector2.right,Mathf.Abs(deltaX));

			if ((deltaXRightB || deltaXRightC || deltaXRightT) && deltaX > .1){
				if (deltaXRightB){interpolateInfo = deltaXRightB;moveDelta.x = interpolateInfo.fraction * Mathf.Abs(deltaX);}
				else if (deltaXRightC){interpolateInfo = deltaXRightC;moveDelta.x = interpolateInfo.fraction * Mathf.Abs(deltaX);}
				else if (deltaXRightT){interpolateInfo = deltaXRightT;moveDelta.x = interpolateInfo.fraction * Mathf.Abs(deltaX);}
			}
			else if ((deltaXLeftT || deltaXLeftC || deltaXLeftB) && deltaX < -.1){
				if (deltaXLeftB){interpolateInfo = deltaXLeftB;moveDelta.x = -interpolateInfo.fraction * Mathf.Abs(deltaX);}
				else if (deltaXLeftC){interpolateInfo = deltaXLeftC;moveDelta.x = -interpolateInfo.fraction * Mathf.Abs(deltaX);}
				else if (deltaXLeftT){interpolateInfo = deltaXLeftT;moveDelta.x = -interpolateInfo.fraction * Mathf.Abs(deltaX);}
			}
		} //all of this may be unnecessary if the rigid body velocity works ok, but it seems to be too choppy

		if (moveDelta.x > 0)
			transform.Translate(new Vector3(moveDelta.x, moveDelta.y, 0f));
		else
			transform.Translate(new Vector3(-moveDelta.x, moveDelta.y, 0f));
	}
	
	/// <summary>
	/// Updates the position change climbing.
	/// </summary>
	/// <param name='dt'>
	/// time since last frame (in seconds).
	/// </param>
	void UpdatePositionChangeClimbing(float dt){
		Vector2 deltaXRayOriginRightTop = new Vector2(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn);
		Vector2 deltaXRayOriginRightCenter = new Vector2(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y);
		Vector2 deltaXRayOriginRightBottom = new Vector2(transform.position.x+boxCollider.size.x/2-.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn);
		Vector2 deltaXRayOriginLeftTop = new Vector2(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y+boxCollider.size.y/2-sideOffsetPushIn);
		Vector2 deltaXRayOriginLeftCenter = new Vector2(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y);
		Vector2 deltaXRayOriginLeftBottom = new Vector2(transform.position.x-boxCollider.size.x/2+.1f,transform.position.y-boxCollider.size.y/2+sideOffsetPushIn);


		//check to the right
		RaycastHit2D interpolateInfoR;
		RaycastHit2D hitTRight = Physics2D.Raycast(deltaXRayOriginRightTop,Vector2.right,1f);
		RaycastHit2D hitCRight = Physics2D.Raycast(deltaXRayOriginRightCenter,Vector2.right,1f);
		RaycastHit2D hitBRight = Physics2D.Raycast(deltaXRayOriginRightBottom,Vector2.right,1f);

		if (hitTRight){interpolateInfoR = hitTRight; }
		else if (hitCRight){interpolateInfoR = hitCRight;}
		else if (hitBRight){interpolateInfoR = hitBRight;}

		if (hitTRight || hitCRight || hitBRight){
			if (interpolateInfoR.transform.gameObject.GetComponent<KillZones>().HasKillLeft ())
				Damage();
			else if (interpolateInfoR.transform.gameObject.GetComponent<Climbable>() == null)
				SetState(STATE_NORMAL);
		}


		//check to the left
		RaycastHit2D interpolateInfoL;
		RaycastHit2D hitTLeft = Physics2D.Raycast(deltaXRayOriginLeftTop,-Vector2.right,1f);
		RaycastHit2D hitCLeft = Physics2D.Raycast(deltaXRayOriginLeftCenter,-Vector2.right,1f);
		RaycastHit2D hitBLeft = Physics2D.Raycast(deltaXRayOriginLeftBottom,-Vector2.right,1f);

		if (hitTLeft){interpolateInfoL = hitTLeft; }
		else if (hitCLeft){interpolateInfoL = hitCLeft;}
		else if (hitBLeft){interpolateInfoL = hitBLeft;}

		if (hitTLeft || hitCLeft || hitBLeft){
			if (interpolateInfoL.transform.gameObject.GetComponent<KillZones>().HasKillRight ())
				Damage();
			else if (interpolateInfoL.transform.gameObject.GetComponent<Climbable>() == null)
				SetState(STATE_NORMAL);
		}

		if (!hitTLeft && !hitCLeft && !hitBLeft && !hitTRight && !hitCRight && !hitBRight) {
			velocity.x *= 2;
			velocity.y += 5f;
			SetState(STATE_NORMAL);
		}
		
		rigidBody.velocity = velocity;
	}
	
	/// <summary>
	/// Updates the rays used for raycasting to check collisions
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateRays(float dt){
		leftRayBottom.origin=position2d - bottomOffset + boxCollider.center;
		centerRayBottom.origin=position2d + boxCollider.center;
		rightRayBottom.origin=position2d + bottomOffset + boxCollider.center;
		
		topRayRight.origin=position2d+sideOffset+boxCollider.center;
		centerRayRight.origin=position2d+boxCollider.center;
		bottomRayRight.origin=position2d-sideOffset+boxCollider.center;
		
		topRayLeft.origin=position2d+sideOffset+boxCollider.center;
		centerRayLeft.origin=position2d+boxCollider.center;
		bottomRayLeft.origin=position2d-sideOffset+boxCollider.center;
	}
	
	
	/// <summary>
	/// Update step for any debugging happening
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateDebug(float dt){
		Debug.DrawRay(position2d - bottomOffset + boxCollider.center,Vector3.down*rayLengthBottom);
		Debug.DrawRay(position2d + boxCollider.center,Vector3.down*rayLengthBottom);
		Debug.DrawRay(position2d + bottomOffset + boxCollider.center, Vector3.down*rayLengthBottom);
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
	void HandleBottomCollision(RaycastHit2D hitInfo, float dt){
		if (-velocity.y > Mathf.Abs(fallKillSpeed) && !IsSlamming()) Damage (); //for right now, no fall damage when slamming. We may need to change this though
		else if (velocity.y < 0) velocity.y = 0;
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
	
	void HandleSideCollision(RaycastHit2D hitInfo, float dt){
	}
	
	void HandleRightCollision(RaycastHit2D hitInfo, float dt){
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
	
	void HandleLeftCollision(RaycastHit2D hitInfo, float dt){
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

