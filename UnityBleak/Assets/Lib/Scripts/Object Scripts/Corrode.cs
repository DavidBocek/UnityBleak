using UnityEngine;
using System.Collections;

public class Corrode : MonoBehaviour {

	public float activateRadius;
	public AudioClip corrosionSound;

	// Use this for initialization
	void Start () {
		Messenger.AddListener<Vector3>("GrimmVialUsed",HandleGrimmVialUsed);
	}
	
	void HandleGrimmVialUsed(Vector3 useLocation){
		if ((transform.position - useLocation).magnitude <= activateRadius){
			if (corrosionSound != null)
				AudioSource.PlayClipAtPoint(corrosionSound,transform.position,1f);
			Destroy(gameObject,1.2f);
			GetComponent<tk2dSpriteAnimator>().Play("Dissolve");
		}
	}
}
