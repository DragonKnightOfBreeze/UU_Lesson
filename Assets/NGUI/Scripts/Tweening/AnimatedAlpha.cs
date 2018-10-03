//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>Makes it possible to animate alpha of the widget or a panel.</summary>
[ExecuteInEditMode]
public class AnimatedAlpha : MonoBehaviour {
	[Range(0f, 1f)]
	public float alpha = 1f;

	private UIWidget mWidget;
	private UIPanel mPanel;

	private void OnEnable() {
		mWidget = GetComponent<UIWidget>();
		mPanel = GetComponent<UIPanel>();
		LateUpdate();
	}

	private void LateUpdate() {
		if(mWidget != null) mWidget.alpha = alpha;
		if(mPanel != null) mPanel.alpha = alpha;
	}
}