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
	public float pushSpeed;
	public float gravityAcceleration;
	public float slamMultiplayer;
	public bool canControl = true;
	public Transform startPoint;
	public float fallKillSpeed;
	public float timeUntilRun = 1.75f;
	public float bottomOffsetPushIn;
	public float sideOffsetPushIn;
	public GameObject skelAnimObj;
	public AudioClip noooo;
	public AudioSource cammie;
	public Aperture aperture;
	public float maximumSlopeAngle;

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
	private bool isPushing = false;
	private Climbable attachedClimbObject;
	private KeyCode actionButton = KeyCode.E;
	private SkeletonAnimation skelAnim;
	private GameObject attachedRideObject;
	private bool runningOnSlope = false;
	
	private const bool LEFT = false;
	private const bool RIGHT = true;

	private BasicEntityCollision collisionHandler;
	
	

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
	public const short STATE_PUSH_LEFT = 8;
	public const short STATE_PUSH_RIGHT = 9;
	
	
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
		//load info from other level
		//numLives = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().numLivesPlayer;

		//instantiate
		transform.position = startPoint.position;
		position2d = new Vector2(transform.position[0],transform.position[1]);
		boxCollider = transform.gameObject.GetComponent<BoxCollider2D>();
		rigidBody = transform.gameObject.GetComponent<Rigidbody2D>();
		skelAnim = skelAnimObj.GetComponent<SkeletonAnimation>();

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

		collisionHandler = new BasicEntityCollision();
		collisionHandler.Init(boxCollider,rigidBody);

		StartCoroutine("RemoveControlForTime",.75f);
	}

	void OnLevelWasLoaded(int levelIndex){
		//deal with loading in start point changes and such here
	}
	
	// Update is called once per frame
	float tempVelVert;
	float timeSinceEnteredClimbing = 0f;
	void Update () {
		position2d.x = transform.position[0]; position2d.y = transform.position[1];
		if (state != STATE_NORMAL){
			attachedRideObject = null;
		}
		switch (state){
		case STATE_NORMAL:
			velocity.x = 0;
			dt = Time.smoothDeltaTime;
			UpdateRays(dt);
			UpdateGravity(dt);
			if (canControl){
				UpdateInputNormal(dt);
				UpdateIdle(dt);
				UpdateLeftRight(dt);
				UpdateUp(dt);
			}
			UpdateRotationNormal(dt);
			break;
		case STATE_CLIMBING_LEFT:
			Debug.Log("left");
			UpdateRays (dt);
			if (canControl){
				if (timeSinceEnteredClimbing < 1f){
					timeSinceEnteredClimbing += Time.smoothDeltaTime;
				}
				if (Input.GetKey (KeyCode.W)){
					if (skelAnim.state.ToString() != "climb up") skelAnim.state.SetAnimation(0,"climb up",true);
					velocity.y = attachedClimbObject.climbSpeed;
				} else if (Input.GetKey(KeyCode.S)){
					if (skelAnim.state.ToString() != "climb down") skelAnim.state.SetAnimation(0,"climb down",true);
					velocity.y = -attachedClimbObject.climbSpeed * 2/3;
				} /*else if (Input.GetKey(KeyCode.D)){
					SetState(STATE_NORMAL);
					velocity.x = 0;
				}*/ else if (Input.GetKey (KeyCode.Space)){
					SetState(STATE_NORMAL);
					jumping = -1;
					velocity.x = runSpeed*joggingMultiplier*.5f;
					velocity.y = jumpSpeed * .35f;
					skelAnim.state.SetAnimation(0,"jump",false);
				} else if (Input.GetKeyDown (KeyCode.A) && timeSinceEnteredClimbing >= 1f){ 
					//switch sides
					StartCoroutine("cSwitchClimbingSide","right");
				} else {
					if (skelAnim.state.ToString() != "climb idle" /*and check to be sure it isn't in the rope side transition animation*/) skelAnim.state.SetAnimation(0,"climb idle",true);
					velocity.y = 0;
				}
			}
			break;
		case STATE_CLIMBING_RIGHT:
			Debug.Log("right");
			UpdateRays (dt);
			if (canControl){
				if (timeSinceEnteredClimbing < 1f){
					timeSinceEnteredClimbing += Time.smoothDeltaTime;
				}
				if (Input.GetKey (KeyCode.W)){
					if (skelAnim.state.ToString() != "climb up") skelAnim.state.SetAnimation(0,"climb up",true);
					velocity.y = attachedClimbObject.climbSpeed;
				} else if (Input.GetKey(KeyCode.S)){
					if (skelAnim.state.ToString() != "climb down") skelAnim.state.SetAnimation(0,"climb down",true);
					velocity.y = -attachedClimbObject.climbSpeed * 2/3;
				} /*else if (Input.GetKey(KeyCode.A)){
					SetState(STATE_NORMAL);
					velocity.x = 0;
				}*/ else if (Input.GetKey (KeyCode.Space)){
					SetState(STATE_NORMAL);
					jumping = -1;
					velocity.x = -runSpeed*joggingMultiplier*.5f;
					velocity.y = jumpSpeed * .35f;
					skelAnim.state.SetAnimation(0,"jump",false);
				} else if (Input.GetKeyDown (KeyCode.D) && timeSinceEnteredClimbing >= 1f){ 
					//switch sides
					StartCoroutine("cSwitchClimbingSide","left");
				} else {
					if (skelAnim.state.ToString() != "climb idle" /*and check to be sure it isn't in the rope side transition animation*/) skelAnim.state.SetAnimation(0,"climb idle",true);
					velocity.y = 0;
				}
			}
			break;
		case STATE_FORCE_MOVE:
			UpdateForceMove(dt);
			break;
		case STATE_HURT:
			switch (numLives){
			case 1:
				skelAnim.skeleton.SetSkin("damaged01");
				break;
			case 0:
				skelAnim.skeleton.SetSkin("damaged02");
				break;
			default:
				throw new UnityException("bleak was hurt, and his number of lives was not 0 or 1");
			}
			if (canControl){
				StartCoroutine("CloseAndOpenApertureRespawn",startPoint);
			}
			break;
		case STATE_DEAD:
			//skelAnim.state.SetAnimation(0,"death",false);
			if (Input.GetKeyDown(KeyCode.R)){
				transform.position = startPoint.position;
				numLives = 2;
				skelAnim.skeleton.SetSkin("damaged00");
				SetState(STATE_NORMAL);
			}
			break;
		case STATE_PUSH_LEFT:
			if (skelAnim.state.ToString() != "push")
				skelAnim.state.SetAnimation(0,"push",true);
			UpdateRays(dt);
			if (canControl) UpdatePushing(dt,LEFT);
			UpdateGravity(dt);
			UpdateUp(dt);
			UpdateRotationNormal(dt);
			break;
		case STATE_PUSH_RIGHT:
			if (skelAnim.state.ToString() != "push")
				skelAnim.state.SetAnimation(0,"push",true);
			UpdateRays(dt);
			if (canControl) UpdatePushing(dt,RIGHT);
			UpdateGravity(dt);
			UpdateUp(dt);
			UpdateRotationNormal(dt);
			break;
		}
		if (Debug.isDebugBuild)
			UpdateDebug(dt);
	}
	
	void FixedUpdate(){
		fdt = Time.fixedDeltaTime;
		//Debug.Log(jumping);
		switch (state){
		case STATE_NORMAL:
			UpdateJump(fdt);
			break;
		}
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
		KillZones killOnTouch = collision.gameObject.GetComponent<KillZones>();
		if (killOnTouch){
			if (killOnTouch.HasKillAny()){
				Damage();
			}
		}
	}
	void LateUpdate(){
		switch (state){
		case STATE_NORMAL:
			//transform.Translate(collisionHandler.Move(velocity*dt,transform.position,(facing ? 1f : -1f)));
			UpdatePositionChangeNormal(dt);
			break;
		case STATE_PUSH_RIGHT:
			transform.Translate(collisionHandler.Move(velocity*dt,transform.position,(facing ? 1f : -1f)));
			break;
		case STATE_PUSH_LEFT:
			transform.Translate(collisionHandler.Move(velocity*dt,transform.position,(facing ? 1f : -1f)));
			break;
		case STATE_CLIMBING_LEFT:
			UpdatePositionChangeClimbing(dt);
			break;
		case STATE_CLIMBING_RIGHT:
			UpdatePositionChangeClimbing(dt);
			break;
		}
	}

	/// <summary>
	/// Updates the gravity.
	/// </summary>
	/// <param name="dt">Dt.</param>
	void UpdateGravity(float dt){
		if (canControl){
			//enable gravity if no raycasts return true from the three downward rays
			//RaycastHit2D rayHitInfoLBottom; RaycastHit2D rayHitInfoCBottom; RaycastHit2D rayHitInfoRBottom;
			rayHitInfoLBottom = Physics2D.Raycast(leftRayBottom.origin, leftRayBottom.direction,rayLengthBottom);
			rayHitInfoCBottom = Physics2D.Raycast(centerRayBottom.origin,centerRayBottom.direction,rayLengthBottom);
			rayHitInfoRBottom = Physics2D.Raycast(rightRayBottom.origin,rightRayBottom.direction,rayLengthBottom);

			/*Debug.DrawRay(leftRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
			Debug.DrawRay(centerRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);
			Debug.DrawRay(rightRayBottom.origin,Vector3.down*rayLengthBottom,Color.green);*/

			if (!rayHitInfoLBottom && !rayHitInfoCBottom && !rayHitInfoRBottom ){
				tempVelVert = velocity.y - gravityAcceleration * dt;
				if (!runningOnSlope) velocity.y = Mathf.Max(tempVelVert, -maxFallSpeed);
				isGrounded = false;
				//otherwise deal with collisions from the top if the object is not ignoring them
			} else if (rayHitInfoLBottom || rayHitInfoCBottom || rayHitInfoRBottom){
				//Debug.Break();
				bool collidableDown = true;
				bool collidableUp = true;
				bool aboveCollider = true;
				if (rayHitInfoLBottom){ rayHitInfoBottom = rayHitInfoLBottom;}
				else if (rayHitInfoCBottom){ rayHitInfoBottom = rayHitInfoCBottom;}
				else if (rayHitInfoRBottom){ rayHitInfoBottom = rayHitInfoRBottom;}
				RaycastHit2D highestHitBottom;
				highestHitBottom = rayHitInfoLBottom.fraction < rayHitInfoRBottom.fraction ? rayHitInfoLBottom : rayHitInfoRBottom;
				if (rayHitInfoCBottom.fraction < highestHitBottom.fraction) highestHitBottom = rayHitInfoCBottom;
				/*if (rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>()!=null){ 
						collidableDown = !rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreUp();
						collidableUp = !rayHitInfoBottom.transform.gameObject.GetComponent<IgnoreCollisions>().HasIgnoreDown();
						//have to check that the object is below bleak, or it will catch the raycasts which start from the center y line of bleak's collider
						aboveCollider = (rayHitInfoBottom.transform.position.y+rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2 + rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().center.y) <= (transform.position.y - boxCollider.size.y/2 + boxCollider.center.y);
						Debug.Log ("above collider: "+aboveCollider+" object: "+(rayHitInfoBottom.transform.position.y+rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2-5)+" player: "+(transform.position.y - boxCollider.size.y/2));
						//Debug.Log("object collider y/2: "+(rayHitInfoBottom.transform.gameObject.GetComponent<BoxCollider>().size.y/2)+" player collider y/2: "+(boxCollider.size.y/2));
					}*/

				//falling
				if (/*collidableDown && aboveCollider && */velocity.y <= 0){
					if (-velocity.y > Mathf.Abs(fallKillSpeed) /*&& !IsSlamming()*/) 
						Damage (); //for right now, no fall damage when slamming. We may need to change this though
					else if (velocity.y < 0) velocity.y = 0;
					HandleBottomCollision(rayHitInfoBottom,dt);	
				} else {
					tempVelVert = velocity.y - gravityAcceleration * dt;
					if (!runningOnSlope) velocity.y = Mathf.Max(tempVelVert, -maxFallSpeed);
				}
			}
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
					/*if (skelAnim.state.ToString()=="sprint"){
						skelAnim.state.ClearTrack(0);
						skelAnim.state.AddAnimation(0,"skid",false,0.0f);
						//skelAnim.state.AddAnimation(0,"idle",true,0.0f);
					} else */if (skelAnim.state.ToString()=="run"){
						skelAnim.state.ClearTrack(0);
						if (numLives > 0) skelAnim.state.AddAnimation(0,"idle",true,0.0f);
						else skelAnim.state.AddAnimation(0,"idle-injured",true,0.0f);
					} else {
						if (numLives > 0) skelAnim.state.AddAnimation(0,"idle",true,0.0f);
						else skelAnim.state.AddAnimation(0,"idle-injured",true,0.0f);
					}
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
			//rigidBody.velocity = velocity;
			transform.Translate(new Vector3(velocity.x*dt,velocity.y*dt));
		} else {
			Messenger.Broadcast<Transform,BleakController>("CompletedForceMove",forceMoveDestination,this);
		}
	}

	private Pushable attachedPushObj;
	private float pushLag = .4f;
	void UpdatePushing(float dt, bool direction){
		bool stillTouching = false;

		if (direction == LEFT){
			RaycastHit2D rayHitInfoTLeft = Physics2D.Raycast(topRayLeft.origin,topRayLeft.direction,rayLengthSide+.5f);
			RaycastHit2D rayHitInfoCLeft = Physics2D.Raycast(centerRayLeft.origin,centerRayLeft.direction,rayLengthSide+.5f);
			RaycastHit2D rayHitInfoBLeft = Physics2D.Raycast(bottomRayLeft.origin,bottomRayLeft.direction,rayLengthSide+.5f);
			if (rayHitInfoTLeft || rayHitInfoCLeft || rayHitInfoBLeft) stillTouching = true;
			if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && stillTouching){
				if (pushLag <= 0f){
					if (!attachedPushObj.isLocked){
						Vector2 newVel = new Vector2(-pushSpeed-1f,attachedPushObj.gameObject.GetComponent<Rigidbody2D>().velocity.y);
						attachedPushObj.gameObject.GetComponent<Rigidbody2D>().velocity = newVel;
						velocity.x = -pushSpeed;
					}
				} else {
					pushLag -= dt;
				}
			} else {
				attachedPushObj = null;
				pushLag = .4f;
				SetState(STATE_NORMAL);
				skelAnim.state.SetAnimation(0,"idle",true);
			}
		} else if (direction == RIGHT){
			RaycastHit2D rayHitInfoTRight = Physics2D.Raycast(topRayRight.origin,topRayRight.direction,rayLengthSide+.5f);
			RaycastHit2D rayHitInfoCRight = Physics2D.Raycast(centerRayRight.origin,centerRayRight.direction,rayLengthSide+.5f);
			RaycastHit2D rayHitInfoBRight = Physics2D.Raycast(bottomRayRight.origin,bottomRayRight.direction,rayLengthSide+.5f);
			if (rayHitInfoTRight || rayHitInfoCRight || rayHitInfoBRight) stillTouching = true;
			if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && stillTouching){
				if (pushLag <= 0f){
					if (!attachedPushObj.isLocked){
						Vector2 newVel = new Vector2(pushSpeed+1f,attachedPushObj.gameObject.GetComponent<Rigidbody2D>().velocity.y);
						attachedPushObj.gameObject.GetComponent<Rigidbody2D>().velocity = newVel;
						velocity.x = pushSpeed;
					}
				} else {
					pushLag -= dt;
				}
			} else {
				attachedPushObj = null;
				pushLag = .4f;
				SetState(STATE_NORMAL);
				skelAnim.state.SetAnimation(0,"idle",true);
			}
		}
	}
	
	/// <summary>
	/// Updates x movement based on controls.
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateLeftRight(float dt){
		runningOnSlope = false;
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
		//Debug.DrawRay(new Vector3(topRayRight.origin.x,topRayRight.origin.y-sideOffset.y-boxCollider.size.y/2f,0f),Vector3.right,Color.green);
		//Debug.DrawRay(new Vector3(topRayRight.origin.x,topRayRight.origin.y-sideOffset.y-boxCollider.size.y/2f+.1f,0f),Vector3.right,Color.blue);
		//player is hitting direction
		if (directionInt != 0){


			RaycastHit2D rayHitInfoSlopeCheckRight = Physics2D.Raycast(new Vector2(topRayRight.origin.x,topRayRight.origin.y-sideOffset.y-boxCollider.size.y/2f+.05f),
			                                                           topRayRight.direction,1f);
			RaycastHit2D rayHitInfoSlopeCheckRight2 = Physics2D.Raycast(new Vector2(topRayRight.origin.x,topRayRight.origin.y-sideOffset.y-boxCollider.size.y/2f),
			                                                            topRayRight.direction,1f);
			RaycastHit2D rayHitInfoSlopeCheckLeft = Physics2D.Raycast(new Vector2(topRayLeft.origin.x,topRayLeft.origin.y-sideOffset.y-boxCollider.size.y/2f+.05f),
			                                                          topRayLeft.direction,1f);
			RaycastHit2D rayHitInfoSlopeCheckLeft2 = Physics2D.Raycast(new Vector2(topRayLeft.origin.x,topRayLeft.origin.y-sideOffset.y-boxCollider.size.y/2f),
			                                                           topRayLeft.direction,1f);
			bool lowObstructedRight = false; bool lowObstructedLeft = false;
			if (rayHitInfoSlopeCheckRight2){
				if (rayHitInfoSlopeCheckRight2.fraction-boxCollider.size.x/2f<=.05f){
					lowObstructedRight = true;
				}
			}
			if (rayHitInfoSlopeCheckLeft2){
				if (rayHitInfoSlopeCheckLeft2.fraction-boxCollider.size.x/2f<=.05f){
					lowObstructedLeft = true;
				}
			}
			idleDelay = 0;
			if (Input.GetButton("Walk")){
				//check for clear space
				if ((facing && !obstructedRight && !lowObstructedRight) || (!facing && !obstructedLeft && !lowObstructedLeft)){
					velocity.x = runSpeed * joggingMultiplier * directionInt;
				}


				//check for slopes
				else if ((facing && lowObstructedRight) || (!facing && lowObstructedLeft)){
					if (directionInt > 0){
						if (rayHitInfoSlopeCheckRight){
							float yDiff = .05f;
							float xDiff = rayHitInfoSlopeCheckRight.fraction - rayHitInfoSlopeCheckRight2.fraction;
							float yOverx = yDiff/xDiff;
							float angle = Mathf.Atan2(yDiff,xDiff)*Mathf.Rad2Deg;
							//Debug.Log(angle);
							if (angle <= maximumSlopeAngle && angle > 0){
								velocity.x = runSpeed * joggingMultiplier;
								velocity.y = yOverx * (runSpeed*joggingMultiplier-rayHitInfoSlopeCheckRight2.fraction);
								velocity.Normalize();
								velocity *= runSpeed;
								velocity *= joggingMultiplier;
								runningOnSlope = true;
							}
						}
					} else if (directionInt < 0){
						if (rayHitInfoSlopeCheckLeft){
							float yDiff = .05f;
							float xDiff = rayHitInfoSlopeCheckLeft.fraction - rayHitInfoSlopeCheckLeft2.fraction;
							float yOverx = yDiff/xDiff;
							float angle = Mathf.Atan2(yDiff,xDiff)*Mathf.Rad2Deg;
							if (angle <= maximumSlopeAngle && angle > 0){
								velocity.x = runSpeed * joggingMultiplier;
								velocity.y = yOverx * (runSpeed*joggingMultiplier-rayHitInfoSlopeCheckLeft2.fraction);
								velocity.Normalize();
								velocity *= runSpeed;
								velocity *= joggingMultiplier;
								runningOnSlope = true;
							}
						}
					}
				}
				if (jumping != -1){
					//play jogging anim
					if (skelAnim.state.ToString()!="run" && skelAnim.state.ToString()!="run-injured" && skelAnim.state.ToString()!="pick up"){
						if (numLives > 0) skelAnim.state.SetAnimation(0,"run",true);
						else skelAnim.state.SetAnimation(0,"run-injured",true);
					}
				}
			} else {
				if ((facing && !obstructedRight && !lowObstructedRight) || (!facing && !obstructedLeft && !lowObstructedLeft)){
					velocity.x = runSpeed * directionInt;
					runningOnSlope = false;
				}

				//check for slopes
				else if ((facing && lowObstructedRight) || (!facing && lowObstructedLeft)){
					if (directionInt > 0){
						if (rayHitInfoSlopeCheckRight){
							float yDiff = .05f;
							float xDiff = rayHitInfoSlopeCheckRight.fraction - rayHitInfoSlopeCheckRight2.fraction;
							float yOverx = yDiff/xDiff;
							float angle = Mathf.Atan2(yDiff,xDiff)*Mathf.Rad2Deg;
							if (angle <= maximumSlopeAngle && angle > 0){
								velocity.x = runSpeed;
								velocity.y = yOverx * (runSpeed*joggingMultiplier-rayHitInfoSlopeCheckRight2.fraction);
								velocity.Normalize();
								velocity *= runSpeed;
								runningOnSlope = true;
							}
						}
					} else if (directionInt < 0){
						if (rayHitInfoSlopeCheckLeft){
							float yDiff = .05f;
							float xDiff = rayHitInfoSlopeCheckLeft.fraction - rayHitInfoSlopeCheckLeft2.fraction;
							float yOverx = yDiff/xDiff;
							float angle = Mathf.Atan2(yDiff,xDiff)*Mathf.Rad2Deg;
							if (angle <= maximumSlopeAngle && angle > 0){
								velocity.x = runSpeed;
								velocity.y = yOverx * (runSpeed*joggingMultiplier-rayHitInfoSlopeCheckLeft2.fraction);
								velocity.Normalize();
								velocity *= runSpeed;
								runningOnSlope = true;
							}
						}
					}
				}

				if (jumping != -1){
					//play running anim
					if (skelAnim.state.ToString()!="sprint"){
						skelAnim.state.SetAnimation(0,"sprint",true);
					}
				}
			}
		} else {	//otherwise horizontal direction isn't being applied
			runDelay = 0;
		}
	}


	void UpdateUp(float dt){
		RaycastHit2D rayHitUpC = Physics2D.Raycast(position2d+boxCollider.center,Vector2.up,rayLengthBottom);

		if (rayHitUpC){
			HandleTopCollision(rayHitUpC,dt);
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
			if (attachedRideObject != null) attachedRideObject = null;
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
			skelAnim.state.SetAnimation(0,"jump",false);
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
			skelAnim.state.SetAnimation(0,"jump-slam",false);
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
			Messenger.Broadcast<Transform,BleakController>("BleakInteractionActivate",transform,this);
		}
	}
	
	
	
	/// <summary>
	/// Updates rotation
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateRotationNormal(float dt){
		/*if (!facing && transform.rotation.y <= 0.1f){
			transform.Rotate(0,180,0);
		}
		if (facing && transform.rotation.y >= .9f){
			transform.Rotate (0,180,0);
		}*/

		if (!facing && skelAnimObj.transform.localScale.x!=-1.0f){
			skelAnimObj.transform.localScale = new Vector3(-1.0f,skelAnimObj.transform.localScale.y,skelAnimObj.transform.localScale.z);
		}
		if (facing && skelAnimObj.transform.localScale.x!=1.0f){
			skelAnimObj.transform.localScale = new Vector3(1.0f,skelAnimObj.transform.localScale.y,skelAnimObj.transform.localScale.z);
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
		if (attachedRideObject != null){
			moveDelta += attachedRideObject.GetComponent<MoveWithObject>().GetMoveDeltaLastFrame();
		}
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

			Debug.DrawRay(new Vector3(deltaYRayOriginLeft.x,deltaYRayOriginLeft.y),Vector3.down*deltaY,Color.green);
			Debug.DrawRay(new Vector3(deltaYRayOriginCenter.x,deltaYRayOriginCenter.y),Vector3.down*deltaY,Color.green);
			Debug.DrawRay(new Vector3(deltaYRayOriginRight.x,deltaYRayOriginRight.y),Vector3.down*deltaY,Color.green);

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
		}

		//slopes check
		RaycastHit2D rayHitInfoSlopeRight = Physics2D.Raycast(rightRayBottom.origin-new Vector2(.05f,0f),rightRayBottom.direction,rayLengthBottom+.5f);
		RaycastHit2D rayHitInfoSlopeLeft = Physics2D.Raycast(leftRayBottom.origin+new Vector2(.05f,0f),leftRayBottom.direction,rayLengthBottom+.5f);
		RaycastHit2D rayHitInfoSlope;
		
		if ((rayHitInfoSlopeLeft || rayHitInfoSlopeRight) && moveDelta.y <= 0){
			if (rayHitInfoSlopeLeft.normal.x != 0 && rayHitInfoSlopeRight.normal.x != 0){
				float dist = Mathf.Min (rayHitInfoSlopeLeft.fraction*(rayLengthBottom+.5f),rayHitInfoSlopeRight.fraction*(rayLengthBottom+.5f));
				deltaY = dist;
				RaycastHit2D rayHitInfoSlopeCheckRight = Physics2D.Raycast(new Vector2(topRayRight.origin.x,topRayRight.origin.y-sideOffset.y-boxCollider.size.y/2f+.1f),
				                                                           topRayRight.direction,1f);
				RaycastHit2D rayHitInfoSlopeCheckRight2 = Physics2D.Raycast(new Vector2(topRayRight.origin.x,topRayRight.origin.y-sideOffset.y-boxCollider.size.y/2f),
				                                                            topRayRight.direction,1f);
				RaycastHit2D rayHitInfoSlopeCheckLeft = Physics2D.Raycast(new Vector2(topRayLeft.origin.x,topRayLeft.origin.y-sideOffset.y-boxCollider.size.y/2f+.1f),
				                                                          topRayLeft.direction,1f);
				RaycastHit2D rayHitInfoSlopeCheckLeft2 = Physics2D.Raycast(new Vector2(topRayLeft.origin.x,topRayLeft.origin.y-sideOffset.y-boxCollider.size.y/2f),
				                                                           topRayLeft.direction,1f);
				if (Input.GetAxis("Horizontal")==0 && rayHitInfoSlopeCheckRight){
					deltaX = rayHitInfoSlopeCheckRight.fraction;
				} else if (Input.GetAxis("Horizontal")==0 && rayHitInfoSlopeCheckLeft){
					deltaX = -rayHitInfoSlopeCheckRight.fraction;
				}
			}
		}
		rigidBody.isKinematic = false;
		if (rayHitInfoSlopeLeft && rayHitInfoSlopeRight){
			RaycastHit2D higherSlopeInfo;
			higherSlopeInfo = rayHitInfoSlopeLeft.fraction < rayHitInfoSlopeRight.fraction ? rayHitInfoSlopeLeft : rayHitInfoSlopeRight;
			//Debug.Log (higherSlopeInfo.normal);
			if (higherSlopeInfo.normal != Vector2.up){
				rigidBody.isKinematic = true;
			}
		}

		transform.Translate(new Vector3(moveDelta.x, moveDelta.y, 0.0f));
	}
	
	/// <summary>
	/// Updates the position change climbing.
	/// </summary>
	/// <param name='dt'>
	/// time since last frame (in seconds).
	/// </param>
	void UpdatePositionChangeClimbing(float dt){
		if (!canControl) return;
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
			KillZones killZone = interpolateInfoR.transform.gameObject.GetComponent<KillZones>();
			if (killZone){
				if (killZone.HasKillLeft()){
					Damage();
				}
			}
			else if (!interpolateInfoR.transform.gameObject.GetComponent<Climbable>()){
				velocity.y = 10f;
				SetState(STATE_NORMAL);
			}
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
			KillZones killZone = interpolateInfoL.transform.gameObject.GetComponent<KillZones>();
			if (killZone){
				if (killZone.HasKillLeft()){
					Damage();
				}
			}
			else if (!interpolateInfoL.transform.gameObject.GetComponent<Climbable>()){
				SetState(STATE_NORMAL);
			}
		}

		if (!hitTLeft && !hitCLeft && !hitBLeft && !hitTRight && !hitCRight && !hitBRight) {
			velocity.y += 3f;
			isGrounded = true;
			SetState(STATE_NORMAL);
		}
		
		//rigidBody.velocity = velocity;
		transform.Translate(new Vector3(velocity.x*dt,velocity.y*dt));
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
		/*Debug.DrawRay(position2d - bottomOffset + boxCollider.center,Vector3.down*rayLengthBottom);
		Debug.DrawRay(position2d + boxCollider.center,Vector3.down*rayLengthBottom);
		Debug.DrawRay(position2d + bottomOffset + boxCollider.center, Vector3.down*rayLengthBottom);*/
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
		isGrounded = true;

		//kill zones component check, to see if bleak should die touching this
		KillZones killZones = hitInfo.transform.gameObject.GetComponent<KillZones>();
		if (killZones){
			if (killZones.HasKillUp()){
				Damage();
			}
		}

		//back and forth knock over, to see if bleak should knock this down by jumping on it
		BackAndForthMovement backAndForth = hitInfo.transform.gameObject.GetComponent<BackAndForthMovement>();
		if (backAndForth){
			backAndForth.KnockDown();
		}

		//bounce off component check, to see if bleak should spring off this
		BounceOffTop bounceOff = hitInfo.transform.gameObject.GetComponent<BounceOffTop>();
		if (bounceOff && IsSlamming() && !bounceOff.IsSprung()){
			SpringJump(bounceOff);
			bounceOff.SetSprung(true);
			bounceOff.PlaySpringAnimation();
		}

		Breakable breakable = hitInfo.transform.gameObject.GetComponent<Breakable>();
		if (breakable){
			if (!breakable.isBreaking && breakable.breakableUp){
				if (this.IsSlamming()){
					breakable.Break();
				} else if (!breakable.needsSlam){
					breakable.Break();
				} else if (breakable.needsSlam && jumping != 0){
					breakable.JumpSound();
				} else if (breakable.needsSlam){
					breakable.StandSound();
				}
			}
		}

		MoveWithObject rideable = hitInfo.transform.gameObject.GetComponent<MoveWithObject>();
		if (rideable){
			if (rideable.moveWithTop){
				attachedRideObject = rideable.gameObject;
			}
		}
	}

	void HandleTopCollision(RaycastHit2D hitInfo, float dt){
		Breakable breakable = hitInfo.transform.gameObject.GetComponent<Breakable>();
		if (breakable){
			if (!breakable.isBreaking && breakable.breakableDown)
				breakable.Break();
		}
	}
	
	void HandleSideCollision(RaycastHit2D hitInfo, float dt){
		Pickup pickup = hitInfo.transform.gameObject.GetComponent<Pickup>();
		if (pickup){
			PickUpPickup(pickup);
		}
		Item item = hitInfo.transform.gameObject.GetComponent<Item>();
		if (item){
			PickUpItem(item);
		}
	}
	
	void HandleRightCollision(RaycastHit2D hitInfo, float dt){
		KillZones killZones = hitInfo.transform.gameObject.GetComponent<KillZones>();
		if (killZones){
			if (killZones.HasKillLeft())
				Damage();
		}
		Climbable climbable = hitInfo.transform.gameObject.GetComponent<Climbable>();
		if (climbable){
			timeSinceEnteredClimbing = 0f;
			SetState (STATE_CLIMBING_RIGHT);
			attachedClimbObject = climbable;
		}
		Pushable pushable = hitInfo.transform.gameObject.GetComponent<Pushable>();
		if (pushable){
			SetState(STATE_PUSH_RIGHT);
			attachedPushObj = pushable;
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
			timeSinceEnteredClimbing = 0f;
			SetState (STATE_CLIMBING_LEFT);
			attachedClimbObject = climbable;
		}
		Pushable pushable = hitInfo.transform.gameObject.GetComponent<Pushable>();
		if (pushable){
			SetState(STATE_PUSH_LEFT);
			attachedPushObj = pushable;
		}
	}
	
	public int numLives;
	void Damage(){
		skelAnimObj.transform.localScale = new Vector3(1.0f,skelAnimObj.transform.localScale.y,skelAnimObj.transform.localScale.z);
		velocity = Vector2.zero;
		if (numLives > 0){
			//hurt should reset to a checkpoint and graphically change bleak to look more battered
			cammie.audio.PlayOneShot(noooo);
			SetState (STATE_HURT);
			numLives--;
		} else {
			//dead is like a game over and should switch to a menu or move the player back to the beginning of the level
			cammie.audio.PlayOneShot(noooo);
			SetState(STATE_DEAD);
		}
	}
	
	void SpringJump(BounceOffTop bounceOffInfo){
		velocity.y = bounceOffInfo.springPower;
	}

	public void PickUpPickup(Pickup pickup){
		/*StartCoroutine("RemoveControlForTime",1.5f);
		skelAnim.state.SetAnimation(0,"pick up",false);*/
		pickup.Grab(gameObject);
	}

	public void PickUpItem(Item item){
		/*StartCoroutine("RemoveControlForTime",1.5f);
		skelAnim.state.SetAnimation(0,"pick up",false);*/
		item.Grab(gameObject);
	}

	public float distanceToMoveSwitchingRopeSides;
	IEnumerator cSwitchClimbingSide(string newSide){
		rigidBody.isKinematic = true;
		StartCoroutine("RemoveControlForTime",1f);
		float directionInt = Input.GetAxisRaw("Horizontal");
		if (directionInt < 0)
			facing = !LEFT;
		if (directionInt >0)
			facing = !RIGHT;
		UpdateRotationNormal(Time.smoothDeltaTime);
		if (newSide == "right"){
			//play switch left to right side animation
			//lerp to new location
			for (float i=0; i<.85f; i+=Time.deltaTime){
				transform.Translate(new Vector3(-distanceToMoveSwitchingRopeSides/(.75f/Time.deltaTime),0f,0f));
				yield return null;
			}
			timeSinceEnteredClimbing = 0f;
			SetState(STATE_CLIMBING_RIGHT);
		} else if (newSide == "left"){
			//play switch right to left side animation
			//lerp to new location
			for (float i=0; i<.85f; i+=Time.deltaTime){
				transform.Translate(new Vector3(distanceToMoveSwitchingRopeSides/(.75f/Time.deltaTime),0f,0f));
				yield return null;
			}
			timeSinceEnteredClimbing = 0f;
			SetState(STATE_CLIMBING_LEFT);
		}
		yield return new WaitForSeconds(.5f);
		rigidBody.isKinematic = false;
	}

	IEnumerator RemoveControlForTime(float time){
		canControl = false;
		yield return new WaitForSeconds(time);
		canControl = true;
	}

	IEnumerator CloseAndOpenApertureRespawn(Transform respawnLocation){
		canControl = false;
		aperture.Close();
		skelAnim.state.SetAnimation(0,"death",false);
		yield return new WaitForSeconds(1f);
		transform.position = respawnLocation.position;
		yield return new WaitForSeconds(1f);
		aperture.Open();
		canControl = true;
		SetState(STATE_NORMAL);
		yield return null;
	}

	
	//============== Accessor Methods ==================
	public bool IsSlamming(){
		return slamming;
	}
	
	public void SetState(short state){
		this.state = state;
	}
}

public class BasicEntityCollision {
	private Vector2 size;
	private Vector3 center;
	// Give a bit of space between the raycast and boxCollider to prevent ray going through collision layer.
	private float skin = 0f;
	
	private LayerMask collisionMask;
	private LayerMask playerMask;

	private Rigidbody2D rigidBody;

	public bool OnGround { get; set; }
	public bool SideCollision { get; set; }
	
	public void Init(BoxCollider2D boxCollider, Rigidbody2D rb) {
		size = boxCollider.size;
		center = boxCollider.center;
		rigidBody = rb;
	}
	
	public Vector3 Move(Vector2 moveAmount, Vector3 position, float dirX) {
		float deltaX = moveAmount.x;
		float deltaY = moveAmount.y;
		Vector3 entityPosition = position;
		// Resolve any possible collisions below and above the entity.
		deltaY = yAxisCollisions(deltaY, dirX, entityPosition);
		// Resolve any possible collisions left and right of the entity.
		// Check if our deltaX value is 0 to avoid unnecessary collision detection.
		if (deltaX != 0) {
			deltaX = xAxisCollisions(deltaX, entityPosition);
		}
		Vector3 finalTransform = new Vector2(deltaX, deltaY);
		return finalTransform;
	}
	
	private float xAxisCollisions(float deltaX, Vector3 entityPosition) {
		SideCollision = false;
		float i;
		// If we are on the ground, perform just three, normal sized raycasts.
		if (OnGround) {
			i = 0;
			// Else, perform a larger range of raycasts that extend slightly outside of
			// the box collider in order to prevent falling through corners in the Collisions layermask.
		} else {
			i = -0.01f;
		}
		for (; i < 2; ++i) {
			float dirX = Mathf.Sign(deltaX);
			
			float x = entityPosition.x + center.x + size.x / 2 * dirX;
			float y = (entityPosition.y + center.y - size.y / 2) + size.y / 2 * i *.9f + .05f;
			
			RaycastHit2D hit;
			Ray2D rayX = new Ray2D(new Vector2(x, y), new Vector2(dirX, 0));
			hit = Physics2D.Raycast(rayX.origin,rayX.direction, Mathf.Abs(deltaX));
			Debug.DrawRay(rayX.origin, rayX.direction);
			//Debug.Break();
			
			if (hit) {
				if (hit.normal != Vector2.up && hit.normal != Vector2.right && hit.normal != -Vector2.right) break;
				Debug.DrawRay(rayX.origin, rayX.direction, Color.yellow);
				deltaX = 0;
				SideCollision = true;
				break;
			}
		}
		
		return deltaX;
	}
	
	private float yAxisCollisions(float deltaY, float dirX, Vector3 entityPosition) {
		OnGround = false;
		// To prevent falling through collision layers by a gap in the corner
		// if we are facing right, peform y-axis raycasts starting on the right.
		int facingRight = 1;
		if (dirX == facingRight) {
			for (int i = 2; i > -1; --i) {
				if (yAxisRaycasts(i, ref deltaY, entityPosition)) {
					break;
				}
			}
			// else we are facing left, peform y-axis raycasts starting on the left
		} else {
			for (int i = 0; i < 3; ++i) {
				if (yAxisRaycasts(i, ref deltaY, entityPosition)) {
					break;
				}
			}
		}
		
		return deltaY;
	}
	
	private bool yAxisRaycasts(int i, ref float deltaY, Vector3 entityPosition) {
		float dirY = Mathf.Sign(deltaY);
		// Start at the left or the right of the boxCollider, depending on the value of i.
		float x = (entityPosition.x + center.x - size.x / 2) + size.x / 2 * i *.8f + .1f;
		// Bottom or top of boxCollider, depending on if dirY is positive or negative
		float y = entityPosition.y + center.y + size.y / 2 * dirY;
		
		RaycastHit2D hit;
		Ray2D ray = new Ray2D(new Vector2(x, y), new Vector2(0, dirY));
		hit = Physics2D.Raycast(ray.origin,ray.direction, Mathf.Abs(deltaY));
		Debug.DrawRay(ray.origin, ray.direction);
		
		if (hit) {
			if (hit.normal != Vector2.up && hit.normal != Vector2.right && hit.normal != -Vector2.right){
				rigidBody.isKinematic = true;
			} else {
				rigidBody.isKinematic = false;
			}
			Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
			// Get Distance between entity and ground
			float distance = Vector2.Distance(ray.origin, hit.point);
			// Stop entity's downward movement after coming within skin width of a boxCollider
			if (distance > skin) {
				deltaY = distance * dirY + skin;
			} else {
				deltaY = 0;
			}
			OnGround = true;
			return true;
		}
		
		return false;
	}
	public static void loadLevel(string sceneName)
	{
		LoadingScreen.show();
		Application.LoadLevel(sceneName);
	}
}
