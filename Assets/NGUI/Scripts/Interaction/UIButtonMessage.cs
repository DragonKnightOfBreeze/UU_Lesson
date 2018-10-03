//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>Sends a message to the remote object when something happens.</summary>
[AddComponentMenu("NGUI/Interaction/Button Message (Legacy)")]
public class UIButtonMessage : MonoBehaviour {
	public enum Trigger {
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick
	}

	public GameObject target;
	public string functionName;
	public Trigger trigger = Trigger.OnClick;
	public bool includeChildren;

	private bool mStarted;

	private void Start() {
		mStarted = true;
	}

	private void OnEnable() {
		if(mStarted) OnHover(UICamera.IsHighlighted(gameObject));
	}

	private void OnHover(bool isOver) {
		if(enabled)
			if(isOver && trigger == Trigger.OnMouseOver ||
			   !isOver && trigger == Trigger.OnMouseOut)
				Send();
	}

	private void OnPress(bool isPressed) {
		if(enabled)
			if(isPressed && trigger == Trigger.OnPress ||
			   !isPressed && trigger == Trigger.OnRelease)
				Send();
	}

	private void OnSelect(bool isSelected) {
		if(enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
			OnHover(isSelected);
	}

	private void OnClick() {
		if(enabled && trigger == Trigger.OnClick) Send();
	}

	private void OnDoubleClick() {
		if(enabled && trigger == Trigger.OnDoubleClick) Send();
	}

	private void Send() {
		if(string.IsNullOrEmpty(functionName)) return;
		if(target == null) target = gameObject;

		if(includeChildren) {
			var transforms = target.GetComponentsInChildren<Transform>();

			for(int i = 0, imax = transforms.Length; i < imax; ++i) {
				var t = transforms[i];
				t.gameObject.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else {
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}