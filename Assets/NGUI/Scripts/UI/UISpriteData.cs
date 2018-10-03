//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

[System.Serializable]
public class UISpriteData {
	public string name = "Sprite";
	public int x;
	public int y;
	public int width;
	public int height;

	public int borderLeft;
	public int borderRight;
	public int borderTop;
	public int borderBottom;

	public int paddingLeft;
	public int paddingRight;
	public int paddingTop;
	public int paddingBottom;

	//bool rotated = false;

	/// <summary>Whether the sprite has a border.</summary>

	public bool hasBorder => (borderLeft | borderRight | borderTop | borderBottom) != 0;

	/// <summary>Whether the sprite has been offset via padding.</summary>

	public bool hasPadding => (paddingLeft | paddingRight | paddingTop | paddingBottom) != 0;

	/// <summary>Convenience function -- set the X, Y, width, and height.</summary>
	public void SetRect(int x, int y, int width, int height) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	/// <summary>Convenience function -- set the sprite's padding.</summary>
	public void SetPadding(int left, int bottom, int right, int top) {
		paddingLeft = left;
		paddingBottom = bottom;
		paddingRight = right;
		paddingTop = top;
	}

	/// <summary>Convenience function -- set the sprite's border.</summary>
	public void SetBorder(int left, int bottom, int right, int top) {
		borderLeft = left;
		borderBottom = bottom;
		borderRight = right;
		borderTop = top;
	}

	/// <summary>Copy all values of the specified sprite data.</summary>
	public void CopyFrom(UISpriteData sd) {
		name = sd.name;

		x = sd.x;
		y = sd.y;
		width = sd.width;
		height = sd.height;

		borderLeft = sd.borderLeft;
		borderRight = sd.borderRight;
		borderTop = sd.borderTop;
		borderBottom = sd.borderBottom;

		paddingLeft = sd.paddingLeft;
		paddingRight = sd.paddingRight;
		paddingTop = sd.paddingTop;
		paddingBottom = sd.paddingBottom;
	}

	/// <summary>Copy the border information from the specified sprite.</summary>
	public void CopyBorderFrom(UISpriteData sd) {
		borderLeft = sd.borderLeft;
		borderRight = sd.borderRight;
		borderTop = sd.borderTop;
		borderBottom = sd.borderBottom;
	}
}