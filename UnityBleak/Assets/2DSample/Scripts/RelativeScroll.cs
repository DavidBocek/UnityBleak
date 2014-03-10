using UnityEngine;
using System.Collections;

public class RelativeScroll : MonoBehaviour {

	public Transform cameraTransform;
	public float relativeMovement = 0.5f;
	
	private float lastPosition;

	void Start() {
		lastPosition = cameraTransform.position.x;
	}
	
	void Update () {
		transform.Translate((lastPosition - cameraTransform.position.x) * relativeMovement, 0.0f, 0.0f);
		lastPosition = cameraTransform.position.x;
	}
}
