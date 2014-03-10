using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A touch input for the 2D Platform controller
/// </summary>
public class TouchController: RaycastCharacterInput {

	/// <summary>
	/// The buttons this controller manages.
	/// </summary>
	public List<TouchControllerButton> buttons;

	/// <summary>
	/// If true then the controller will try to predict
	/// what the user meant to do instead of relying on the
	/// user to clearly press each button.
	/// </summary>
	public bool predictiveMode;

	/// <summary>
	/// Assign the camera used to render the buttons here.
	/// </summary>
	public Camera uiCamera;

	/// <summary>
	/// Assign the layer that the buttons ar eon, it should be different to the
	/// controllers layers so the button don't effect the character.
	/// </summary>
	public int buttonLayer = -1;

	/// <summary>
	/// The amount to set the x input to when a button is pressed. Typically use
	/// 0.5f for walk and 1.0f for run.
	/// </summary>
	public float xButtonValue = 1.0f;
	
	/// <summary>
	/// The amount to set the y input to when a button is pressed. Typically use 1.0f;
	/// </summary>
	public float yButtonValue = 1.0f;

	/// <summary>
	/// If true the run button will double the button value for left and right buttons.
	/// </summary>
	public bool allowRun;
	
	private bool running;

	private Dictionary<int, TouchControllerButton> fingersToButtons;

	/// <summary>
	/// Unity awake hook. 
	/// </summary>
	void Awake() {
		// Add all children that are buttons
		Object[] buttonObjects = gameObject.GetComponentsInChildren (typeof(TouchControllerButton));
		foreach (Object o in buttonObjects) {
			if (!buttons.Contains((TouchControllerButton)o)) {
				buttons.Add((TouchControllerButton)o);
			}
		}
		// If no camera is assigned main camera will be assigned
		if (uiCamera == null) uiCamera = Camera.main;

		// If no layer is assigned, layer of first button will be assigned
		if (buttonLayer == -1 && buttons.Count > 0) buttonLayer = buttons [0].gameObject.layer;

		fingersToButtons = new Dictionary<int, TouchControllerButton > ();
	}

	/// <summary>
	/// Unity update hook, the main control loop.
	/// </summary>
	void Update() {
		running = false;
		x = 0.0f; y = 0.0f;
		jumpButtonHeld = false;
		jumpButtonDown = false;

		DoStandard ();

		if (allowRun && running) {
			x *= 2.0f;
		}
	}

	/// <summary>
	/// Handle button touches in standard mode.
	/// </summary>
	private void DoStandard() {
		for (int i = 0; i < Input.touchCount; i++) {
			// Find all collisions
			if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Stationary || Input.touches[i].phase == TouchPhase.Moved) {
				RaycastHit[] hits;
				Ray ray = uiCamera.ScreenPointToRay(Input.touches[i].position);
				hits = Physics.RaycastAll(ray, 100.0f, 1 << buttonLayer);
				foreach (RaycastHit hit in hits) {
					TouchControllerButton button = hit.collider.gameObject.GetComponent<TouchControllerButton>();
					if (button != null) {
						DoButton(button, Input.touches[i]);
					}
				}
			} else { // Released button
				if (fingersToButtons.ContainsKey(Input.touches[i].fingerId)){
					fingersToButtons[Input.touches[i].fingerId].Released();
					fingersToButtons.Remove(Input.touches[i].fingerId);    
				}
			}
		}
	}

	/// <summary>
	/// Do the actual button action
	/// </summary>
	private void DoButton(TouchControllerButton button, Touch touch) {
		AssignButtonToFinger (touch.fingerId, button);
		button.Held();
		switch (button.type) {
			case TouchControllerButtonType.LEFT:
				x = -1 * xButtonValue;
				break;
			case TouchControllerButtonType.RIGHT:
				x = xButtonValue;
				break;
			case TouchControllerButtonType.UP:
				y = yButtonValue;
				break;
			case TouchControllerButtonType.DOWN:
				y = -1 * yButtonValue;
				break;
			case TouchControllerButtonType.JUMP:
				if (touch.phase == TouchPhase.Began) {
					jumpButtonDown = true;
				}
				jumpButtonHeld = true;
				break;
			case TouchControllerButtonType.RUN:
				if (allowRun) running = true;
				break;
		}
	}

	/// <summary>
	/// Release button if user moved finger away from it. Assign new button to given fingerId.
	/// </summary>
	private void AssignButtonToFinger(int fingerId, TouchControllerButton button) {
		if (fingersToButtons.ContainsKey (fingerId)) {
			fingersToButtons[fingerId].Released();
			fingersToButtons.Remove(fingerId);
		}
		fingersToButtons.Add (fingerId, button);
	}
}
