//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>This script automatically changes the color of the specified sprite based on the value of the slider.</summary>
[AddComponentMenu("NGUI/Examples/Slider Colors")]
public class UISliderColors : MonoBehaviour {
	public UISprite sprite;

	public Color[] colors = {Color.red, Color.yellow, Color.green};

	private UIProgressBar mBar;
	private UIBasicSprite mSprite;

	private void Start() {
		mBar = GetComponent<UIProgressBar>();
		mSprite = GetComponent<UIBasicSprite>();
		Update();
	}

	private void Update() {
		if(sprite == null || colors.Length == 0) return;

		var val = mBar != null ? mBar.value : mSprite.fillAmount;
		val *= colors.Length - 1;
		var startIndex = Mathf.FloorToInt(val);

		var c = colors[0];

		if(startIndex >= 0) {
			if(startIndex + 1 < colors.Length) {
				var factor = val - startIndex;
				c = Color.Lerp(colors[startIndex], colors[startIndex + 1], factor);
			}
			else if(startIndex < colors.Length) {
				c = colors[startIndex];
			}
			else {
				c = colors[colors.Length - 1];
			}
		}

		c.a = sprite.color.a;
		sprite.color = c;
	}
}