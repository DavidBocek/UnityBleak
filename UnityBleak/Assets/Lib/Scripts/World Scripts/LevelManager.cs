using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public string levelEnterLocation;
	public int numLivesPlayer = 2;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}

}
