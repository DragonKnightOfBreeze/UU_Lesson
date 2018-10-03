using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     This sample helps visualize the following finger events: - OnFingerDown - OnFingerMoveBegin - OnFingerMove -
///     OnFingerMoveEnd - OnFingerUp
/// </summary>
public class FingerEventsSamplePart2 : SampleBase {
	#region Properties exposed to the editor

	public LineRenderer lineRendererPrefab;
	public GameObject fingerDownMarkerPrefab;
	public GameObject fingerMoveBeginMarkerPrefab;
	public GameObject fingerMoveEndMarkerPrefab;
	public GameObject fingerUpMarkerPrefab;

	#endregion

	#region Utility class that represent a single finger path

	private class PathRenderer {
		private readonly LineRenderer lineRenderer;

		// passage points
		private readonly List<Vector3> points = new List<Vector3>();

		// list of marker objects currently instantiated
		private readonly List<GameObject> markers = new List<GameObject>();

		public PathRenderer(int index, LineRenderer lineRendererPrefab) {
			lineRenderer = Instantiate(lineRendererPrefab);
			lineRenderer.name = lineRendererPrefab.name + index;
			lineRenderer.enabled = true;

			UpdateLines();
		}

		public void Reset() {
			points.Clear();
			UpdateLines();

			// destroy markers
			foreach(var marker in markers)
				Destroy(marker);

			markers.Clear();
		}

		public void AddPoint(Vector2 screenPos) {
			AddPoint(screenPos, null);
		}

		public void AddPoint(Vector2 screenPos, GameObject markerPrefab) {
			var pos = GetWorldPos(screenPos);

			if(markerPrefab)
				AddMarker(pos, markerPrefab);

			points.Add(pos);
			UpdateLines();
		}

		private GameObject AddMarker(Vector2 pos, GameObject prefab) {
			var instance = Instantiate(prefab, pos, Quaternion.identity);
			instance.name = prefab.name + "(" + markers.Count + ")";
			markers.Add(instance);
			return instance;
		}

		private void UpdateLines() {
			lineRenderer.positionCount = points.Count;

			for(var i = 0; i < points.Count; ++i)
				lineRenderer.SetPosition(i, points[i]);
		}
	}

	#endregion

	// one PathRenderer per finger
	private PathRenderer[] paths;

	#region Setup

	protected override void Start() {
		base.Start();

		UI.StatusText = "Drag your fingers anywhere on the screen";

		// create one PathRenderer per finger
		paths = new PathRenderer[FingerGestures.Instance.MaxFingers];
		for(var i = 0; i < paths.Length; ++i)
			paths[i] = new PathRenderer(i, lineRendererPrefab);
	}

	protected override string GetHelpText() {
		return @"This sample lets you visualize the FingerDown, FingerMoveBegin, FingerMove, FingerMoveEnd and FingerUp events.

INSTRUCTIONS:
Move your finger accross the screen and observe what happens.

LEGEND:
- Red Circle = FingerDown position
- Yellow Square = FingerMoveBegin position
- Green Sphere = FingerMoveEnd position
- Blue Circle = FingerUp position";
	}

	#endregion


	#region Gesture event registration/unregistration

	private void OnEnable() {
		Debug.Log("Registering finger gesture events from C# script");

		// register input events
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
		FingerGestures.OnFingerMoveBegin += FingerGestures_OnFingerMoveBegin;
		FingerGestures.OnFingerMove += FingerGestures_OnFingerMove;
		FingerGestures.OnFingerMoveEnd += FingerGestures_OnFingerMoveEnd;
	}

	private void OnDisable() {
		// unregister finger gesture events
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
		FingerGestures.OnFingerMoveBegin -= FingerGestures_OnFingerMoveBegin;
		FingerGestures.OnFingerMove -= FingerGestures_OnFingerMove;
		FingerGestures.OnFingerMoveEnd -= FingerGestures_OnFingerMoveEnd;
	}

	#endregion

	#region Reaction to finger events

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos) {
		var path = paths[fingerIndex];
		path.Reset();
		path.AddPoint(fingerPos, fingerDownMarkerPrefab);
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown) {
		var path = paths[fingerIndex];
		path.AddPoint(fingerPos, fingerUpMarkerPrefab);

		UI.StatusText = "Finger " + fingerIndex + " was held down for " + timeHeldDown.ToString("N2") + " seconds";
	}

	private void FingerGestures_OnFingerMoveBegin(int fingerIndex, Vector2 fingerPos) {
		UI.StatusText = "Started moving finger " + fingerIndex;

		var path = paths[fingerIndex];
		path.AddPoint(fingerPos, fingerMoveBeginMarkerPrefab);
	}

	private void FingerGestures_OnFingerMove(int fingerIndex, Vector2 fingerPos) {
		var path = paths[fingerIndex];
		path.AddPoint(fingerPos);
	}

	private void FingerGestures_OnFingerMoveEnd(int fingerIndex, Vector2 fingerPos) {
		UI.StatusText = "Stopped moving finger " + fingerIndex;

		var path = paths[fingerIndex];
		path.AddPoint(fingerPos, fingerMoveEndMarkerPrefab);
	}

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