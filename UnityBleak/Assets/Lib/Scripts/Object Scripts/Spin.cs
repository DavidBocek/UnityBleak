using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
	public float speed = 10f;
	
	
	void Update ()
	{
		transform.Rotate(new Vector3(0,0,1 * speed * Time.deltaTime));
	}
}