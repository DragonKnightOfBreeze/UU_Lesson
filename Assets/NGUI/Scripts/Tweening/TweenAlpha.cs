//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>Tween the object's alpha. Works with both UI widgets as well as renderers.</summary>
[AddComponentMenu("NGUI/Tween/Tween Alpha")]
public class TweenAlpha : UITweener {
	[Range(0f, 1f)]
	public float from = 1f;
	[Range(0f, 1f)]
	public float to = 1f;

	private bool mCached;
	private UIRect mRect;
	private Material mMat;
	private Light mLight;
	private SpriteRenderer mSr;
	private float mBaseIntensity = 1f;

	[System.Obsolete("Use 'value' instead")]
	public float alpha {
		get { return value; }
		set { this.value = value; }
	}

	private void Cache() {
		mCached = true;
		mRect = GetComponent<UIRect>();
		mSr = GetComponent<SpriteRenderer>();

		if(mRect == null && mSr == null) {
			mLight = GetComponent<Light>();

			if(mLight == null) {
				var ren = GetComponent<Renderer>();
				if(ren != null) mMat = ren.material;
				if(mMat == null) mRect = GetComponentInChildren<UIRect>();
			}
			else {
				mBaseIntensity = mLight.intensity;
			}
		}
	}

	/// <summary>Tween's current value.</summary>

	public float value {
		get {
			if(!mCached) Cache();
			if(mRect != null) return mRect.alpha;
			if(mSr != null) return mSr.color.a;
			return mMat != null ? mMat.color.a : 1f;
		}
		set {
			if(!mCached) Cache();

			if(mRect != null) {
				mRect.alpha = value;
			}
			else if(mSr != null) {
				var c = mSr.color;
				c.a = value;
				mSr.color = c;
			}
			else if(mMat != null) {
				var c = mMat.color;
				c.a = value;
				mMat.color = c;
			}
			else if(mLight != null) {
				mLight.intensity = mBaseIntensity * value;
			}
		}
	}

	/// <summary>Tween the value.</summary>
	protected override void OnUpdate(float factor, bool isFinished) {
		value = Mathf.Lerp(from, to, factor);
	}

	/// <summary>Start the tweening operation.</summary>
	public static TweenAlpha Begin(GameObject go, float duration, float alpha, float delay = 0f) {
		var comp = UITweener.Begin<TweenAlpha>(go, duration, delay);
		comp.from = comp.value;
		comp.to = alpha;

		if(duration <= 0f) {
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	public override void SetStartToCurrentValue() {
		from = value;
	}

	public override void SetEndToCurrentValue() {
		to = value;
	}
}