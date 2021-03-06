﻿using UnityEngine;
using System.Collections;

public class BleakUIElement : MonoBehaviour {

	public bool hasAnimation;
	public bool hasSubElements = false;
	public int selectionMax;
	public bool isMapOrKey;
	public SkeletonAnimation mapKeySkelAnim;
	
	private bool isOpen = false;
	private SkeletonAnimation skelAnim;
	private UIBasicAnimation basicAnimator;
	private tk2dSpriteAnimation animation;
	private int selectionIndex = 0;

	void Start(){
		if (hasAnimation){
			basicAnimator = GetComponent<UIBasicAnimation>();
			skelAnim = GetComponentInChildren<SkeletonAnimation>();
		}
	}

	//open the tree under and including this element
	public void OpenTree(){
		if (!isOpen)
			SendMessage("Open");
	}

	//close the tree under and including this element
	public void CloseTree(){
		if (isOpen)
			SendMessage("Close");
	}

	//sends a select message under and including this element
	public void UISelect(string type){
		if (type == "inventory"){
			GetComponent<UIInventoryDisplay>().UISelect(selectionIndex);
		}
	}

	public void MapToKey(){
		StartCoroutine("cSetWorkingForTime",.75f);
		mapKeySkelAnim.state.SetAnimation(0,"switchTab2",false);
	}

	public void KeyToMap(){
		StartCoroutine("cSetWorkingForTime",.75f);
		mapKeySkelAnim.state.SetAnimation(0,"switchTab",false);
	}


	//alter the selection index, which is used by the specific selection element on this object
	public void IncrementSelection(){
		if (hasAnimation && selectionIndex != selectionMax){
			skelAnim.state.AddAnimation(0,"0"+(selectionIndex+1)+"-0"+(selectionIndex+2),false,0);
		}
		selectionIndex = selectionIndex == selectionMax ? selectionIndex : selectionIndex + 1;
	}
	public void DecrementSelection(){
		if (hasAnimation && selectionIndex != 0){
			skelAnim.state.AddAnimation(0,"0"+(selectionIndex+1)+"-0"+(selectionIndex),false,0);
		}
		selectionIndex = selectionIndex == 0 ? selectionIndex : selectionIndex - 1;
	}
	public void ResetSelection(){
		selectionIndex = 0;
	}

	void Open(){
		isOpen = true;
		if (hasAnimation) {
			StartCoroutine("cSetWorkingForTime",1.25f);
			basicAnimator.Show();
		}
	}
	void Close(){
		isOpen = false;
		ResetSelection();
		if (hasAnimation) {
			StartCoroutine("cSetWorkingForTime",1.25f);
			basicAnimator.Hide();
		}
	}

	public void StartWorking(){
		transform.gameObject.SendMessageUpwards("StartWorkingManager");
	}
	public void FinishWorking(){
		transform.gameObject.SendMessageUpwards("FinishWorkingManager");
	}



	
	IEnumerator cSetWorkingForTime(float time){
		StartWorking();
		yield return new WaitForSeconds(time);
		FinishWorking();
	}
}
