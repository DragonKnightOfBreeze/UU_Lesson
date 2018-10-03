using UnityEngine;

public class OpenURLOnClick : MonoBehaviour {
	private void OnClick() {
		var lbl = GetComponent<UILabel>();

		if(lbl != null) {
			var url = lbl.GetUrlAtPosition(UICamera.lastWorldPosition);
			if(!string.IsNullOrEmpty(url)) Application.OpenURL(url);
		}
	}
}