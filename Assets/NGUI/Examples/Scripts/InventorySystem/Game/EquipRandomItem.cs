//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>Create and equip a random item on the specified target.</summary>
[AddComponentMenu("NGUI/Examples/Equip Random Item")]
public class EquipRandomItem : MonoBehaviour {
	public InvEquipment equipment;

	private void OnClick() {
		if(equipment == null) return;
		var list = InvDatabase.list[0].items;
		if(list.Count == 0) return;

		var qualityLevels = (int) InvGameItem.Quality._LastDoNotUse;
		var index = Random.Range(0, list.Count);
		var item = list[index];

		var gi = new InvGameItem(index, item);
		gi.quality = (InvGameItem.Quality) Random.Range(0, qualityLevels);
		gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
		equipment.Equip(gi);
	}
}