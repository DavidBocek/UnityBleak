using UnityEngine;
using System.Collections;
using Spine;
using System;

public class BleakMovement : MonoBehaviour {

	SkeletonAnimation skeletonAnimation;

	public void Start () {
		skeletonAnimation = GetComponent<SkeletonAnimation>();	
		skeletonAnimation.state.Event += Event;
	}
	
	public void Event (object sender, EventTriggeredArgs e) {
		Debug.Log(e.TrackIndex + " " + skeletonAnimation.state.GetCurrent(e.TrackIndex) + ": event " + e.Event + ", " + e.Event.Int);
	}
	public void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			skeletonAnimation.state.SetAnimation(0, "jump", false);
			skeletonAnimation.state.AddAnimation(0, "idle", true, 0);
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			gameObject.transform.localScale = new Vector3(-1,1,1);
			skeletonAnimation.state.SetAnimation(0, "run", true);
		}
		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			gameObject.transform.localScale = new Vector3(1,1,1);
			skeletonAnimation.state.SetAnimation(0, "run", true);
		}
		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			skeletonAnimation.state.SetAnimation(0, "jump-slam", false);
			skeletonAnimation.state.AddAnimation(0, "idle", true, 0);
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow))
		{
			skeletonAnimation.state.SetAnimation(0, "idle", true);
		}
		if(Input.GetKeyUp(KeyCode.RightArrow))
		{
			skeletonAnimation.state.SetAnimation(0, "idle", true);
		}
	}
}
