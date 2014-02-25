using UnityEngine;
using System.Collections;

public class KeepingThings : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this);
	}

}
