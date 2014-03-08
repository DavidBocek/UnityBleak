using UnityEngine;
using System.Collections;

public class ChaserScript : MonoBehaviour {
	public Transform Player;
	public float MoveSpeed = 4;
	public float MaxDist = 10;
	public float MinDist = 5;
	public float CrawlDist = 5;
	public float RunSpeed = 2;
	
	
	
	
	void  Start ()
	{
		
	}
	
	void  Update (){
		transform.LookAt(Player);
		
		if(	(Vector3.Distance(transform.position,Player.position) >= MinDist) &&
			(Vector3.Distance(transform.position,Player.position) <= MaxDist))
		{
			if(Vector3.Distance(transform.position,Player.position) >= CrawlDist)
			{
				transform.position += transform.forward*MoveSpeed*Time.deltaTime;
			}
			else
				transform.position += transform.forward*MoveSpeed*Time.deltaTime*RunSpeed;
			
			
			if(Vector3.Distance(transform.position,Player.position) <= MaxDist)
			{

			} 
			
		}
	}
	
}