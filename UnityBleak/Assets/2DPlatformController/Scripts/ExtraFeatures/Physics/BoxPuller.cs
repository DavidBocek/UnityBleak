using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this to your character to allow for multiple push/pull boxes using PushablePullableBox.cs
/// </summary>
public class BoxPuller : MonoBehaviour {

	/// <summary>
	/// If this is set the player must hold this  key to push or pull.
	/// </summary>
	public KeyCode latchKey;

	private PushablePullableBox currentBox;

	/// <summary>
	/// Returns true if the character can latch on to this box.
	/// </summary>	
	public bool CanLatch(PushablePullableBox newBox) {
		if ((Input.GetKey (latchKey) || latchKey == KeyCode.None) && currentBox == null || currentBox == newBox) {
			currentBox = newBox;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Release (UnLatch) from the current box.
	/// </summary>
	public void ReleaseBox () {
		currentBox = null;
	}
}
