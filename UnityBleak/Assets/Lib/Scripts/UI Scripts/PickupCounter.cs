using UnityEngine;
using System.Collections;

public class PickupCounter : MonoBehaviour {

	public bool isScrapMetal;
	public bool isScrew;
	public bool isGear;

	public int minValue;
	public int maxValue;

	public PickupCounterAnimator onesCounter;
	public PickupCounterAnimator tensCounter;
	public PickupCounterAnimator hundredsCounter;

	private int count;

	// Use this for initialization
	void Awake () {
		//load counter values from the last scene
		/*if (isScrapMetal)
			count = LevelLoaderUtil.scrapCount;
		if (isScrew)
			count = LevelLoaderUtil.screwCount;
		if (isGear)
			count = LevelLoaderUtil.gearCount;*/
	}

	void Start(){
		UpdateAnimators();
	}
	
	public void Add(int value){
		if (count < maxValue){
			count += value;
			UpdateAnimators();
		}
	}

	public void Subtract(int value){
		if (count > minValue){
			count -= value;
			UpdateAnimators();
		}
	}

	public void SetCount(int value){
		if (value >= minValue && value <= maxValue){
			count = value;
			UpdateAnimators();
		} else {
			throw new UnityException("attempted to set a pickup counter to greater than its max vaue or less than its min value");
		}
	}

	public int GetCount(){
		return count;
	}


	/// <summary>
	/// Updates the animators for the counter to display the correct number on the UI.
	/// </summary>
	void UpdateAnimators(){
		int countCopy = count;
		onesCounter.ChangeCounterToValue(countCopy % 10);
		countCopy /= 10;
		tensCounter.ChangeCounterToValue(countCopy % 10);
		countCopy /= 10;
		hundredsCounter.ChangeCounterToValue(countCopy % 10);
	}
}
