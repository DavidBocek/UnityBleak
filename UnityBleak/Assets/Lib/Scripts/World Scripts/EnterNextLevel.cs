using UnityEngine;
using System.Collections;

public class EnterNextLevel : MonoBehaviour {

	public string nextLevelEnterPointTag;
	public string nextLevel;
	
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag("Player")){
			GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().levelEnterLocation = nextLevelEnterPointTag;
			Application.LoadLevel(nextLevel);
		}
	}

}
