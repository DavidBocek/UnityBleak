using UnityEngine;
using System.Collections;

public class ResetSpriteRotation : MonoBehaviour {

	
	// Update is called once per frame
	void LateUpdate () {
		Debug.Log (transform.parent.localRotation);
		transform.rotation = Quaternion.Euler(transform.localRotation.x,transform.localRotation.y, 0);
	}
}
