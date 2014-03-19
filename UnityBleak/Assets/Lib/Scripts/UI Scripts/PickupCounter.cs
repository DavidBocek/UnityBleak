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

	public int count;

	private LevelManager levelManager;

	// Use this for initialization
	void Start () {
		levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
		if (levelManager != null){
			//load counter values from the last scene
			if (isScrapMetal){
				count = levelManager.scrapCount;
			}
			if (isScrew){
				count = levelManager.screwCount;
			}
			if (isGear){
				count = levelManager.gearCount;
			}
		}

		UpdateAnimators();
	}
	
	public void Add(int value){
		if (count < maxValue){
			count += value;
			UpdateAnimators();
		}
		UpdateLevelManager();
	}

	public void Subtract(int value){
		if (count > minValue){
			count -= value;
			UpdateAnimators();
		}
		UpdateLevelManager();
	}

	public void SetCount(int value){
		if (value >= minValue && value <= maxValue){
			count = value;
			UpdateAnimators();
		} else {
			throw new UnityException("attempted to set a pickup counter to greater than its max vaue or less than its min value");
		}
		UpdateLevelManager();
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

	/// <summary>
	/// Updates the level manager counts for saving
	/// </summary>
	void UpdateLevelManager(){
		if (isScrapMetal)
			levelManager.scrapCount = count;
		if (isScrew)
			levelManager.screwCount = count;
		if (isGear)
			levelManager.gearCount = count;
	}
}
