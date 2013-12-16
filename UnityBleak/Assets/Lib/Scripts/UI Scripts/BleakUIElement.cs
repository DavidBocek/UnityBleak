﻿using UnityEngine;
using System.Collections;

public class BleakUIElement : MonoBehaviour {

	public bool hasAnimation;
	public bool hasSubElements = false;
	public int selectionMax;

	private UIBasicAnimation basicAnimator;
	private tk2dSpriteAnimation animation;
	//private bool isOpening = false;
	//private bool isClosing = false;
	private int selectionIndex = 0;

	void Start(){
		if (hasAnimation){
			basicAnimator = GetComponent<UIBasicAnimation>();
		}
	}

	//open the tree under and including this element
	public void OpenTree(){
		SendMessage("Open");
	}

	//close the tree under and including this element
	public void CloseTree(){
		SendMessage("Close");
	}


	//alter the selection index, which is used by the specific selection element on this object
	public void IncrementSelection(){
		selectionIndex = selectionIndex == selectionMax ? selectionIndex : selectionIndex + 1;
	}
	public void DecrementSelection(){
		selectionIndex = selectionIndex == 0 ? selectionIndex : selectionIndex - 1;
	}
	public void ResetSelection(){
		selectionIndex = 0;
	}



	void Open(){
		if (hasAnimation) {basicAnimator.Show();}
	}
	void Close(){
		ResetSelection();
		if (hasAnimation) {basicAnimator.Hide();}
	}

	public void StartWorking(){
		transform.gameObject.SendMessageUpwards("StartWorkingManager");
	}
	public void FinishWorking(){
		transform.gameObject.SendMessageUpwards("FinishWorkingManager");
	}
}
