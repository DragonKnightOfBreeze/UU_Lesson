using UnityEngine;

public class DragTrail : MonoBehaviour {
	public LineRenderer lineRendererPrefab;
	private LineRenderer lineRenderer;

	// Use this for initialization
	private void Start() {
		lineRenderer = Instantiate(lineRendererPrefab, transform.position, transform.rotation);
		lineRenderer.transform.parent = transform;
		lineRenderer.enabled = false;
	}

	// call triggered by the Draggable script
	private void OnDragBegin() {
		// initialize the line renderer
		lineRenderer.enabled = true;
		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(1, transform.position);

		// keep end point width in sync with object's current scale
		lineRenderer.startWidth = 0.01f;
		lineRenderer.endWidth = transform.localScale.x;
	}

	// call triggered by the Draggable script
	private void OnDragEnd() {
		lineRenderer.enabled = false;
	}

	private void Update() {
		if(lineRenderer.enabled)
			lineRenderer.SetPosition(1, transform.position);
	}
}