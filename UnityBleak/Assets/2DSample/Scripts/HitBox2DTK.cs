using UnityEngine;
using System.Collections;

/// <summary>
/// Overrides default HitBox to allow for special collactable behaviours.
/// </summary>
public class HitBox2DTK : HitBox {
	
	override public void Collect (Collectable collectable) {
		if (collectable is Mushroom2DTK && simplehealth is HealthAndPower2DTK) ((HealthAndPower2DTK)simplehealth).PowerUp();
	}
}
