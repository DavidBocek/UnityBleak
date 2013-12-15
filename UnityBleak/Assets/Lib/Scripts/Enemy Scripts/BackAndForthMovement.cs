using UnityEngine;
using System.Collections;

public class BackAndForthMovement : MonoBehaviour {
	
	public Transform startPos;
	public Transform destPos;
	public float moveSpeed;
	public int numberOfBounces;
	public float downDelay = 1.5f;
	public bool canBeKnockedDown = false;
	public bool hasKnockDownAnimation = false;
	SkeletonAnimation skeletonAnimation;
	
	private bool movingLeft;
	private bool movingRight;
	private Vector3 leftPoint;
	private Vector3 rightPoint;
	private float lastTouchTime;
	private float journeyLength;
	private float fracJourney;
	private float distCovered;
	private int bounceCounter;
	private bool _moving;
	public bool moving {
		get{
			return _moving;
		} set {
			_moving = value;
		}
	}
	private float downTimer = 1.5f;
	
	private uint state = 0;
	private const uint STATE_NORMAL = 0;
	private const uint STATE_STOPPED = 1;
	
	private float epsilon = .1f;
	
	// Use this for initialization
	void Start () {
		skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
		transform.position = startPos.position;
		if (destPos.position.x >= startPos.position.x){
			movingRight = true;
			movingLeft = false;
			rightPoint = destPos.position;
			leftPoint = startPos.position;
		} else if (destPos.position.x < startPos.position.x){
			movingRight = false;
			movingLeft = true;
			rightPoint = startPos.position;
			leftPoint = destPos.position;
		}
		lastTouchTime = Time.time;
		journeyLength = Vector3.Distance(startPos.position, destPos.position);
		moving = true;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state){
		case STATE_NORMAL:
			if ((bounceCounter < numberOfBounces || numberOfBounces == -1) && _moving){
				if (movingRight){
					if (transform.position.x >= rightPoint.x - epsilon){
						movingRight = false;
						movingLeft = true;
						lastTouchTime = Time.time;
						if (numberOfBounces != -1) bounceCounter++;
					} else {
						distCovered = (Time.time - lastTouchTime) * moveSpeed;
		        		fracJourney = distCovered / journeyLength;
		        		transform.position = Vector3.Lerp(leftPoint, rightPoint, fracJourney);
					}
				} else if (movingLeft){
					if (transform.position.x <= leftPoint.x + epsilon){
						movingRight = true;
						movingLeft = false;
						lastTouchTime = Time.time;
						if (numberOfBounces != -1) bounceCounter++;
					} else {
						distCovered = (Time.time - lastTouchTime) * moveSpeed;
		        		fracJourney = distCovered / journeyLength;
		        		transform.position = Vector3.Lerp(rightPoint, leftPoint, fracJourney);
					}
				}
			}
			break;
		case STATE_STOPPED:
			lastTouchTime += Time.deltaTime;
			if (downTimer <= 0){
				state = STATE_NORMAL;
				downTimer = downDelay;
				if (hasKnockDownAnimation) skeletonAnimation.state.SetAnimation(0,"stand up",false);
				skeletonAnimation.state.AddAnimation(0,"walking",true,0.0f);
			} else {
				downTimer -= Time.deltaTime;
			}
			break;
		}
	}
	
	
	public void KnockDown(){
		if (canBeKnockedDown){
			if (hasKnockDownAnimation && state == STATE_NORMAL){
				skeletonAnimation.state.SetAnimation(0,"sit down",false);
			}
			state = STATE_STOPPED;
			downTimer = downDelay;
		}
	}
}
