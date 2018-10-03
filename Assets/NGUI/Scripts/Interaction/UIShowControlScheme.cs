//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>Show or hide the widget based on whether the control scheme is appropriate.</summary>
public class UIShowControlScheme : MonoBehaviour {
	public GameObject target;
	public bool mouse;
	public bool touch;
	public bool controller = true;

	private void OnEnable() {
		UICamera.onSchemeChange += OnScheme;
		OnScheme();
	}

	private void OnDisable() {
		UICamera.onSchemeChange -= OnScheme;
	}

	private void OnScheme() {
		if(target != null) {
			var scheme = UICamera.currentScheme;
			if(scheme == UICamera.ControlScheme.Mouse) target.SetActive(mouse);
			else if(scheme == UICamera.ControlScheme.Touch) target.SetActive(touch);
			else if(scheme == UICamera.ControlScheme.Controller) target.SetActive(controller);
		}
	}
}