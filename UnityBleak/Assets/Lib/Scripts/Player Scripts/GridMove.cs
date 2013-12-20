using System.Collections;
using UnityEngine;

class GridMove : MonoBehaviour {
	public float moveSpeed = 3f;
	public enum Orientation {
		Horizontal,
		Vertical
	};
	public Orientation gridOrientation = Orientation.Horizontal;
	private bool allowDiagonals = true;
	private bool correctDiagonalSpeed = true;
	private Vector2 input;
	private bool isMoving = false;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float t;
	private float factor;
	
	public void FixedUpdate() {
		if (!isMoving) {
			input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

			if (input != Vector2.zero) {
				StartCoroutine(move(transform));
			}
		}
	}
	
	public IEnumerator move(Transform transform) {
		isMoving = true;
		startPosition = transform.position;
		t = 0;
		/*
		if(gridOrientation == Orientation.Horizontal) {
			endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x),
			                          startPosition.y, startPosition.z + System.Math.Sign(input.y));
		} else {*/
			endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x),
			                          startPosition.y + System.Math.Sign(input.y), startPosition.z);
		//}
		
		if(allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0) {
			factor = 0.7071f;
		} else {
			factor = 1f;
		}

		while (t < 1f) {
			t += Time.deltaTime * (moveSpeed) * factor;
			transform.position = Vector3.Lerp(startPosition, endPosition, t);
			yield return null;
		}
		
		isMoving = false;
		yield return 0;
	}
}