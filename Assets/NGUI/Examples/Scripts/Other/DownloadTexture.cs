//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>Simple script that shows how to download a remote texture and assign it to be used by a UITexture.</summary>
[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour {
	public string url = "http://www.yourwebsite.com/logo.png";
	public bool pixelPerfect = true;

	private Texture2D mTex;

	private IEnumerator Start() {
		var www = new WWW(url);
		yield return www;
		mTex = www.texture;

		if(mTex != null) {
			var ut = GetComponent<UITexture>();
			ut.mainTexture = mTex;
			if(pixelPerfect) ut.MakePixelPerfect();
		}
		www.Dispose();
	}

	private void OnDestroy() {
		if(mTex != null) Destroy(mTex);
	}
}