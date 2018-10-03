using UnityEngine;

/// <summary>
///     ToolBox FingerUp Component Put this script on any 3D GameObject to detect when a finger has just been lifted
///     from them
/// </summary>
[AddComponentMenu("FingerGestures/Toolbox/FingerUp")]
public class TBFingerUp : TBComponent {
	public Message message = new Message("OnFingerUp");
	public event EventHandler<TBFingerUp> OnFingerUp;

	private float timeHeldDown;
	public float TimeHeldDown {
		get { return timeHeldDown; }
		private set { timeHeldDown = value; }
	}

	public bool RaiseFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown) {
		FingerIndex = fingerIndex;
		FingerPos = fingerPos;
		TimeHeldDown = timeHeldDown;

		if(OnFingerUp != null)
			OnFingerUp(this);

		Send(message);
		return true;
	}
}