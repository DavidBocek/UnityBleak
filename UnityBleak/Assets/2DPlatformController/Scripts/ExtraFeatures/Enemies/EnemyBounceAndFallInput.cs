using UnityEngine;
using System.Collections;

/// <summary>
/// This is the 'AI' for the bounce and fall controller. Basically it bounces off things. It also
/// has a sense distance and the enemy wont activate until within this distance.
/// </summary>
public class EnemyBounceAndFallInput : RaycastCharacterInput {

	public float senseDistance;
	public float direction = -1;
	public float bounceVelocity = 5.0f;
	public float stunTime = 0.5f;

	private EnemyBounceAndFall me;

	private float bounceTimer;	
	private float bounceThreshold = 0.05f;

	void Start() {
		me = GetComponent<EnemyBounceAndFall>();
	}

	void Update() {
		if (!me.controllerActive) {
			if (Mathf.Abs(Camera.main.transform.position.x - transform.position.x) < senseDistance) {
				me.controllerActive = true;
			}
		}
		if (me.controllerActive) {
			if (me.Velocity.x == 0.0f) {
				bounceTimer += RaycastCharacterController.FrameTime;
			} else {
				bounceTimer = 0.0f;
			}
			if (bounceTimer > bounceThreshold) direction *= -1;
		}
		x = direction;
	}
}
