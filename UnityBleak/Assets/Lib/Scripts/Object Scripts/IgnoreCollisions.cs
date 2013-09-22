using UnityEngine;
using System.Collections;

public class IgnoreCollisions : MonoBehaviour {
	
	public bool ignoreLeft;
	public bool ignoreRight;
	public bool ignoreUp;
	public bool ignoreDown;
	
	public bool HasIgnoreLeft() { return ignoreLeft; }
	public bool HasIgnoreRight() { return ignoreRight; }
	public bool HasIgnoreUp() { return ignoreUp; }
	public bool HasIgnoreDown() { return ignoreDown; }
	
}
