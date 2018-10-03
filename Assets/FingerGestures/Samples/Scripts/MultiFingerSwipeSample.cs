using UnityEngine;

public class MultiFingerSwipeSample : SampleBase {
	#region Properties exposed to the editor

	public SwipeGestureRecognizer swipeGesture;
	public GameObject sphereObject;

	public float baseEmitSpeed = 4.0f;
	public float swipeVelocityEmitSpeedScale = 0.001f;

	#endregion

	#region Misc

	protected override string GetHelpText() {
		return @"Swipe: press the yellow sphere with " + swipeGesture.RequiredFingerCount + " fingers and move them in one of the four cardinal directions, then release. The speed of the motion is taken into account.";
	}

	#endregion

	protected override void Start() {
		base.Start();

		swipeGesture.OnSwipe += OnSwipe;
	}

	private void OnSwipe(SwipeGestureRecognizer source) {
		// make sure we started the swipe gesture on our swipe object
		var selection = PickObject(source.StartPosition);
		if(selection == sphereObject) {
			UI.StatusText = "Swiped " + source.Direction + " with velocity: " + source.Velocity;

			var emitter = selection.GetComponentInChildren<SwipeParticlesEmitter>();
			if(emitter)
				emitter.Emit(source.Direction, source.Velocity);
		}
	}

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