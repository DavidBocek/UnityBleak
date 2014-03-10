using UnityEngine;
using System.Collections;

/// <summary>
/// This is a simple script for tracking characters health. It provides a Damage
/// method and a
/// </summary>
public class SimpleHealth : MonoBehaviour {

	/// <summary>
	/// The controller.
	/// </summary>
	public RaycastCharacterController controller;

	/// <summary>
	/// The max health.
	/// </summary>
	public int maxHealth = 10;
	
	/// <summary>
	/// What happens when health gets to zero?
	/// </summary>
	public ZeroHealthAction zeroHealthAction;

	/// <summary>
	/// Link to the SimpleHealthBar script that can
	/// be used to show health.
	/// </summary>
	public SimpleHealthBarUI healthBar;

	/// <summary>
	/// The fall damage amount.
	/// </summary>
	public float fallDamageAmount;

	/// <summary>
	/// If true the character will get the same amount of fall damage regardless
	/// of how far they fall.
	/// </summary>
	public bool fallDamageIsConstant;

	/// <summary>
	/// Stun the character after hit for this long. During stun you can't move.
	/// </summary>
	public float stunTime = 1f;

	/// <summary>
	/// How long before we call the Die method.
	/// </summary>
	public float dieDelay = 1.0f;

	/// <summary>
	/// After being hit you are invulnerable for this long before you can take another hit. 
	/// Should usually be larger than stun time.
	/// </summary>
	public float invulnerableTime;

	protected int health;

	protected float invulnerableTimer;

	/// <summary>
	/// Gets the health.
	/// </summary>
	/// <value>The health.</value>
	public int Health {
		get { return health; }
	}

	/// <summary>
	/// Gets the max health.
	/// </summary>
	/// <value>The max health.</value>
	public int MaxHealth{ 
		get { return maxHealth; }
	}

	/// <summary>
	/// Gets the health as apercentage. Note percentage value of 0.0f to 1.0f is used,
	/// so that the percentage can be easily used in calculations.
	/// </summary>
	/// <value>The health as percentage.</value>
	public float HealthAsPercentage { 
		get { return (float)health / (float)maxHealth; }
	}
	
	/// <summary>
	/// Returns true if this character is invulnerable.
	/// </summary>
	public bool IsInvulnerable {
		get { return (invulnerableTimer > 0.0f); }
	}

	/// <summary>
	/// Initialise variables.
	/// </summary>
	void Start () {
		health = maxHealth;
		if (controller == null) controller = gameObject.GetComponent<RaycastCharacterController>();
		if (controller == null) Debug.LogError("SimpleHealth script not attached to a character");
	}

	void Update() {
		if (invulnerableTimer > 0.0f) invulnerableTimer -= RaycastCharacterController.FrameTime;
	}

	/// <summary>
	/// Call this to kill the character.
	/// </summary>
	virtual public void Die () {
		health = 0;
		StartCoroutine(DoZeroHealthAction ());
	}
	
	/// <summary>
	/// Damage the character the specified amount.
	/// </summary>
	/// <param name="amount">Amount of damage to cause.</param>
	virtual public void Damage (int amount) {
		if (invulnerableTimer <= 0.0f) {
			float originalHealthPercentage = HealthAsPercentage;
			health -= Mathf.Abs (amount);
			if (health <= 0) {
					health = 0;
			}
			if (healthBar != null)
					healthBar.AnimateHealthChange (originalHealthPercentage, HealthAsPercentage);
			if (health <= 0) {
					StartCoroutine (DoZeroHealthAction ());
			} else {
					controller.Stun (stunTime);
			}
			invulnerableTimer = invulnerableTime;
		}
	}

	/// <summary>
	/// Call this to calculate and apply fall damage.
	/// </summary>
	/// <param name="amount">Amount of fall damage, ignored if fall damage is constant.</param>
	public void FallDamage (float amount) {
		if (fallDamageIsConstant) {
			Damage ((int)fallDamageAmount);
		} else {
			Damage ((int) (amount * fallDamageAmount));
		}
	}

	/// <summary>
	/// Triggers the death actions.
	/// </summary>
	/// <returns>The zero health action.</returns>
	protected IEnumerator DoZeroHealthAction ()
	{
		if (zeroHealthAction == ZeroHealthAction.SEND_MESSAGE_DIED) {
			SendMessage ("Died", SendMessageOptions.DontRequireReceiver); 
		} else {
			controller.Stun(dieDelay * 1.25f);
			controller.Velocity = new Vector2(0.0f, 10.0f);
			int tmpBackgroundLayer = controller.backgroundLayer;
			int tmpPassThroughLayer = controller.passThroughLayer;
			int tmpClimableLayer = controller.climableLayer;
			controller.backgroundLayer = 999;
			controller.passThroughLayer = 999;
			controller.climableLayer = 999;
			yield return new WaitForSeconds(dieDelay);
			controller.backgroundLayer = tmpBackgroundLayer;
			controller.passThroughLayer = tmpPassThroughLayer;
			controller.climableLayer = tmpClimableLayer;
			controller.Velocity =  Vector2.zero;
			switch (zeroHealthAction) {
				case ZeroHealthAction.RELOAD_SCENE : Application.LoadLevel(Application.loadedLevel); break;
				case ZeroHealthAction.RESPAWN : health = maxHealth; RespawnPoint.Respawn(controller); break;
			}
		}
	}
}

public enum ZeroHealthAction {
	// Reload screen on death
	RELOAD_SCENE,
	// Do a send message with the message "Died" on death
	SEND_MESSAGE_DIED,
	// Trigger the reswpan script on death
	RESPAWN
}