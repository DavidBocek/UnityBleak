using UnityEngine;
using System.Collections;

/// <summary>
/// Interface used to determine that an object is an enemy.
/// </summary>
public interface IEnemy {

	void Kill();

	void KillFromBelow(float force);

	void KillFromAbove(HitBox other, Collider me);
}
