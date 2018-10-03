//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>Equip the specified items on the character when the script is started.</summary>
[AddComponentMenu("NGUI/Examples/Equip Items")]
public class EquipItems : MonoBehaviour {
	public int[] itemIDs;

	private void Start() {
		if(itemIDs != null && itemIDs.Length > 0) {
			var eq = GetComponent<InvEquipment>();
			if(eq == null) eq = gameObject.AddComponent<InvEquipment>();

			var qualityLevels = (int) InvGameItem.Quality._LastDoNotUse;

			for(int i = 0, imax = itemIDs.Length; i < imax; ++i) {
				var index = itemIDs[i];
				var item = InvDatabase.FindByID(index);

				if(item != null) {
					var gi = new InvGameItem(index, item);
					gi.quality = (InvGameItem.Quality) Random.Range(0, qualityLevels);
					gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
					eq.Equip(gi);
				}
				else {
					Debug.LogWarning("Can't resolve the item ID of " + index);
				}
			}
		}
		Destroy(this);
	}
}