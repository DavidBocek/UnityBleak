using UnityEngine;

/// <summary>
/// Make the character float gently downwards instead of falling if they hold jump. COuld be changed to
/// use any key (i.e. Input.GetKeyDown() or to not reuqire a key at all (for example as a special ability
/// that is turned on or off by the presence or absence of an item).
/// </summary>
public class Float : MonoBehaviour {

	public RaycastCharacterInput input;
	public RaycastCharacterController controller;
	public float floatFactor = 0.8f;


	void Start () {
		if (input == null) input = gameObject.GetComponent<RaycastCharacterInput>();
		if (input == null) Debug.LogError("Float script does not have an input");
		if (controller == null) controller = gameObject.GetComponent<RaycastCharacterController>();
		if (controller == null) Debug.LogError("Float script not attached to a character");
	}

	void Update () {
		if (input.jumpButtonHeld && controller.State == CharacterState.FALLING) {
			controller.Velocity = new Vector2(controller.Velocity.x, controller.Velocity.y - Physics.gravity.y * floatFactor * Time.deltaTime);
		}
	}
}
