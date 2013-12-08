using UnityEngine;
using System.Collections;

public class ParallaxScrollingElement : MonoBehaviour {

	public float scrollAmountX;
	public float scrollAmountY;

	private Vector3 originalLocation;
	
	void Start(){
		originalLocation = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		Messenger.AddListener<Vector2>("parallaxUpdate", UpdateParallaxMovement);
	}

	void UpdateParallaxMovement(Vector2 cameraPosition){
		Vector3 deltaPosition = new Vector3(cameraPosition.x*scrollAmountX,cameraPosition.y*scrollAmountY,0.0f);
		Vector3 orig = new Vector3(-originalLocation.x,-originalLocation.y,originalLocation.z);
		transform.position = originalLocation + deltaPosition;
	}
}
