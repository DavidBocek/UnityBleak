using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text.RegularExpressions; 

public class LadderWizardEditorWindow : EditorWindow {

	private RaycastCharacterController controller;
	private string stepSize = "0.0";
	private string stepDistance = "0.0";
	private string ladderTopDistance = "0.0";
	private string totalLength = "1.0";
	private string ladderWidth = "1.0";
	private bool useLadderTop = true;
	private bool generateMesh = true;
	private GameObject meshPrefab;

	private static LadderWizardEditorWindow window;

	[MenuItem ("Assets/2D Platform Controller/Ladder Wizard")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		window = (LadderWizardEditorWindow) EditorWindow.GetWindow (typeof (LadderWizardEditorWindow));
		
		// Listen for updates - use this if you want real time graphs
		//if (EditorApplication.update != window.EditorUpdate) EditorApplication.update += window.EditorUpdate;
		
		// Listen for moves
		//window.wantsMouseMove = true;
		
		// Create a style which loads a background image
		window.Load();
		window.Focus();
	} 

	protected void Load() {
		if (controller == null) controller = (RaycastCharacterController) FindObjectOfType(typeof(RaycastCharacterController));
		DoCalculations();
	}

	private void DoCalculations() {
		if (controller != null && controller.feetColliders != null && controller.feetColliders.Length > 0) {
			float distance = controller.feetColliders[0].distance;
			stepSize = (distance / 2.5f ).ToString ("0.00");
			stepDistance = (distance / ( 5.0f / 3.0f)).ToString ("0.00");
			if (controller.headColliders != null && controller.headColliders.Length > 0) {
				float characterHeightApproximation = 	(controller.headColliders[0].offset.y + controller.headColliders[0].transform.position.y + controller.headColliders[0].distance) -
												 		(controller.feetColliders[0].offset.y + controller.feetColliders[0].transform.position.y - controller.feetColliders[0].distance);
				try {
					ladderTopDistance =  (-1 * (characterHeightApproximation - 0.1f)).ToString ("0.00");
				} catch (System.FormatException){};
			}
		}
	}

	void OnGUI() {
		controller = (RaycastCharacterController) EditorGUILayout.ObjectField(controller, typeof(RaycastCharacterController), true);
		if (GUILayout.Button (new GUIContent("Recalculate Now", "Press to automatically generate values based on the controller and total height. Some tweaking may be needed."))){
			DoCalculations();
		}
		totalLength = NumberLabel("Total Length", totalLength, "Total ladder size in world units.");
		ladderWidth = NumberLabel("Ladder Width", ladderWidth, "Width of the ladder in world units.");
		stepSize = NumberLabel("Step Size", stepSize, "Size of each step.");
		stepDistance = NumberLabel("Step Distance", stepDistance, "Distance between each step.");
		useLadderTop = GUILayout.Toggle(useLadderTop, new GUIContent("Use Ladder Top", "Have a rigid top the user can stand on."));
		if (useLadderTop) ladderTopDistance = NumberLabel("Ladder Top Offset", ladderTopDistance, "The offset from the top of the ladder in which the 'climb to top' animation is triggered.");
		generateMesh = GUILayout.Toggle(generateMesh, new GUIContent("Generate View", "Instantiate a prefab for the ladders view and scale it to the ladder bounds."));
		if (generateMesh) {
			GUILayout.BeginHorizontal();
			GUILayout.Label("View Prefab", GUILayout.Width(100));
			GUILayout.FlexibleSpace();
			GUILayout.Box (new GUIContent("", "Select a prefab to use for the ladder view."), GUILayout.Width(100));
			meshPrefab = (GameObject) EditorGUI.ObjectField(GUILayoutUtility.GetLastRect(), meshPrefab, typeof(GameObject), false);
			GUILayout.EndHorizontal();
		}
		if (GUILayout.Button (new GUIContent("Generate Ladder", "Create the ladder!"))) {
			GenerateLadder();
		}
	}

	private void GenerateLadder () {
		try {
			float ladderTopDistance = float.Parse (this.ladderTopDistance);
			float stepSize = float.Parse (this.stepSize);
			float totalLength = float.Parse (this.totalLength);
			float stepDistance = float.Parse (this.stepDistance);
			float ladderWidth = float.Parse (this.ladderWidth);
	
			// Create parent
			GameObject ladderGo = new GameObject ();
			ladderGo.name = "NewLadder";
			LadderControl control = ladderGo.AddComponent<LadderControl> ();
			
			if (useLadderTop) {
				control.ledgeClimbOffset = ladderTopDistance;
				// Create Top Step
				GameObject topStepGo = new GameObject();
				topStepGo.name = "TopStep";
				topStepGo.layer = controller.climableLayer;
				TopStepPlatform topStep = topStepGo.AddComponent<TopStepPlatform>();
				BoxCollider topStepCollider = topStepGo.AddComponent<BoxCollider>();
				topStep.control = control;
				topStepCollider.size = new Vector3(ladderWidth / 2.0f,  stepSize / 2.0f, 0.5f);
				// topStepCollider.extents = new Vector3(ladderWidth / 2.0f,  stepSize / 2.0f, 0.5f);
				topStep.transform.parent = ladderGo.transform;
				topStep.transform.localPosition = new Vector3(0, -1 * (stepSize / 2.0f), 0);
			} else {
				control.disableLedgeClimb = true;
			}
			
			// Create Steps
	
			int count = (int)((totalLength + (useLadderTop ? ladderTopDistance : 0))/ stepDistance);
			while (count > 0 ) {
				GameObject stepGo = new GameObject();
				stepGo.name = "Step" + count;
				stepGo.layer = controller.climableLayer;
				LadderCollider step = stepGo.AddComponent<LadderCollider>();
				BoxCollider stepCollider = stepGo.AddComponent<BoxCollider>();
				step.control = control;
				stepCollider.size = new Vector3(ladderWidth / 2.0f ,  stepSize / 2.0f, 0.5f);
				// stepCollider.extents = new Vector3(ladderWidth / 2.0f ,  stepSize / 2.0f, 0.5f);
				step.transform.parent = ladderGo.transform;
				step.transform.localPosition = new Vector3(0, (-1 * count * stepDistance) + (stepSize / 2.0f) + (useLadderTop ? ladderTopDistance : 0), 0);
				
				count--;
			} 
	
			// Create Mesh
			if (generateMesh && meshPrefab != null) {
				GameObject ladderMesh = (GameObject) Instantiate(meshPrefab);
				ladderMesh.transform.parent = ladderGo.transform;
				ladderMesh.transform.localPosition = new Vector3(0, -1 * (totalLength / 2.0f), ladderMesh.transform.localPosition.z);
				ladderMesh.transform.localScale = new Vector3(ladderWidth, totalLength, ladderMesh.transform.localScale.z);
			}
		} catch (System.Exception ex) {
			Debug.LogError("Failed to generate ladder... check parameters: " + ex.Message);
		}

	}
	private string NumberLabel(string label, string value, string tooltip) {
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(100));
		GUILayout.FlexibleSpace();
		GUILayout.Box (new GUIContent("",tooltip), GUILayout.Width(100));
		if (Regex.IsMatch(Event.current.character.ToString(), @"[0-9\.]") || Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace) {
			value = GUI.TextField(GUILayoutUtility.GetLastRect(), value);
		} else {
			GUI.TextField(GUILayoutUtility.GetLastRect(), value);
		}
		
		
		GUILayout.EndHorizontal();
		return value;
	} 
}
