using UnityEngine;
using System.Collections;

/// <summary>
/// A simple trigger that sets the cahracters state to swimming on enter and not swimmign on exit.
/// </summary>
public class SwimTrigger : MonoBehaviour {

	void OnTriggerEnter (Collider other)
	{
		RaycastCharacterController controller = other.transform.parent.gameObject.GetComponent<RaycastCharacterController> ();
		if (controller != null) {
			controller.IsSwimming = true;
		}
	}

	
	void OnTriggerExit (Collider other)
	{
		RaycastCharacterController controller = other.transform.parent.gameObject.GetComponent<RaycastCharacterController> ();
		if (controller != null) {
			controller.IsSwimming = false;
		}
	}

}
