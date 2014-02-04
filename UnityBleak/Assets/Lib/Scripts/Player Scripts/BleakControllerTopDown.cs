using UnityEngine;
using System.Collections;

public class BleakControllerTopDown : MonoBehaviour {
	
	public float walkSpeed;

	private CircleCollider2D collider;
	private Rigidbody2D rigidBody;
	private SkeletonAnimation skelAnim;
	public GameObject skelAnimObj;
	public float changeSpeed;
	public float lagSpeed;

	public bool canControl = true;

	private Vector2 velocity;
	private Vector2 position;

	void Start(){
		rigidBody = GetComponent<Rigidbody2D>();
		collider = GetComponent<CircleCollider2D>();
		skelAnim = skelAnimObj.GetComponent<SkeletonAnimation>();
	}

	private float dt;
	void Update(){
		dt = Time.smoothDeltaTime;

		position = new Vector2(transform.position[0],transform.position[1]);

		float xInput = Input.GetAxis("Horizontal");
		float yInput = Input.GetAxis("Vertical");

		if (xInput != 0 || yInput != 0)
		{
			velocity.x = xInput * walkSpeed;
			velocity.y = yInput * walkSpeed;
			//velocity.Normalize();
			if((xInput > changeSpeed) && (yInput > -lagSpeed) && (yInput < lagSpeed))
			{
				if (skelAnim.state.ToString()!="walkingRight")
				{
					skelAnim.state.SetAnimation(0,"walkingRight",true);
				}
			}
			if((xInput>changeSpeed)&&(yInput>changeSpeed))
			{
				if (skelAnim.state.ToString()!="walkingUp-right")
				{
					skelAnim.state.SetAnimation(0,"walkingUp-right",true);
				}
			}
			if((xInput < -changeSpeed)&&(yInput > -lagSpeed) && (yInput < lagSpeed))
			{
				if (skelAnim.state.ToString()!="walkingLeft")
				{
					skelAnim.state.SetAnimation(0,"walkingLeft",true);
				}
			}
			if((xInput<-changeSpeed)&&(yInput>changeSpeed))
			{
				if (skelAnim.state.ToString()!="walkingUp-left")
				{
					skelAnim.state.SetAnimation(0,"walkingUp-left",true);
				}
			}
			if((yInput > changeSpeed) && (xInput > -lagSpeed) && (xInput < lagSpeed))
			{
				if (skelAnim.state.ToString()!="walkingUp")
				{
					skelAnim.state.SetAnimation(0,"walkingUp",true);
				}
			}
			if((xInput<-changeSpeed)&&(yInput<-changeSpeed))
			{
				if (skelAnim.state.ToString()!="walkingDown-left")
				{
					skelAnim.state.SetAnimation(0,"walkingDown-left",true);
				}
			}
			if((yInput < -changeSpeed) && (xInput > -lagSpeed) && (xInput < lagSpeed))
			{
				if (skelAnim.state.ToString()!="walkingDown")
				{
					skelAnim.state.SetAnimation(0,"walkingDown",true);
				}
			}
			if((xInput>changeSpeed)&&(yInput<-changeSpeed))
			{
				if (skelAnim.state.ToString()!="walkingDown-right")
				{
					skelAnim.state.SetAnimation(0,"walkingDown-right",true);
				}
			}
		} else {
			switch (skelAnim.state.ToString()){
			case "walkingRight":
				skelAnim.state.SetAnimation(0,"idleRight",true);
				break;
			case "walkingUp-right":
				skelAnim.state.SetAnimation(0,"idleUp-right",true);
				break;
			case "walkingUp":
				skelAnim.state.SetAnimation(0,"idleUp",true);
				break;
			case "walkingUp-left":
				skelAnim.state.SetAnimation(0,"idleUp-left",true);
				break;
			case "walkingLeft":
				skelAnim.state.SetAnimation(0,"idleLeft",true);
				break;
			case "walkingDown-left":
				skelAnim.state.SetAnimation(0,"idleDown-left",true);
				break;
			case "walkingDown":
				skelAnim.state.SetAnimation(0,"idleDown",true);
				break;
			case "walkingDown-right":
				skelAnim.state.SetAnimation(0,"idleDown-right",true);
				break;
			}
		}
		UpdateInput(dt);
		UpdatePositionChangeNormal(dt);
	}


	void UpdateInput(float dt){
		if (Input.GetKeyDown(KeyCode.E) && canControl){
			Messenger.Broadcast<Transform,BleakControllerTopDown>("BleakInteractionActivateTopDown",transform,this);
		}
	}

	void UpdatePositionChangeNormal(float dt){

		rigidBody.velocity = velocity;
	}

}

