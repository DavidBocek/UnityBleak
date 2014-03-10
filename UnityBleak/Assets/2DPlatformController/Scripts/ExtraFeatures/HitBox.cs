using UnityEngine;
using System.Collections;

/// <summary>
/// A hit box takes damage and passes it to a health component for processing.
/// This allows you to easily move the damage points or alternatively attach
/// them to transforms.
///
///	A hit box can also be used to collect items. Although at the moment the collect method
/// doesn't do anything. You could sue this to grant powers, increment counters, etc.
/// </summary>
public class HitBox : MonoBehaviour {

	public SimpleHealth simplehealth;

	public virtual void Damage(int amount) {
		simplehealth.Damage(amount);
	}

	public virtual void Collect (Collectable collectable) {

	}
}
