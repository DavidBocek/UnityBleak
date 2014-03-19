using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public string levelEnterPointTag;
	public int numLivesPlayer = 2;
	public int scrapCount;
	public int screwCount;
	public int gearCount;


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}

	public void SaveGameVars(){
		PlayerPrefs.SetInt("numLives",numLivesPlayer);
		PlayerPrefs.SetInt("scrapCount",scrapCount);
		PlayerPrefs.SetInt("screwCount",screwCount);
		PlayerPrefs.SetInt("gearCount",gearCount);
		PlayerPrefs.SetString("levelEnterPointTag",levelEnterPointTag);
		PlayerPrefs.Save();
	}

	public void LoadGameVars(){
		//empty for now since PAX demo wont have saving
	}

}
