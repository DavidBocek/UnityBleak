using UnityEngine;
using System.Collections;

public class HealthAndPower2DTK : SimpleHealth {

	public bool IsPowered {
		get; private set;
	}

	public AlienAnimator2DTK animator;
	public Float floater;

	/// <summary>
	/// Damage the character the specified amount.
	/// </summary>
	/// <param name="amount">Amount of damage to cause.</param>
	override public void Damage (int amount) {
		if (invulnerableTimer <= 0.0f) {
			if (IsPowered) {
				IsPowered = false;
				animator.PowerDown();
				floater.enabled = false;
			} else {
				Die ();
			}
			invulnerableTimer = invulnerableTime;
		}
	}

	/// <summary>
	/// Call this to kill the character.
	/// </summary>
	override public void Die () {
		if (IsPowered) animator.PowerDown();
		IsPowered = false;
		floater.enabled = false;
		health = 0;
		StartCoroutine(DoZeroHealthAction ());
	}

	public void PowerUp() {
		IsPowered = true;
		invulnerableTimer = invulnerableTime;
		animator.PowerUp();
		floater.enabled = true;
	}
}
