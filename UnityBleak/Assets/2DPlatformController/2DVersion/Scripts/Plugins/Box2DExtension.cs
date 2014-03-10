using UnityEngine;
using System.Collections;

/**
 * Extension methods for colliders.
 */ 
public static class Collider2DExtension  {

	public static Vector2 Max(this Collider2D collider)
	{
		if (collider is BoxCollider2D) return (Vector2)collider.gameObject.transform.position + 
			new Vector2(collider.gameObject.transform.localScale.x * (((BoxCollider2D)collider).center.x + (((BoxCollider2D)collider).size.x / 2)),
			            collider.gameObject.transform.localScale.y * (((BoxCollider2D)collider).center.y + (((BoxCollider2D)collider).size.y / 2)));
		Debug.LogError ("Unable to determine max for this collider");
		return Vector2.zero;
	}

	public static Vector2 Min(this Collider2D collider)
	{
		if (collider is BoxCollider2D) return (Vector2)collider.gameObject.transform.position + 
			new Vector2(collider.gameObject.transform.localScale.x * (((BoxCollider2D)collider).center.x - (((BoxCollider2D)collider).size.x / 2)),
			            collider.gameObject.transform.localScale.y * (((BoxCollider2D)collider).center.y - (((BoxCollider2D)collider).size.y / 2)));

		Debug.LogError ("Unable to determine min for this collider");
		return Vector2.zero;
	}
}
