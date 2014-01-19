using UnityEngine;
using System.Collections;

public class UIBasicAnimation : MonoBehaviour {
	
	public float offset;
	public float speed = 1f;
	public bool x;
	public bool y;

	SkeletonAnimation skeletonAnimation;
	private bool isOpening = false;
	private bool isClosing = false;
	private BleakUIElement p_uiElement;
	private float movedAmount = 0f;

	private const float epsilon = .1f;

	// Use this for initialization
	void Start () {
		p_uiElement = GetComponent<BleakUIElement>();
		skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
	}
	
	// Update is called once per frame
	void Update () {
		if (x){
			if (isOpening){
				if (Mathf.Abs (movedAmount - offset) <= epsilon){
					isOpening = false;
					p_uiElement.FinishWorking();
					movedAmount = 0f;
					skeletonAnimation.state.SetAnimation(0,"openLeftPanel",false);
				} else {
					transform.position= transform.position + new Vector3(offset*speed*Time.smoothDeltaTime,0f,0f);
					movedAmount += offset*speed*Time.smoothDeltaTime;
				}

			} else if (isClosing){
				if (Mathf.Abs (movedAmount - offset) <= epsilon){
					isClosing = false;
					p_uiElement.FinishWorking();
					movedAmount = 0f;
				} else {
					transform.Translate(-offset*speed*Time.smoothDeltaTime,0f,0f);
					movedAmount += offset*speed*Time.smoothDeltaTime;
				}
			}
		}
		else if (y){
			if (isOpening){
				if (Mathf.Abs (movedAmount - offset) <= epsilon){
					isOpening = false;
					p_uiElement.FinishWorking();
					movedAmount = 0f;
				} else {
					transform.position = transform.position + new Vector3(0f,offset*speed*Time.smoothDeltaTime,0f);
					movedAmount += offset*speed*Time.smoothDeltaTime;
				}
				
			} else if (isClosing){
				if (Mathf.Abs (movedAmount - offset) <= epsilon){
					isClosing = false;
					p_uiElement.FinishWorking();
					movedAmount = 0f;
				} else {
					transform.Translate(0f,-offset*speed*Time.smoothDeltaTime,0f);
					movedAmount += offset*speed*Time.smoothDeltaTime;
				}
			}
		}
	}

	public void Show(){
		if (!isOpening){ isOpening = true; isClosing = false;}
		p_uiElement.StartWorking();
	}
	public void Hide(){
		if (!isClosing){ isClosing = true; isOpening = false;}
		skeletonAnimation.state.SetAnimation(0,"closeLeftPanel",false);
		p_uiElement.StartWorking();
	}
}
