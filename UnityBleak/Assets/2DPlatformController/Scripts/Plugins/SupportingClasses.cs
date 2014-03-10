using UnityEngine;
using System.Collections;

/// <summary>
/// Raycast Directions.
/// </summary>
public enum RC_Direction {UP, DOWN, LEFT, RIGHT};

/// <summary>
/// Movement details.
/// </summary>
[System.Serializable]
public class MovementDetails {
	/// <summary>
	/// Speed the character walks at.
	/// </summary>
	public float walkSpeed = 3.0f;
	/// <summary>
	/// Speed the chracter runs at.
	/// </summary>
	public float runSpeed = 5.0f;
	/// <summary>
	/// Allows for different moement styles, see defintiions in the enum (MovementStyle).
	/// </summary>
	public MovementStyle movementStyle = MovementStyle.PHYSICS_LIKE;
	/// <summary>
	/// If true when you jump you can move around in X at the run speed. Default is to use walk speed. 
	/// This does not apply if movement is PHYSICS STYLE. In that case use jump drag instead.
	/// </summary>
	public bool jumpAtRunSpeed = false;
	/// <summary>
	/// The acceleration to apply when input is left or right.
	/// </summary>
	public float acceleration = 75.0f;	
	/// <summary>
	/// The drag to apply each frame.
	/// </summary>
	public float drag = 1.15f;	
	/// <summary>
	/// The terminal velocity in the y direction. 
	/// WARNING: If this is too large you will be able to fall through platforms. Make sure maxframeTime * terminalVelcoty < feetCollider.distance.
	/// </summary>
	public float terminalVelocity = -12.0f;
	/// <summary>
	/// Minimum movement distance, used to stop shaking.
	/// </summary>
	public float skinSize = 0.001f;
}

/// <summary>
/// Slope details.
/// </summary>
[System.Serializable]
public class SlopeDetails {
	/// <summary>
	/// Set to true if the character can handle slopes.
	/// </summary>
	public bool allowSlopes = false;
	/// <summary>
	/// How far to look ahead when determinin gif the character is on a slope. Large values will make the character tilt in the air.
	/// </summary>
	public float slopeLookAhead = 0.5f;
	/// <summary>
	/// How fast to rotate to the sloped position (and back again)
	/// </summary>
	public float rotationSpeed = 0.25f;
	/// <summary>
	/// The maximum permissable rotation in degrees.
	/// </summary>
	public float maxRotation = 45f;
}

/// <summary>
/// Jump details. Note that jumps are also affected by the Physics.gravity setting.
/// </summary>
[System.Serializable]
public class JumpDetails {
	/// <summary>
	/// Set to true to enable double jump.
	/// </summary>
	public bool canDoubleJump = false;
	/// <summary>
	/// Set to true to enable wall jumps.
	/// </summary>
	/// <summary>
	/// How fast the jump is.
	/// </summary>
	public float jumpVelocity = 10.0f;	
	/// <summary>
	/// How fast the doublejump is.
	/// </summary>	
	public float doubleJumpVelocity = 8.0f;	
	/// <summary>
	/// Controls how long you are considered to be jumping. If this is too small you wont be able
	/// to jump away from climables. If its too large you wont be able to quickly jump twice in a row.
	/// </summary>
	public float jumpTimer = 0.2f;
	/// <summary>
	/// How long after pressing jump it can be held down to add extra force. use this if you want jumps to
	/// get bigger when you hold jump.
	/// </summary>
	public float jumpHeldTime = 0.25f;
	/// <summary>
	/// How much extra acceleration is added when you hold the jump button down. Set to zero for fixed height jumps.
	/// </summary>
	public float jumpFrameVelocity = 25.0f;
	/// <summary>
	/// The amount of drag in the air. When you jump you move/change direction at walk speed. By setting the drag 
	/// low you will get extra distance when you do a running jump.
	/// </summary>
	public float drag = 1.005f;
	/// <summary>
	/// The velocity required before the fall event replaces the airbourne event (negative number).
	/// </summary>
	public float fallVelocity = -1.0f;
	/// <summary>
	/// How much of the parent velocity should we inherit when we jump off of a moving parent.
	/// </summary>
	public float inheritParentVelocityFactor = 0.0f;
}

/// <summary>
/// Wall details. Controls wall jump and wall slide.
/// </summary>
[System.Serializable]
public class WallDetails {
	public bool canWallJump = false;
	/// <summary>
	/// Can the character slide down the wall 
	/// </summary>
	public bool canWallSlide = false;
	/// <summary>
	/// If true, character needs to jump while holding against a wall to wall jump. Otherwise they need
	/// to also time it so that they hold against the wall and then hit the opposite direction as they jump.
	/// </summary>
	public bool easyWallJump = true;
	/// <summary>
	/// If true when wall jumping you always jump away from wall. If false you can also jump directly upwards.
	/// Only has an effect when easyWallJump is enabled as if easyWallJump = false, you must hold away and therefore
	/// jump away
	/// from wall.
	/// </summary>
	public bool wallJumpOnlyInOppositeDirection = true;
	/// <summary>
	/// The reduciton in gravity when the character is wall sliding (only used if wallSlide = true). Set to
	/// 0 to make it stick.
	/// </summary>
	public float wallSlideGravityFactor = 0.33f;
	/// <summary>
	/// The time where user input is ingored in the x direction when wall jupming away from a wall.
	/// </summary>
	public float oppositeDirectionTime = 0.5f;
	/// <summary>
	/// Additional distance to add to side collider hit checks when determining if we are wall sliding.
	/// </summary>
	public float wallSlideAdditionalDistance = 0.05f;
	/// <summary>
	/// Amount off leeway between pushing against a wall, and being able to wall jump 
	/// by holding opposite direction and pressing jump.
	/// </summary>
	public float wallJumpTime = 0.33f;
	/// <summary>
	/// The offset for the extra raycast used for edge detection.
	/// Note the y value is the y offset but that the x value is used as a scalar
	/// distance (i.e. changes based on direction).
	/// </summary>
	public Vector2 edgeDetectionOffset;
	/// <summary>
	/// If this is not null/empty string then the character can only wall jump from walls with this tag.
	/// </summary>
	public string wallJumpTag;
	/// <summary>
	/// The wall jump speed. Y value is equivalent to jump velocity, x value is (initial) horizontal 
	/// speed but only applied if wallJumpInOppositeDirectionOnly is set to true.
	/// </summary>
	public Vector2 wallJumpSpeed;
}

/// <summary>
/// Climb details.
/// </summary>
[System.Serializable]
public class ClimbDetails {
	/// <summary>
	/// If true you will autoamtically stick to climables when you touch them. Otherwise you 
	/// will need to press up or down to stick.
	/// </summary>
	public bool autoStick = false;
	/// <summary>
	/// ALlow climbing if true.
	/// </summary>
	public bool allowClimbing = true;
	/// <summary>
	/// The vertical speed at which ladders are climbed.
	/// </summary> 
	public float speed = 2.5f;
	
	/// <summary>
	/// If the ladder allows climbing horizontally use this speed.
	/// </summary> 
	public float horizontalSpeed = 1.5f;
	
	/// <summary>
	/// How many feet colliders are required to be on the ladder before it can be climbed. cannot be larger
	/// than total eet colliders. Larger numbers make ladders "thinner", i.e. you have to be closer to the
	/// centre of the ladder to climb them.
	/// </summary> 
	public int collidersRequired = 3;
	
	/// <summary>
	/// How much to accentuate the x velocity of the rope when you launch off of it.
	/// </summary>
	public float ropeVelocityFactor = 1.33f;
	/// <summary>
	/// How much force to impart to the rope when swinging.
	/// </summary>
	public float ropeSwingForce = 1.5f;
	
	/// <summary>
	/// The time it takes to play the climbing the ladder top animation
	/// </summary>
	public float climbTopAnimationTime = 1.5f;
	
	/// <summary>
	/// The time it takes to play the climbing the ladder top downanimation
	/// </summary>
	public float climbTopDownAnimationTime = 1.5f;
	
	/// <summary>
	/// The difference between the characters root position
	/// at the top of a climb, and during the normal state (idle, walking, etc).
	/// </summary>
	public Vector3 climbOffset;
}

/// <summary>
/// Ledge details. Control what happens when you hang off a ledge.
/// </summary>
[System.Serializable]
public class LedgeDetails {
	/// <summary>
	/// Can character hang and climb ledge.
	/// </summary>
	public bool canLedgeHang = true;
	
	/// <summary>
	/// If true the character will grab and autograb ledges only if facing them.
	/// If false character can also grab ledges behind them.
	/// </summary>
	public bool grabOnlyInFacingDirection = true;
	
	/// <summary>
	/// If true the character will try to grab ledges near them without user input.
	/// If false the user needs to hold towards the ledge to grab.
	/// </summary>
	public bool autoGrab;
	
	/// <summary>
	/// The distance used to calcualte autograbs, should be very small or
	/// else the character will grab ledges when they shouldn't be able 
	/// to.
	/// </summary>
	public float autoGrabDistance = 0.1f;
	
	/// <summary>
	/// When a ledge hang starts the edge detection offset is added to the ledge hang raycast 
	/// and if an impassable object is detected it will cancel the hang. 
	/// Note the y value is the y offset but that the x value is used as a scalar
	/// distance (i.e. changes based on direction).
	/// </summary>
	public Vector2 edgeDetectionOffset;
	
	/// <summary>
	/// If false the character can jump directly upwards from a ledge hang. Otherwise
	// they can jump only jump only in the opposite direction to which they are holding.
	/// </summary>
	public bool jumpOnlyInOppositeDirection = true;
	
	/// <summary>
	/// The time where user input is ingored in the x direction when wall jupming away from a ledge.
	/// </summary>
	public float oppositeDirectionTime = 0.5f;
	
	/// <summary>
	/// Velocity imparted by a jump from a ledge hang. If this is zero jump
	/// will simply cause the cahracter to fall from the ledge (and the jump
	/// animation will not be played).
	/// </summary>
	public float jumpVelocity = 7.0f;
	
	/// <summary>
	/// The point where the character hangs from.
	/// </summary>
	public Vector3 hangOffset;
	
	/// <summary>
	/// How far above the highest collider is the grasp point (i.e. the cahracters
	/// hands when grasping for a ledge).
	/// </summary>
	public float graspPoint = 0.4f;
	
	/// <summary>
	/// How much leeway to use when cacluating the grasp. A bigger number
	/// makes it easier to ledge hang, but it will looks incorrect
	/// if the value is too large.
	/// </summary>
	public float graspLeeway = 0.1f;
	
	/// <summary>
	/// The time it takes to get to the hang position in seconds.
	/// </summary>
	public float transitionTime;
	
	/// <summary>
	/// The time it takes to climb in seconds.
	/// </summary>
	public float climbTime;
	
	/// <summary>
	/// The difference between the characters root position
	/// at the top of a climb, and during the normal state (idle, walking, etc).
	/// </summary>
	public Vector3 climbOffset;
	
	/// <summary>
	/// The time after dropping off a ledge where feet colliders will be ignored.The lower the better
	/// but you may need to increase if you have tiled or complex geometry.
	/// </summary>
	public float ledgeDropTime = 0.15f;
	
}

/// <summary>
/// Crouch details. Control what happens when you press down.
/// </summary>
[System.Serializable]
public class CrouchDetails {
	/// <summary>
	/// Can character crouch.
	/// </summary>
	public bool canCrouch = true;
	
	/// <summary>
	/// Can character slide along in a crouch position.
	/// </summary>
	public bool canCrouchSlide = true;
	
	/// <summary>
	/// If true the character can maintain a crouch position while jumping.
	/// </summary>
	public bool canCrouchJump = false;
	
	/// <summary>
	/// How fast character needs to be moving in the x direction
	/// before a crouch slide is initiated.
	/// </summary>
	public float minVelocityForSlide = 3.0f;
	
	/// <summary>
	/// Once crouch sliding how slow does the character need to be going for the 
	/// slide to stop.
	/// </summary>
	public float minVelocityForStopSlide = 1.75f;
	
	/// <summary>
	///  How much drag is applied when crouch sliding.
	/// </summary>
	public float slideDrag = 0.6f;
	
	/// <summary>
	/// If true we will automatically shrink the characters head colliders by the
	/// the height reduction factor. The alternative is to attach the head colliders
	/// to transforms, or listen for events and handle it compeltely in your own code.
	/// </summary>
	public bool useHeightReduction;
	
	/// <summary>
	/// If use heightRecution = true then this is amount the characters head colliders 
	/// will be reduced by. Specifically the head collider distance is scaled by this factor
	/// and then the offset downwards by (0.5f * new distance)
	/// </summary>
	public float heightReductionFactor = 2f;
	
	/// <summary>
	/// If use heightRecution = true then any side collider higher (determined by its
	/// yOffset only) than this value will be ignored while crouching.
	/// </summary>
	public float ignoredSideCollidersHigherThan = 0f;
	
	/// <summary>
	/// If you are not usinng the automatic height recution (i.e. heightReduction = false),
	/// then this value will be added to your head collider distance when determining if
	/// character is able to stand up. This is used to prevent the character standing up while
	/// sliding under a platform.
	/// </summary>
	public float headDetectionDistance;
}


/// <summary>
/// Swim details. Control what happens when you are swimming.
/// </summary>
[System.Serializable]
public class SwimDetails {
	/// <summary>
	/// Can character swim.
	/// </summary>
	public bool canSwim = true;
	
	/// <summary>
	/// How much force is imparted by a swim stroke in both x and y.
	/// </summary>
	public Vector2 swimStrokeAcceleration;
	
	/// <summary>
	/// Cap on how fast the character can move upwards.
	/// </summary>
	public float maxYSpeed;
	
	/// <summary>
	/// Overriden gravity that applies while underwater.
	/// </summary>
	public float gravityOverride;
	
	/// <summary>
	/// How much the character is slowed down by the water in X and Y. 1 means completely, 0 means not at all.
	/// </summary>
	public Vector2 waterResistance;
	
	/// <summary>
	/// The time between swim strokes.
	/// </summary>
	public float swimStrokeTime;
	
	/// <summary>
	/// Can the character run along the ground while underwater. 
	/// </summary>
	public bool canRun = true;
	
}


public enum StunType {
	// Stop the player inputs but still allow character to move
	STOP_INPUT_ONLY, 
	// Stop the player inputs and X movement, but still allow character to rise/fall
	STOP_INPUT_AND_X_MOVEMENT, 
	// Stop the player inputs and Y movement, but still allow character to slide
	STOP_INPUT_AND_Y_MOVEMENT, 
	// Stop the player inputs and all movement
	STOP_INPUT_AND_ALL_MOVEMENT
}

/// <summary>
/// Character animation states. The int value represents the animation priority.
/// If you insert new animations in here be sure to give them a unique priority value.
/// </summary>
public enum CharacterState {
	
	NONE 			=  -1,
	
	IDLE 			=  00,
	WALKING			=  10,
	RUNNING 		=  20,
	SLIDING 		=  30,			// If you don't want sliding animations change the maxSpeedForIdle parameter
	
	AIRBORNE		= 110,			// This is sent when you are in the air but are moving up or haven't reached the fall velocity
	FALLING 		= 120,
	AIRBORNE_CROUCH	= 130,			// This is sent if you are crouching during airborne or falling state
	
	WALL_SLIDING	= 160,
	
	
	CROUCHING		= 180,
	CROUCH_SLIDING 	= 190,
	
	HOLDING 		= 210,  		// On a ladder/climable other than a rope
	CLIMBING		= 220,  		// As above but moving up and down
	CLIMB_TOP_OF_LADDER_UP = 231,	// At the very top of ladder climbing up
	CLIMB_TOP_OF_LADDER_DOWN = 232,	// At the very top of ladder climbing down
	LEDGE_HANGING	= 240,
	LEDGE_CLIMBING	= 250,
	LEDGE_CLIMB_FINISHED = 260,
	ROPE_CLIMBING	= 270,
	ROPE_HANGING	= 280,
	ROPE_SWING		= 290,			// Note this is sent only when the player initiates a swing (1 frame)
	// if you need to check moving on rope vs hanging use character.myParent.velocity
	
	SWIMMING		= 300, 			// Character is doing a swim stroke (sent the frame you stroke)
	
	JUMPING 		= 410,			// This is sent on the frame you start your jump
	DOUBLE_JUMPING 	= 420,			// This is sent on the frame you start your double jump
	WALL_JUMPING 	= 430,			// This is sent on the frame you start your wall jump
	
	PUSHING			= 510,
	PULLING			= 520,
	
	STUNNED 		= 1000
};

public enum LedgeHangingState {
	TRANSITION,
	HANG,
	CLIMBING,
	FINISHED
}

public enum LadderTopState {
	CLIMBING_UP,
	FINISHED_UP,
	CLIMBING_DOWN,
	CLIMBING_DOWN_ACTION,
	CLIMBING_DOWN_PAUSE,
	FINISHED_DOWN
}

public enum MovementStyle {
	PHYSICS_LIKE, 			// Uses accerlation and drag to give a physics like movement style (this is how it always worked prior to v1.7.2)
	DIGITAL,				// Pressing a direction immediately changes speed to walk/run speed. NO accerlation or sliding.
	DIGITAL_WITH_SLIDE		// As above but character still slides to a halt.
}

/// <summary>
/// Character controller animation event delegate.
/// </summary>
public class CharacterControllerEventDelegate {
	
}