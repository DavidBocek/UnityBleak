using UnityEngine;
using System.Collections;

public class BleakControllerModified : MonoBehaviour {

	public float runSpeed;
	public float walkFraction;
	public float jumpSpeed;
	public float pushSpeed;
	public float maxFallSpeed;
	public float deathFallSpeed;
	public float gravityAcceleration;
	public bool slamming = false;
	public float slamMultiplier;
	public bool canControl = true;

	public Transform startPoint;
	public Aperture aperture;
	public GameObject skelAnimObj;
	public AudioClip deathSoundClip;

	public const float SMALL_JUMP_TIME = .06f;
	public const float MED_JUMP_TIME = .08f;
	public const float LARGE_JUMP_TIME = .12f;


	private SkeletonAnimation skelAnim;
	private GameObject attachedClimbObject;
	private GameObject attachedPushObject;
	private GameObject attachedRideObject;
	private float idleDelay = 0f;
	private float jumping = 0f;
	private Vector2 velocity;
	private float dirX = 0;
	private float dt = 0f;


	//=== STATES ===
	private int state;

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


	void Start(){
		InitRayCollider();
		skelAnim = skelAnimObj.GetComponent<SkeletonAnimation>();
		transform.position = startPoint.position;
	}

	void Update(){
		switch(state){
		case STATE_NORMAL:
			dt = Time.smoothDeltaTime;
			velocity.x = 0;
			UpdateGravity(dt);
			UpdateJumping(dt);
			UpdateIdle(dt);
			UpdateHorizontalVelocity(dt);
			UpdateRotation(dt);
			MoveSelf(dt);
			ResolveCollisions(dt);
			break;
		case STATE_CLIMBING_LEFT:
			break;
		case STATE_CLIMBING_RIGHT:
			break;
		case STATE_DYING:
			break;
		case STATE_HURTING:
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
			skelAnim.state.SetAnimation(0,"death",false);
			if (Input.GetKeyDown(KeyCode.R)){
				transform.position = startPoint.position;
				numLives = 2;
				skelAnim.skeleton.SetSkin("damaged00");
				SetState(STATE_NORMAL);
			}
			break;
		case STATE_FORCE_MOVE:
			break;
		case STATE_PUSH_LEFT:
			break;
		case STATE_PUSH_RIGHT:
			break;
		default:
			throw new UnityException("unrecognized state value: "+state);
		}
	}

	/// <summary>
	/// Updates velocity based on gravity
	/// </summary>
	/// <param name="dt">Dt.</param>
	void UpdateGravity(float dt){
		Debug.Log (OnGround+" "+verticalCollision);
		if (!OnGround){
			float tempVertVel = velocity.y - gravityAcceleration * dt;
			velocity.y = Mathf.Max (tempVertVel,-maxFallSpeed);
		}
	}
	/// <summary>
	/// Updates velocity based on jumping commands
	/// </summary>
	/// <param name="dt">Dt.</param>
	void UpdateJumping(float dt){
		//jumping is 0 when on the ground, >0 for a split second while the jump impulse is occuring, and -1 after that before he hits the ground (so that you can't jump again in air)
		if (Input.GetButton("Jump") && jumping >= 0){
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
		if (OnGround && jumping == -1){
			slamming = false;
			jumping = 0;
		}
		
		//beginning a slam
		if (jumping == -1 && Input.GetButtonDown("Slam")){
			velocity.y = -jumpSpeed * slamMultiplier;
			slamming = true;
			//play slamming animation and sound
			skelAnim.state.SetAnimation(0,"jump-slam",false);
		}
	}
	/// <summary>
	/// Updates animations and variables during idleing
	/// </summary>
	/// <param name="dt">Dt.</param>
	void UpdateIdle(float dt){
		if (jumping>=0){
			//handle idle animations
			if (idleDelay >= 7)
				idleDelay = 0;
			else{
				if (Input.GetAxisRaw("Horizontal") == 0 && !Input.GetButton("Jump")){
					if (skelAnim.state.ToString()=="sprint"){
						skelAnim.state.ClearTrack(0);
						skelAnim.state.AddAnimation(0,"skid",false,0.0f);
						//skelAnim.state.AddAnimation(0,"idle",true,0.0f);
					} else if (skelAnim.state.ToString()=="run"){
						skelAnim.state.ClearTrack(0);
						if (numLives > 0) skelAnim.state.AddAnimation(0,"idle",true,0.0f);
						else skelAnim.state.AddAnimation(0,"idle-injured",true,0.0f);
					} else {
						if (numLives > 0) skelAnim.state.AddAnimation(0,"idle",true,0.0f);
						else skelAnim.state.AddAnimation(0,"idle-injured",true,0.0f);
					}
					idleDelay += dt;
				}
			}
		}
	}
	/// <summary>
	/// Updates the horizontal velocity.
	/// </summary>
	/// <param name="dt">Dt.</param>
	void UpdateHorizontalVelocity(float dt){
		dirX = Input.GetAxisRaw("Horizontal");
		if (dirX != 0){
			velocity.x = Input.GetButton ("Walk") ? runSpeed * walkFraction * dirX : runSpeed * dirX;
		}
		if (jumping != -1){
			if (Input.GetButton("Walk")){
				//play jogging anim
				if (skelAnim.state.ToString()!="run" && skelAnim.state.ToString()!="run-injured" && skelAnim.state.ToString()!="pick up"){
					if (numLives > 0) skelAnim.state.SetAnimation(0,"run",true);
					else skelAnim.state.SetAnimation(0,"run-injured",true);
				}
			} else {
				//play running anim
				if (skelAnim.state.ToString()!="sprint"){
					skelAnim.state.SetAnimation(0,"sprint",true);
				}
			}
		}
	}
	/// <summary>
	/// Moves this object, checking for collisions with raycasts
	/// </summary>
	/// <param name="dt">Dt.</param>
	void MoveSelf(float dt){
		transform.Translate(Move(velocity*dt,transform.position,dirX));
	}
	/// <summary>
	/// Resolves collisions.
	/// </summary>
	/// <param name="dt">Dt.</param>
	void ResolveCollisions(float dt){
		if (sideCollision){
			HandleCollisionSide(sideCollisionHitInfo.collider,dirX);
		}

		if (verticalCollision){
			HandleCollisionVertical(verticalCollisionHitInfo.collider,Mathf.Sign(velocity.y));
		}
	}
	/// <summary>
	/// Updates rotation
	/// </summary>
	/// <param name='dt'>
	/// time since last frame was called (in seconds)
	/// </param>
	void UpdateRotation(float dt){
		if (dirX<0 && skelAnimObj.transform.localScale.x!=-1.0f){
			skelAnimObj.transform.localScale = new Vector3(-1.0f,skelAnimObj.transform.localScale.y,skelAnimObj.transform.localScale.z);
		}
		if (dirX>0 && skelAnimObj.transform.localScale.x!=1.0f){
			skelAnimObj.transform.localScale = new Vector3(1.0f,skelAnimObj.transform.localScale.y,skelAnimObj.transform.localScale.z);
		}
	}

	//== Handlers for collisions ==

	void HandleCollisionSide(Collider2D collider, float dirX){
		Pickup pickup = collider.gameObject.GetComponent<Pickup>();
		if (pickup){
			PickUpPickup(pickup);
		}
		Item item = collider.gameObject.GetComponent<Item>();
		if (item){
			PickUpItem(item);
		}
		KillZones killZones = collider.gameObject.GetComponent<KillZones>();
		if (killZones){
			if (killZones.HasKillLeft() && dirX > 0)
				Damage();
			else if (killZones.HasKillRight() && dirX < 0)
				Damage();
		}
		Climbable climbable = collider.gameObject.GetComponent<Climbable>();
		if (climbable){
			if (dirX > 0) SetState(STATE_CLIMBING_RIGHT);
			else if (dirX < 0) SetState(STATE_CLIMBING_LEFT);
			attachedClimbObject = climbable.gameObject;
		}
		Pushable pushable = collider.gameObject.GetComponent<Pushable>();
		if (pushable){
			if (dirX > 0) SetState(STATE_PUSH_RIGHT);
			else if (dirX < 0) SetState(STATE_PUSH_LEFT);
			attachedPushObject = pushable.gameObject;
		}
	}
	void HandleCollisionVertical(Collider2D collider, float dirY){
		Breakable breakable = collider.gameObject.GetComponent<Breakable>();
		if (breakable){
			if (!breakable.isBreaking && breakable.breakableDown && dirY > 0)
				breakable.Break();
			if (!breakable.isBreaking && breakable.breakableUp && dirY <= 0)
				breakable.Break();
			if (!breakable.isBreaking && breakable.breakableUp){
				if (slamming){
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

		//kill zones component check, to see if bleak should die touching this
		KillZones killZones = collider.gameObject.GetComponent<KillZones>();
		if (killZones){
			if (killZones.HasKillUp() && dirY < 0){
				Damage();
			}
			if (killZones.HasKillDown() && dirY > 0){
				Damage();
			}
		}
		
		//back and forth knock over, to see if bleak should knock this down by jumping on it
		BackAndForthMovement backAndForth = collider.gameObject.GetComponent<BackAndForthMovement>();
		if (backAndForth){
			if (dirY <= 0)
				backAndForth.KnockDown();
		}
		
		//bounce off component check, to see if bleak should spring off this
		BounceOffTop bounceOff = collider.gameObject.GetComponent<BounceOffTop>();
		if (bounceOff && slamming && !bounceOff.IsSprung() && dirY <= 0){
			SpringJump(bounceOff);
			bounceOff.SetSprung(true);
			bounceOff.PlaySpringAnimation();
		}

			
		
		MoveWithObject rideable = collider.gameObject.GetComponent<MoveWithObject>();
		if (rideable){
			if (rideable.moveWithTop && dirY <= 0){
				attachedRideObject = rideable.gameObject;
			}
		}
	}

	//== unity events ==
	void OnCollisionEnter2D(Collision2D collision){
		HandleCollisionSide(collision.collider,dirX);
		HandleCollisionVertical(collision.collider,Mathf.Sign(velocity.y));
	}


	//== helper functions ==
	public int numLives;
	void Damage(){
		AudioSource.PlayClipAtPoint(deathSoundClip, transform.position);
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


	public void PickUpPickup(Pickup pickup){
		StartCoroutine("RemoveControlForTime",1.5f);
		skelAnim.state.SetAnimation(0,"pick up",false);
		pickup.Grab(gameObject);
	}
	
	public void PickUpItem(Item item){
		StartCoroutine("RemoveControlForTime",1.5f);
		skelAnim.state.SetAnimation(0,"pick up",false);
		item.Grab(gameObject);
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



	void SetState(int state){
		this.state = state;
	}





	// =========== BASIC RAYCASTING COLLISIONS =============
	private Vector2 size;
	private Vector2 center;
	// Give a bit of space between the raycast and boxCollider to prevent ray going through collision layer.
	private float skin = .1f;

	public float acceptableSlopeAngle = 45f;
	public bool OnGround { get; set; }
	public bool sideCollision;
	public RaycastHit2D sideCollisionHitInfo;
	public bool verticalCollision;
	public RaycastHit2D verticalCollisionHitInfo;

	private bool slopeHitHoriz = false;
	private RaycastHit2D slopeHitInfo;
	
	public void InitRayCollider() {
		size = GetComponent<BoxCollider2D>().size;
		center = GetComponent<BoxCollider2D>().center;
	}
	
	public Vector3 Move(Vector2 moveAmount, Vector3 position, float dirX) {
		float deltaX = moveAmount.x;
		float deltaY = moveAmount.y;
		float deltaXcopy = moveAmount.x;
		Vector3 entityPosition = position;
		// Resolve any possible collisions below and above the entity.
		deltaY = yAxisCollisions(deltaY, dirX, entityPosition);
		// Resolve any possible collisions left and right of the entity.
		// Check if our deltaX value is 0 to avoid unnecessary collision detection.
		if (deltaX != 0) {
			deltaX = xAxisCollisions(deltaX, entityPosition);
		}

		if (slopeHitHoriz){
			//find new displacement up the slope if the value is acceptable
			//rotate 90 deg if on right, -90 if on left
			Vector2 slopeVector = new Vector2();
			if (dirX >0){
				slopeVector.x = slopeHitInfo.normal.y; slopeVector.y = -slopeHitInfo.normal.x;
			}
			else if (dirX < 0){
				slopeVector.x = -slopeHitInfo.normal.y; slopeVector.y = slopeHitInfo.normal.x;
			}
			if (Mathf.Atan2(slopeVector.y,slopeVector.x)*Mathf.Rad2Deg <= acceptableSlopeAngle){
				Vector2 totalMovement = new Vector2((slopeHitInfo.fraction),0);
				totalMovement += (deltaXcopy-slopeHitInfo.fraction)*slopeVector*Mathf.Sign(deltaX);
				deltaX = totalMovement.x; deltaY = totalMovement.y;
			}
		}

		Vector3 finalTransform = new Vector2(deltaX, deltaY);
		return finalTransform;
	}
	
	private float xAxisCollisions(float deltaX, Vector3 entityPosition) {
		slopeHitHoriz = false;
		sideCollision = false;
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
				Debug.DrawRay(rayX.origin, rayX.direction, Color.yellow);
				if (hit.normal != Vector2.up && hit.normal != Vector2.right && hit.normal != -Vector2.right){
					slopeHitHoriz = true;
					slopeHitInfo = hit;
				}
				if (hit.fraction <= deltaX){
					sideCollisionHitInfo = hit;
					sideCollision = true;
				}
				deltaX = Mathf.Min (deltaX,hit.fraction);
			}
		}
		if (slopeHitHoriz){
			//if the slope touching ray isn't the one that hit closest, then we won't handle it like a slope
			if (slopeHitInfo.fraction > deltaX){
				slopeHitHoriz = false;
			}
		}
		
		return deltaX;
	}
	
	private float yAxisCollisions(float deltaY, float dirX, Vector3 entityPosition) {
		OnGround = false;
		verticalCollision = false;
		// To prevent falling through collision layers by a gap in the corner
		// if we are facing right, peform y-axis raycasts starting on the right.
		int facingRight = 1;
		if (dirX == facingRight) {
			for (int i = 2; i > -1; --i) {
				//alters deltaY
				yAxisRaycasts(i, ref deltaY, entityPosition);
			}
			// else we are facing left, peform y-axis raycasts starting on the left
		} else {
			for (int i = 0; i < 3; ++i) {
				//alters deltaY
				yAxisRaycasts(i, ref deltaY, entityPosition);
			}
		}
		
		return deltaY;
	}
	
	private void yAxisRaycasts(int i, ref float deltaY, Vector3 entityPosition) {
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
			Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
			// Get Distance between entity and ground
			float distance = Vector2.Distance(ray.origin, hit.point);
			// Stop entity's downward movement after coming within skin width of a boxCollider
			if (distance > skin) {
				if (distance*dirY+skin <= deltaY){
					verticalCollisionHitInfo = hit;
				}
				deltaY = Mathf.Min (deltaY, distance * dirY + skin);
			} else {
				verticalCollisionHitInfo = hit;
				deltaY = 0;
			}

			if (distance <= deltaY){
				OnGround = true;
				verticalCollision = true;
			}
		}
	}
}
