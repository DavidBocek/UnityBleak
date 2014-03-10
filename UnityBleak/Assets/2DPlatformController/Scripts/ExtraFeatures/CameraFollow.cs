using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public bool oneDirectionOnly;
	public float minX = -5;
	public float maxX = 5;

	void Update () {
		float delta = target.position.x - transform.position.x;
		if (!oneDirectionOnly || delta > 0.0f) {
			transform.Translate(delta * Time.deltaTime, 0.0f, 0.0f);
		}
		if (transform.position.x < minX) transform.Translate(minX - transform.position.x, 0, 0);
		if (transform.position.x > maxX) transform.Translate(maxX - transform.position.x , 0, 0);
	}
}
