using UnityEngine;
using System.Collections;

public class MoveWithObject : MonoBehaviour {

	public bool moveWithTop;
	public bool moveWithRight;
	public bool moveWithLeft;
	public bool moveWithBottom;
	
	private Vector2 oldPosition;
	private Vector2 moveDeltaLastFrame;
	
	public Vector2 GetMoveDeltaLastFrame(){
		return moveDeltaLastFrame;
	}

	void LateUpdate(){
		moveDeltaLastFrame = (new Vector2(transform.position.x,transform.position.y) - oldPosition);
		oldPosition = transform.position;
	}

}
