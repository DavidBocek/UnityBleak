using UnityEngine;
using System.Collections;

public class RotateArm : MonoBehaviour 
{
	// Interpolates rotation between the rotations
	// of from and to.
	// (Choose from and to not to be the same as 
	// the object you attach this script to)
	public Transform from;
	public Transform to;
	public int speed = 1;
	void  Update ()
	{
		transform.rotation = Quaternion.Lerp (from.rotation, to.rotation, Time.time * speed);
	}
}