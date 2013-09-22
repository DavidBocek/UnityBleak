using UnityEngine;
using System.Collections;

public class BounceOffTop : MonoBehaviour {
	
	public int springPower;
	private bool isSprung;
	
	public bool IsSprung(){
		return isSprung;
	}
	
	public void SetSprung(bool b){
		isSprung = b;
	}
}
