using UnityEngine;
using System.Collections;

public class KillZones : MonoBehaviour {
	
	public bool killAny;
	public bool killUp;
	public bool killLeft;
	public bool killRight;
	public bool killDown;
	
	public bool HasKillUp(){
		return (killUp || killAny);
	}
	
	public bool HasKillLeft(){
		return (killLeft || killAny);
	}
	
	public bool HasKillRight(){
		return (killRight || killAny);
	}
	
	public bool HasKillDown(){
		return (killDown || killAny);
	}
}
