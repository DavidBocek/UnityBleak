using UnityEngine;
using System.Collections;

/// <summary>
/// A very simple health bar UI. You could extend to create your
/// own health bar if you wnt to easily integrate with the other
/// sample scripts.
/// </summary>
public class SimpleHealthBarUI : MonoBehaviour {

	/// <summary>
	/// The visible contents. Enable/disable this to show
	/// the health bar.
	/// </summary>
	public GameObject visibleContents;

	/// <summary>
	/// Transform of the mesh representing health.
	/// </summary>
	public Transform healthBar;

	/// <summary>
	/// How fast does damage show.
	/// </summary>
	public float animationSpeed;	

	/// <summary>
	/// How long do we show the health bar for after taking a hit.
	/// </summary>
	public float visibleDelay;

	void Awake() {
#if UNITY_3_0 || UNITY_3_1	|| UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		visibleContents.SetActiveRecursively(false);
#else
		visibleContents.SetActive(false);
#endif
	}
	virtual public void AnimateHealthChange(float from, float to) {
		StopAllCoroutines();
		StartCoroutine(DoAnimateHealthChange(from, to));
	}

	private IEnumerator DoAnimateHealthChange(float from, float to) {
		healthBar.localScale = new Vector3(from, 1.0f, 1.0f);
		healthBar.transform.localPosition = new Vector3((from / 2.0f) - 0.5f, 0.0f, healthBar.transform.localPosition.z );
#if UNITY_3_0 || UNITY_3_1	|| UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		visibleContents.SetActiveRecursively(false);
#else
		visibleContents.SetActive(false);
#endif
		float t = 0;
		while (healthBar.localScale.x > to) {
			t += Time.deltaTime;
			float lerp = Mathf.Lerp(from , to, animationSpeed * t);
			healthBar.localScale = new Vector3(lerp , 1.0f, 1.0f);
			healthBar.transform.localPosition = new Vector3((lerp / 2.0f) - 0.5f, 0.0f, healthBar.transform.localPosition.z);
			yield return true;
		}
		healthBar.localScale = new Vector3(to, 1.0f, 1.0f);
		healthBar.transform.localPosition = new Vector3((to / 2.0f) - 0.5f, 0.0f, healthBar.transform.localPosition.z);
		yield return new WaitForSeconds(visibleDelay);
#if UNITY_3_0 || UNITY_3_1	|| UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		visibleContents.SetActiveRecursively(false);
#else
		visibleContents.SetActive(false);
#endif
	}
}
