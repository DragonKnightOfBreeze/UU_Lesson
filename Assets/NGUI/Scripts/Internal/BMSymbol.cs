//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>Symbols are a sequence of characters such as ":)" that get replaced with a sprite, such as the smiley face.</summary>
[System.Serializable]
public class BMSymbol {
	public string sequence;
	public string spriteName;

	private UISpriteData mSprite;
	private bool mIsValid;
	private int mLength;
	private int mOffsetX; // (outer - inner) in pixels
	private int mOffsetY; // (outer - inner) in pixels
	private int mWidth;   // Symbol's width in pixels (sprite.outer.width)
	private int mHeight;  // Symbol's height in pixels (sprite.outer.height)
	private int mAdvance; // Symbol's inner width in pixels (sprite.inner.width)
	private Rect mUV;

	public int length {
		get {
			if(mLength == 0) mLength = sequence.Length;
			return mLength;
		}
	}
	public int offsetX => mOffsetX;
	public int offsetY => mOffsetY;
	public int width => mWidth;
	public int height => mHeight;
	public int advance => mAdvance;
	public Rect uvRect => mUV;

	/// <summary>Mark this symbol as dirty, clearing the sprite reference.</summary>
	public void MarkAsChanged() {
		mIsValid = false;
	}

	/// <summary>Validate this symbol, given the specified atlas.</summary>
	public bool Validate(UIAtlas atlas) {
		if(atlas == null) return false;

#if UNITY_EDITOR
		if(!Application.isPlaying || !mIsValid)
#else
		if (!mIsValid)
#endif 
		{
			if(string.IsNullOrEmpty(spriteName)) return false;

			mSprite = atlas != null ? atlas.GetSprite(spriteName) : null;

			if(mSprite != null) {
				var tex = atlas.texture;

				if(tex == null) {
					mSprite = null;
				}
				else {
					mUV = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
					mUV = NGUIMath.ConvertToTexCoords(mUV, tex.width, tex.height);
					mOffsetX = mSprite.paddingLeft;
					mOffsetY = mSprite.paddingTop;
					mWidth = mSprite.width;
					mHeight = mSprite.height;
					mAdvance = mSprite.width + mSprite.paddingLeft + mSprite.paddingRight;
					mIsValid = true;
				}
			}
		}
		return mSprite != null;
	}
}