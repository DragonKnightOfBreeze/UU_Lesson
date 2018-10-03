using UnityEngine;

/// <summary>Attach to a game object to make its rotation always lag behind its parent as the parent rotates.</summary>
[AddComponentMenu("NGUI/Examples/Lag Rotation")]
public class LagRotation : MonoBehaviour {
	public float speed = 10f;
	public bool ignoreTimeScale;

	private Transform mTrans;
	private Quaternion mRelative;
	private Quaternion mAbsolute;

	public void OnRepositionEnd() {
		Interpolate(1000f);
	}

	private void Interpolate(float delta) {
		if(mTrans != null) {
			var parent = mTrans.parent;

			if(parent != null) {
				mAbsolute = Quaternion.Slerp(mAbsolute, parent.rotation * mRelative, delta * speed);
				mTrans.rotation = mAbsolute;
			}
		}
	}

	private void Start() {
		mTrans = transform;
		mRelative = mTrans.localRotation;
		mAbsolute = mTrans.rotation;
	}

	private void Update() {
		Interpolate(ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime);
	}
}