using UnityEngine;
using System.Collections;

public class EnterNextLevel : MonoBehaviour {

	public string nextLevelEnterPointTag;
	public string nextLevel;
	
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag("Player")){
			LevelManager lm = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
			lm.levelEnterPointTag = nextLevelEnterPointTag;
			lm.SaveGameVars();
			Application.LoadLevel(nextLevel);
		}
	}

}
