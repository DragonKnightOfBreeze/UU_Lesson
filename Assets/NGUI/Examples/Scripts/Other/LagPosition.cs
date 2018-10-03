using UnityEngine;

/// <summary>Attach to a game object to make its position always lag behind its parent as the parent moves.</summary>
public class LagPosition : MonoBehaviour {
	public Vector3 speed = new Vector3(10f, 10f, 10f);
	public bool ignoreTimeScale;

	private Transform mTrans;
	private Vector3 mRelative;
	private Vector3 mAbsolute;
	private bool mStarted;

	public void OnRepositionEnd() {
		Interpolate(1000f);
	}

	private void Interpolate(float delta) {
		var parent = mTrans.parent;

		if(parent != null) {
			var target = parent.position + parent.rotation * mRelative;
			mAbsolute.x = Mathf.Lerp(mAbsolute.x, target.x, Mathf.Clamp01(delta * speed.x));
			mAbsolute.y = Mathf.Lerp(mAbsolute.y, target.y, Mathf.Clamp01(delta * speed.y));
			mAbsolute.z = Mathf.Lerp(mAbsolute.z, target.z, Mathf.Clamp01(delta * speed.z));
			mTrans.position = mAbsolute;
		}
	}

	private void Awake() {
		mTrans = transform;
	}

	private void OnEnable() {
		if(mStarted) ResetPosition();
	}

	private void Start() {
		mStarted = true;
		ResetPosition();
	}

	public void ResetPosition() {
		mAbsolute = mTrans.position;
		mRelative = mTrans.localPosition;
	}

	private void Update() {
		Interpolate(ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime);
	}
}