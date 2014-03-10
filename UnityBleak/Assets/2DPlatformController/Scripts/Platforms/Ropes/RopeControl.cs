using UnityEngine;
using System.Collections;

public class RopeControl : MonoBehaviour {
	
	public float ropeVelocityFactor = 1.33f;
	public float jumpFlattenFactor = 3;
	public bool canClimb = true;
	public float swingTime = 5.0f;
	
	public float _lastSwingDirection;
	public bool hasClimbed;
	public bool hasSwung;
	
	private float swingTimer;
	
	void Start() {
		hasClimbed = !canClimb;	
	}
	
	void Update() {
		if (canClimb) hasClimbed = false;
		hasSwung = false;
		if (swingTimer > 0.0f) {
			swingTimer -= Time.deltaTime;
			if (swingTimer <= 0.0f) _lastSwingDirection = 0.0f;
		}
	}
	
	public float LastSwingDirection {
		get {return _lastSwingDirection;}
		set {
			_lastSwingDirection = value;
			swingTimer = swingTime;
		}
	}
}
