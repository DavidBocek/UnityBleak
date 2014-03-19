using UnityEngine;
using System.Collections;
 
public class Smooth2DCamera : MonoBehaviour {
 
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
 	
	public Vector2 lowerLeft = new Vector2(0.0f,0.0f);
	public Vector2 upperRight;

	void OnLevelWasLoaded(int levelIndex){
		GameObject player = GameObject.FindWithTag("Player");
		if (player != null){
			target = player.transform;
		}
	}

    // Update is called once per frame
    void Update () 
    {
       	if (target)
       	{
        	 Vector3 point = camera.WorldToViewportPoint(target.position);
        	 Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
        	 Vector3 destination = transform.position + delta;
        	 transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
       	}
		Vector3 temp = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		if (transform.position.x < lowerLeft[0]){ temp.x = lowerLeft[0]; }
		else if (transform.position.x > upperRight[0]){ temp.x = upperRight[0]; }
		if (transform.position.y < lowerLeft[1]){ temp.y = lowerLeft[1]; }
		else if (transform.position.y > upperRight[1]){ temp.y = upperRight[1]; }
		transform.position = temp;
		Vector2 cameraPosition = new Vector2(transform.position.x,transform.position.y);
		Messenger.Broadcast<Vector2>("parallaxUpdate",cameraPosition);
	}
}