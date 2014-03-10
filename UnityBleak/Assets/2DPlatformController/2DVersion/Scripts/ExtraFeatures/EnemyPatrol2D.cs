using UnityEngine;
using System.Collections;

public class EnemyPatrol2D : MonoBehaviour, IEnemy {

	public float maxX;
	public float minX;
	public float speed;
	public bool bobble;
	public float stunTime;
	public string enemyName;
	public float bounceVelocity;

	private float velocity;
	private bool hasHitPlayer;
	private bool isDead;

	// Use this for initialization
	void Start () {
		velocity = speed;	
		if (bobble) StartCoroutine (Bobble());
	}
	
	// Update is called once per frame
	void Update () {
		if (!isDead) {
			transform.Translate(velocity * Time.deltaTime, 0.0f, 0.0f);
			if ((transform.position.x < minX && velocity < 0)|| (transform.position.x > maxX && velocity > 0)) {
				velocity *= -1;
				if (velocity > 0) transform.localScale = new Vector3(-1, 1, 1);
				else if (velocity < 0) transform.localScale = new Vector3(1, 1, 1);
			}
		} else {
			transform.Translate(0.0f, velocity * Time.deltaTime , 0.0f);
			velocity += Physics.gravity.y * Time.deltaTime;
		}
	}

	
	public IEnumerator Bobble() {
		while (true) {
			iTween.PunchPosition(gameObject, new Vector3(0.0f, 0.5f, 0.0f), 1.0f);
			yield return new WaitForSeconds(1.05f);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		HitBox2D health = other.gameObject.GetComponent<HitBox2D>();
		if (health != null && !hasHitPlayer) {
			hasHitPlayer = true;
			health.Damage(1);
			if (other.transform.position.x > transform.position.x) 	velocity = speed * -1;
			else if (other.transform.position.x < transform.position.x) velocity = speed;
		}
	}

	public void Kill() {
		hasHitPlayer = true;
		isDead = true;
		StartCoroutine(DoDie ());
	}
	
	private IEnumerator DoDie() {
		velocity = 0.0f;
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer != null) spriteRenderer.enabled = false;
		if (particleSystem != null) particleSystem.Play ();
		yield return new WaitForSeconds(stunTime);
		Destroy(gameObject);
	}
	

	public void KillFromAbove(HitBox other, Collider me) {
		if (!hasHitPlayer && other != null && other.simplehealth != null) {
			// If we can find a character controller 
			RaycastCharacterController2D hero = other.simplehealth.GetComponent<RaycastCharacterController2D>();
			if (hero != null) {
				me.collider.enabled = false;
				Kill();
				hero.Velocity = new Vector2(hero.Velocity.x, bounceVelocity);
			}
		}
	}	

	public void KillFromBelow(float force) {
		if (!hasHitPlayer) {
			Kill();
		}
	}

}
