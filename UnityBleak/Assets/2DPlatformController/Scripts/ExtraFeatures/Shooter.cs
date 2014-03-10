using UnityEngine;
using System.Collections;

/// <summary>
/// Extremely simple "gun". Purely here to demonstrate the hit box/damge features.
/// </summary>
public class Shooter : MonoBehaviour {

	/// <summary>
	/// Prefab of the "bullet"
	/// </summary>
	public GameObject projectilePrefab;

	// Use this for initialization
	void Start () {
		InvokeRepeating("LaunchProjectile", 3, 3);
	}
	
	// Update is called once per frame
	private void LaunchProjectile () {
		GameObject.Instantiate(projectilePrefab, transform.position, Quaternion.identity);	
	}
}
