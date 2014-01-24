using UnityEngine;
using System.Collections;

public class TimeDestroy : MonoBehaviour {
	public float TimeDelay = 2;
	// Update is called once per frame
	void Update () 
	{
		Destroy(gameObject, TimeDelay);
	}
}
