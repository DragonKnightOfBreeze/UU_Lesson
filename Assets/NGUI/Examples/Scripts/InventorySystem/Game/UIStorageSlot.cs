﻿//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>A UI script that keeps an eye on the slot in a storage container.</summary>
[AddComponentMenu("NGUI/Examples/UI Storage Slot")]
public class UIStorageSlot : UIItemSlot {
	public UIItemStorage storage;
	public int slot;

	protected override InvGameItem observedItem => storage != null ? storage.GetItem(slot) : null;

	/// <summary>Replace the observed item with the specified value. Should return the item that was replaced.</summary>
	protected override InvGameItem Replace(InvGameItem item) {
		return storage != null ? storage.Replace(slot, item) : item;
	}
}