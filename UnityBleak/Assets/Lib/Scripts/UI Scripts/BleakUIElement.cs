using UnityEngine;
using System.Collections;

public class BleakUIElement : MonoBehaviour {

	public bool hasAnimation;
	public int selectionMax;

	private tk2dSpriteAnimation animation;
	private bool isOpening;
	private bool isClosing;
	private int selectionIndex = 0;

	void Start(){
		if (hasAnimation){
			animation = GetComponent<tk2dSpriteAnimation>();
		}
	}
	
	void Update () {
		if (isOpening){
			//move object to open spot
			SendMessage("OpenSubAnimation");
			if (hasAnimation){
				//play opening animation
			}
		} else if (isClosing){
			//move object to close spot
			SendMessage("CloseSubAnimation");
			if (hasAnimation){
				//play closing animation
			}
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
		if (!isOpening){ isOpening = true;}
	}
	void Close(){
		if(!isClosing){ isClosing = true;}
		ResetSelection();
	}
}
