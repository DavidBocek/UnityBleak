using UnityEngine;
using System.Collections;

public class BleakControllerTopDown : MonoBehaviour {
	
	public float walkSpeed;

	private CircleCollider2D collider;
	private Rigidbody2D rigidBody;

	private Vector2 velocity;
	private Vector2 position;

	void Start(){
		rigidBody = GetComponent<Rigidbody2D>();
		collider = GetComponent<CircleCollider2D>();
	}

	private float dt;
	void Update(){
		dt = Time.smoothDeltaTime;

		position = new Vector2(transform.position[0],transform.position[1]);

		float xInput = Input.GetAxis("Horizontal");
		float yInput = Input.GetAxis("Vertical");

		if (xInput != 0 || yInput != 0){
			velocity.x = xInput;
			velocity.y = yInput;
			velocity.Normalize();
		}

		UpdatePositionChangeNormal(dt);
	}

	void UpdatePositionChangeNormal(float dt){

		/*RaycastHit2D rayHit0 = Physics2D.Raycast(position+collider.center+Vector2.right*collider.radius);
		RaycastHit2D rayHit45 = Physics2D.Raycast(position+collider.center+(new Vector2(1f,1f)).normalized*collider.radius);
		RaycastHit2D rayHit90 = Physics2D.Raycast(position+collider.center+Vector2.up*collider.radius);
		RaycastHit2D rayHit135 = Physics2D.Raycast(position+collider.center+(new Vector2(-1f,1f)).normalized*collider.radius);
		RaycastHit2D rayHit180 = Physics2D.Raycast(position+collider.center+-Vector2.right*collider.radius);
		RaycastHit2D rayHit225 = Physics2D.Raycast(position+collider.center+(new Vector2(-1f,-1f)).normalized*collider.radius);
		RaycastHit2D rayHit270 = Physics2D.Raycast(position+collider.center+-Vector2.up*collider.radius);
		RaycastHit2D rayHit315 = Physics2D.Raycast(position+collider.center+(new Vector2(1f,-1f)).normalized*collider.radius);


		if (rayHit0 || rayHit45 || rayHit90 || rayHit135 || rayHit180 || rayHit225 || rayHit270 || rayHit315){

		}*/

		rigidBody.velocity = velocity;
	}

}

