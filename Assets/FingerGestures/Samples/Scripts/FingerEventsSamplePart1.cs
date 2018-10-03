using UnityEngine;

/// <summary>
///     This sample lets you visualize and understand the following finger input events: - OnFingerDown -
///     OnFingerStationaryBegin - OnFingerStationary - OnFingerStationaryEnd - OnFingerUp
/// </summary>
public class FingerEventsSamplePart1 : SampleBase {
	#region Properties exposed to the editor

	public GameObject fingerDownObject;
	public GameObject fingerStationaryObject;
	public GameObject fingerUpObject;

	public float chargeDelay = 0.5f;
	public float chargeTime = 5.0f;
	public float minSationaryParticleEmissionCount = 5;
	public float maxSationaryParticleEmissionCount = 50;
	public Material stationaryMaterial;

	public int requiredTapCount = 2;

	#endregion


	#region Setup

	protected override string GetHelpText() {
		return @"This sample lets you visualize and understand the FingerDown, FingerStationary and FingerUp events.

INSTRUCTIONS:
- Press, hold and release the red and blue spheres
- Press & hold the green sphere without moving for a few seconds";
	}

	private ParticleSystem stationaryParticleEmitter;

	protected override void Start() {
		base.Start();

		if(fingerStationaryObject)
			stationaryParticleEmitter = fingerStationaryObject.GetComponentInChildren<ParticleSystem>();
	}

	private void StopStationaryParticleEmitter() {
		var _emissionModule = stationaryParticleEmitter.emission;
		_emissionModule.enabled = false;
		UI.StatusText = "";
	}

	#endregion

	#region Gesture event registration/unregistration

	private void OnEnable() {
		Debug.Log("Registering finger gesture events from C# script");

		// register input events
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
		FingerGestures.OnFingerStationaryBegin += FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary += FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd += FingerGestures_OnFingerStationaryEnd;
	}

	private void OnDisable() {
		// unregister finger gesture events
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
		FingerGestures.OnFingerStationaryBegin -= FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary -= FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd -= FingerGestures_OnFingerStationaryEnd;
	}

	#endregion

	#region Reaction to gesture events

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos) {
		CheckSpawnParticles(fingerPos, fingerDownObject);
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown) {
		CheckSpawnParticles(fingerPos, fingerUpObject);

		// this shows how to access a finger object using its index
		// The finger object contains useful information not available through the event arguments that you might want to use
		var finger = FingerGestures.GetFinger(fingerIndex);
		Debug.Log("Finger was lifted up at " + finger.Position + " and moved " + finger.DistanceFromStart.ToString("N0") + " pixels from its initial position at " + finger.StartPosition);
	}

	#region Stationary events

	private int stationaryFingerIndex = -1;
	private Material originalMaterial;

	private void FingerGestures_OnFingerStationaryBegin(int fingerIndex, Vector2 fingerPos) {
		// skip if we're already holding another finger stationary on our object
		if(stationaryFingerIndex != -1)
			return;

		var selection = PickObject(fingerPos);
		if(selection == fingerStationaryObject) {
			UI.StatusText = "Begin stationary on finger " + fingerIndex;

			// remember which finger we're using
			stationaryFingerIndex = fingerIndex;

			// remember the original material 
			originalMaterial = selection.GetComponent<Renderer>().sharedMaterial;

			// change the material to show we've started the stationary state
			selection.GetComponent<Renderer>().sharedMaterial = stationaryMaterial;
		}
	}

	private void FingerGestures_OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime) {
		if(elapsedTime < chargeDelay)
			return;

		var selection = PickObject(fingerPos);
		if(selection == fingerStationaryObject) {
			// compute charge progress % (0 to 1)
			var chargePercent = Mathf.Clamp01((elapsedTime - chargeDelay) / chargeTime);

			// compute and apply new particle emission rate based on charge %
			var emissionRate = Mathf.Lerp(minSationaryParticleEmissionCount, maxSationaryParticleEmissionCount, chargePercent);


			var emissionModule = stationaryParticleEmitter.emission;
			emissionModule.rateOverTimeMultiplier = emissionRate;

			// make sure the emitter is turned on
			emissionModule.enabled = true;
			UI.StatusText = "Charge: " + (100 * chargePercent).ToString("N1") + "%";
		}
	}

	private void FingerGestures_OnFingerStationaryEnd(int fingerIndex, Vector2 fingerPos, float elapsedTime) {
		if(fingerIndex == stationaryFingerIndex) {
			UI.StatusText = "Stationary ended on finger " + fingerIndex + " - " + elapsedTime.ToString("N1") + " seconds elapsed";

			// turn off the stationary particle emitter when we begin to move the finger, as it's no longer stationary
			StopStationaryParticleEmitter();

			// restore the original material
			fingerStationaryObject.GetComponent<Renderer>().sharedMaterial = originalMaterial;

			// reset our stationary finger index
			stationaryFingerIndex = -1;
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