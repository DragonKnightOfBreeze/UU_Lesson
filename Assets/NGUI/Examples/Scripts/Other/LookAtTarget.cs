using UnityEngine;

/// <summary>
///     Attaching this script to an object will make that object face the specified target. The most ideal use for
///     this script is to attach it to the camera and make the camera look at its target.
/// </summary>
[AddComponentMenu("NGUI/Examples/Look At Target")]
public class LookAtTarget : MonoBehaviour {
	public int level;
	public Transform target;
	public float speed = 8f;

	private Transform mTrans;

	private void Start() {
		mTrans = transform;
	}

	private void LateUpdate() {
		if(target != null) {
			var dir = target.position - mTrans.position;
			var mag = dir.magnitude;

			if(mag > 0.001f) {
				var lookRot = Quaternion.LookRotation(dir);
				mTrans.rotation = Quaternion.Slerp(mTrans.rotation, lookRot, Mathf.Clamp01(speed * Time.deltaTime));
			}
		}
	}
}