using UnityEngine;

/// <summary>
///     This sample demonstrates some of the supported one-finger gestures: - TwoFingerLongPress - TwoFingerTap -
///     TwoFingerDrag - TwoFingerSwipe
/// </summary>
public class TwoFingerGestureSample : SampleBase {
	#region Properties exposed to the editor

	public GameObject longPressObject;
	public GameObject tapObject;
	public GameObject swipeObject;
	public GameObject dragObject;

	public int requiredTapCount = 2;

	#endregion

	#region Misc

	protected override string GetHelpText() {
		return @"This sample demonstrates some of the supported two-finger gestures:

- Drag: press the red sphere with two fingers and move them to drag the sphere around  

- LongPress: keep your two fingers pressed on the cyan sphere for at least " + FingerGestures.Defaults.Fingers[0].LongPress.Duration + @" seconds

- Tap: rapidly press & release the purple sphere " + requiredTapCount + @" times with two fingers

- Swipe: press the yellow sphere with two fingers and move them in one of the four cardinal directions, then release your fingers. The speed of the motion is taken into account.";
	}

	#endregion

	#region Gesture event registration/unregistration

	private void OnEnable() {
		Debug.Log("Registering finger gesture events from C# script");

		// register input events
		FingerGestures.OnTwoFingerLongPress += FingerGestures_OnTwoFingerLongPress;
		FingerGestures.OnTwoFingerTap += FingerGestures_OnTwoFingerTap;
		FingerGestures.OnTwoFingerSwipe += FingerGestures_OnTwoFingerSwipe;
		FingerGestures.OnTwoFingerDragBegin += FingerGestures_OnTwoFingerDragBegin;
		FingerGestures.OnTwoFingerDragMove += FingerGestures_OnTwoFingerDragMove;
		FingerGestures.OnTwoFingerDragEnd += FingerGestures_OnTwoFingerDragEnd;
	}

	private void OnDisable() {
		// unregister finger gesture events
		FingerGestures.OnTwoFingerLongPress -= FingerGestures_OnTwoFingerLongPress;
		FingerGestures.OnTwoFingerTap -= FingerGestures_OnTwoFingerTap;
		FingerGestures.OnTwoFingerSwipe -= FingerGestures_OnTwoFingerSwipe;
		FingerGestures.OnTwoFingerDragBegin -= FingerGestures_OnTwoFingerDragBegin;
		FingerGestures.OnTwoFingerDragMove -= FingerGestures_OnTwoFingerDragMove;
		FingerGestures.OnTwoFingerDragEnd -= FingerGestures_OnTwoFingerDragEnd;
	}

	#endregion

	#region Reaction to gesture events

	private void FingerGestures_OnTwoFingerLongPress(Vector2 fingerPos) {
		if(CheckSpawnParticles(fingerPos, longPressObject))
			UI.StatusText = "Performed a two-finger long-press";
	}

	private void FingerGestures_OnTwoFingerTap(Vector2 fingerPos, int tapCount) {
		// spawn some particles when tapping the object at least requiredTapCount times
		if(tapCount == requiredTapCount)
			if(CheckSpawnParticles(fingerPos, tapObject))
				UI.StatusText = "Tapped " + requiredTapCount + " times with two fingers";
	}

	// spin the yellow cube when swipping it
	private void FingerGestures_OnTwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {
		// make sure we started the swipe gesture on our swipe object
		var selection = PickObject(startPos);
		if(selection == swipeObject) {
			UI.StatusText = "Swiped " + direction + " with two fingers";

			var emitter = selection.GetComponentInChildren<SwipeParticlesEmitter>();
			if(emitter)
				emitter.Emit(direction, velocity);
		}
	}

	#region Drag & Drop Gesture

	private bool dragging;

	private void FingerGestures_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos) {
		// make sure we raycast from the initial finger position, not the current finger position (see remark about dragTreshold in comments)
		var selection = PickObject(startPos);
		if(selection == dragObject) {
			dragging = true;

			UI.StatusText = "Started dragging with two fingers";

			// spawn some particles because it's cool.
			SpawnParticles(selection);
		}
	}

	private void FingerGestures_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta) {
		if(dragging)
			dragObject.transform.position = GetWorldPos(fingerPos);
	}

	private void FingerGestures_OnTwoFingerDragEnd(Vector2 fingerPos) {
		if(dragging) {
			UI.StatusText = "Stopped dragging with two fingers";

			// spawn some particles because it's cool.
			SpawnParticles(dragObject);

			dragging = false;
		}
	}

	#endregion

	#endregion

	#region Utils

	// attempt to pick the scene object at the given finger position and compare it to the given requiredObject. 
	// If it's this object spawn its particles.
	private bool CheckSpawnParticles(Vector2 fingerPos, GameObject requiredObject) {
		var selection = PickObject(fingerPos);

		if(!selection || selection != requiredObject)
			return false;

		SpawnParticles(selection);
		return true;
	}

	private void SpawnParticles(GameObject obj) {
		var emitter = obj.GetComponentInChildren<ParticleSystem>();
		if(emitter)
			emitter.Emit(1);
	}

	#endregion
}