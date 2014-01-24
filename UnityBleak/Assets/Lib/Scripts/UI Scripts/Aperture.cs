using UnityEngine;
using System.Collections;

public class Aperture : MonoBehaviour {

	private SkeletonAnimation skelAnim;
	//public AudioListener cammie;
	// Use this for initialization
	void Start () {
		skelAnim = GetComponentInChildren<SkeletonAnimation>();
	}
	
	public void Open(){
		skelAnim.state.SetAnimation(0,"open",false);
		//cammie.enabled = true;
	}

	public void Close(){
		skelAnim.state.SetAnimation(0,"close",false);
		//cammie.enabled = false;
	}
}
