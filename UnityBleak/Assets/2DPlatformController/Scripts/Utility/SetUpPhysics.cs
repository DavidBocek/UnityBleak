using UnityEngine;
using System.Collections;

/// <summary>
/// Set gravity to a higher value as it looks and feel better.
/// Make sure the ingore raycasts layer doesn't collide with any of the character 
/// layers the defaul layer or itself.
/// 
/// *Note* you should set this up in your Physics settings, this script is here mainly to facillitate the examples.
/// </summary>
public class SetUpPhysics : MonoBehaviour {
	
	public float gravity = -12.0f;
	public RaycastCharacterController character;
	public float timeScale = 1.2f;

	public int[] ignoreColliderLayers = new int[]{2};
		
	// Use this for initialization
	void Start () {
		Physics.gravity = new Vector3(0.0f, gravity, 0.0f);
		
		foreach (int i in ignoreColliderLayers) {
			Physics.IgnoreLayerCollision(0, i);
			Physics.IgnoreLayerCollision(2, i);
			Physics.IgnoreLayerCollision(character.backgroundLayer, i);
			Physics.IgnoreLayerCollision(character.passThroughLayer, i);
			Physics.IgnoreLayerCollision(character.climableLayer, i);
		}
		
		Time.timeScale = timeScale;
	}
}
