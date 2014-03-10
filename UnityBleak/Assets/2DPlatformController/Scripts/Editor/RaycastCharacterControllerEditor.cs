using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor (typeof(RaycastCharacterController))]
public class RaycastCharacterControllerEditor : Editor
{
	public const float SNAP = 0.05f;
	public bool editSides = true;
	public bool editFeet = true;
	public bool editHead = true;
	public bool showEditorOptions = false;
	
	override public void OnInspectorGUI () {
		showEditorOptions = EditorGUILayout.Foldout(showEditorOptions, "Collidor Editor Options");
		if (showEditorOptions) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(editSides ? "Sides: On" : "Sides: Off")){
				editSides = !editSides;	
				EditorUtility.SetDirty(target);
			}
			if (GUILayout.Button(editFeet ? "Feet: On" : "Feet: Off")){
				editFeet = !editFeet;	
				EditorUtility.SetDirty(target);
			}
			if (GUILayout.Button(editHead ? "Head: On" : "Head: Off")){
				editHead = !editHead;	
				EditorUtility.SetDirty(target);
			}
			EditorGUILayout.EndHorizontal();
			if (GUILayout.Button("Align Feet")){
				float distance = 0;
				float y = 0;
				foreach (RaycastCollider collider in ((RaycastCharacterController)target).feetColliders) {
					distance += collider.distance;
					y += collider.offset.y;
				}
				distance /= ((RaycastCharacterController)target).feetColliders.Length;
				y /= ((RaycastCharacterController)target).feetColliders.Length;
				foreach (RaycastCollider collider in ((RaycastCharacterController)target).feetColliders) {
					collider.distance = distance;
					collider.offset.y = y;
				}
			}
		}
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
		DrawDefaultInspector();
	}
	
	void OnSceneGUI () {
		Vector3 targetPosition = ((RaycastCharacterController)target).gameObject.transform.position;
		if (editSides) {
			foreach (RaycastCollider collider in ((RaycastCharacterController)target).sides) {
				Handles.color = (collider.direction == RC_Direction.LEFT ? Color.yellow : Color.red);
				collider.offset = Handles.FreeMoveHandle(collider.offset + targetPosition, 
					Quaternion.identity, 
					HandleUtility.GetHandleSize(collider.offset) * 0.25f, 
					Vector3.one * SNAP, 
					Handles.CubeCap) - targetPosition;
				
				collider.distance = (Handles.FreeMoveHandle(collider.offset + targetPosition + collider.GetVectorForDirection() * collider.distance, 
					Quaternion.identity, 
					HandleUtility.GetHandleSize(collider.offset) * 0.25f, 
					Vector3.one * SNAP, 
					Handles.CircleCap) - collider.offset - targetPosition).x * (collider.direction == RC_Direction.LEFT ? -1 : 1);	
			}
		}
		if (editFeet) {
			if (((RaycastCharacterController)target).feetColliders.Length == 0) {
				((RaycastCharacterController)target).feetColliders = new RaycastCollider[1];
				((RaycastCharacterController)target).feetColliders[0] = new RaycastCollider();
				((RaycastCharacterController)target).feetColliders[0].transform = ((RaycastCharacterController)target).transform;
				((RaycastCharacterController)target).feetColliders[0].distance = 1;
				((RaycastCharacterController)target).feetColliders[0].direction = RC_Direction.DOWN;
			}
			Handles.color = Color.green;
			float y = (Handles.FreeMoveHandle(((RaycastCharacterController)target).feetColliders[0].offset + targetPosition, 
			                                         Quaternion.identity, 
			                                  		HandleUtility.GetHandleSize(((RaycastCharacterController)target).feetColliders[0].offset) * 0.25f, 
			                                         Vector3.one * SNAP, 
			                                         Handles.CubeCap) - targetPosition).y;
			float distance = (Handles.FreeMoveHandle(((RaycastCharacterController)target).feetColliders[0].offset + targetPosition + ((RaycastCharacterController)target).feetColliders[0].GetVectorForDirection() * ((RaycastCharacterController)target).feetColliders[0].distance, 
			                                            Quaternion.identity, 
			                                         HandleUtility.GetHandleSize(((RaycastCharacterController)target).feetColliders[0].offset) * 0.25f, 
			                                            Vector3.one * SNAP, 
			                                         Handles.CircleCap) - ((RaycastCharacterController)target).feetColliders[0].offset - targetPosition).y * -1;	
			
			foreach (RaycastCollider collider in ((RaycastCharacterController)target).feetColliders) {
				collider.offset = new Vector3(collider.offset.x, y, collider.offset.z);
				collider.distance = distance;
			}
		}
		if (editHead) {
			Handles.color = Color.green;
			foreach (RaycastCollider collider in ((RaycastCharacterController)target).headColliders) {
				collider.offset = Handles.FreeMoveHandle(collider.offset + targetPosition, 
					Quaternion.identity, 
					HandleUtility.GetHandleSize(collider.offset) * 0.25f, 
					Vector3.one * SNAP, 
					Handles.CubeCap) - targetPosition;
				
				collider.distance = (Handles.FreeMoveHandle(collider.offset + targetPosition + collider.GetVectorForDirection() * collider.distance, 
					Quaternion.identity, 
					HandleUtility.GetHandleSize(collider.offset) * 0.25f, 
					Vector3.one * SNAP, 
					Handles.CircleCap) - collider.offset - targetPosition).y;	
			}
		}
    }
}

