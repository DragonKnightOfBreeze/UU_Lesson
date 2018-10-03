using UnityEngine;

/// <summary>
///     Attach this script to a child of a draggable window to make it tilt as it's dragged. Look at how it's used in
///     Example 6.
/// </summary>
[AddComponentMenu("NGUI/Examples/Window Drag Tilt")]
public class WindowDragTilt : MonoBehaviour {
	public int updateOrder;
	public float degrees = 30f;

	private Vector3 mLastPos;
	private Transform mTrans;
	private float mAngle;

	private void OnEnable() {
		mTrans = transform;
		mLastPos = mTrans.position;
	}

	private void Update() {
		var deltaPos = mTrans.position - mLastPos;
		mLastPos = mTrans.position;

		mAngle += deltaPos.x * degrees;
		mAngle = NGUIMath.SpringLerp(mAngle, 0f, 20f, Time.deltaTime);

		mTrans.localRotation = Quaternion.Euler(0f, 0f, -mAngle);
	}
}