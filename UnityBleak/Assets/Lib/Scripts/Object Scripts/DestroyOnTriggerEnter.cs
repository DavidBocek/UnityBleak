using UnityEngine;
using System.Collections;

public class DestroyOnTriggerEnter : MonoBehaviour {

	private bool OnTheMoney = false;
	public GameObject drops;

	void OnCollision2D(Collision2D other)
	{
		OnTheMoney = true;
	}
	void Update()
	{
		if(OnTheMoney)
		{
			drops.SetActive(false);
		}
	}
}
