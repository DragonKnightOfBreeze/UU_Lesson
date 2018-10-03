//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
///     Event Hook class lets you easily add remote event listener functions to an object. Example usage:
///     UIEventListener.Get(gameObject).onClick += MyClickFunction;
/// </summary>
[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour {
	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, bool state);

	public delegate void FloatDelegate(GameObject go, float delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

	public object parameter;

	public VoidDelegate onSubmit;
	public VoidDelegate onClick;
	public VoidDelegate onDoubleClick;
	public BoolDelegate onHover;
	public BoolDelegate onPress;
	public BoolDelegate onSelect;
	public FloatDelegate onScroll;
	public VoidDelegate onDragStart;
	public VectorDelegate onDrag;
	public VoidDelegate onDragOver;
	public VoidDelegate onDragOut;
	public VoidDelegate onDragEnd;
	public ObjectDelegate onDrop;
	public KeyCodeDelegate onKey;
	public BoolDelegate onTooltip;

	private bool isColliderEnabled {
		get {
			var c = GetComponent<Collider>();
			if(c != null) return c.enabled;
			var b = GetComponent<Collider2D>();
			return b != null && b.enabled;
		}
	}

	private void OnSubmit() {
		if(isColliderEnabled && onSubmit != null) onSubmit(gameObject);
	}

	private void OnClick() {
		if(isColliderEnabled && onClick != null) onClick(gameObject);
	}

	private void OnDoubleClick() {
		if(isColliderEnabled && onDoubleClick != null) onDoubleClick(gameObject);
	}

	private void OnHover(bool isOver) {
		if(isColliderEnabled && onHover != null) onHover(gameObject, isOver);
	}

	private void OnPress(bool isPressed) {
		if(isColliderEnabled && onPress != null) onPress(gameObject, isPressed);
	}

	private void OnSelect(bool selected) {
		if(isColliderEnabled && onSelect != null) onSelect(gameObject, selected);
	}

	private void OnScroll(float delta) {
		if(isColliderEnabled && onScroll != null) onScroll(gameObject, delta);
	}

	private void OnDragStart() {
		if(onDragStart != null) onDragStart(gameObject);
	}

	private void OnDrag(Vector2 delta) {
		if(onDrag != null) onDrag(gameObject, delta);
	}

	private void OnDragOver() {
		if(isColliderEnabled && onDragOver != null) onDragOver(gameObject);
	}

	private void OnDragOut() {
		if(isColliderEnabled && onDragOut != null) onDragOut(gameObject);
	}

	private void OnDragEnd() {
		if(onDragEnd != null) onDragEnd(gameObject);
	}

	private void OnDrop(GameObject go) {
		if(isColliderEnabled && onDrop != null) onDrop(gameObject, go);
	}

	private void OnKey(KeyCode key) {
		if(isColliderEnabled && onKey != null) onKey(gameObject, key);
	}

	private void OnTooltip(bool show) {
		if(isColliderEnabled && onTooltip != null) onTooltip(gameObject, show);
	}

	public void Clear() {
		onSubmit = null;
		onClick = null;
		onDoubleClick = null;
		onHover = null;
		onPress = null;
		onSelect = null;
		onScroll = null;
		onDragStart = null;
		onDrag = null;
		onDragOver = null;
		onDragOut = null;
		onDragEnd = null;
		onDrop = null;
		onKey = null;
		onTooltip = null;
	}

	/// <summary>Get or add an event listener to the specified game object.</summary>
	public static UIEventListener Get(GameObject go) {
		var listener = go.GetComponent<UIEventListener>();
		if(listener == null) listener = go.AddComponent<UIEventListener>();
		return listener;
	}
}