using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {
	public GUIStyle titleStyle;
	public GUIStyle buttonStyle;

	public float buttonHeight = 80;

	public Transform itemsTree;

	private Transform currentMenuRoot;
	public Transform CurrentMenuRoot {
		get { return currentMenuRoot; }
		set { currentMenuRoot = value; }
	}

	// Use this for initialization
	private void Start() {
		CurrentMenuRoot = itemsTree;
	}

	private readonly Rect screenRect = new Rect(0, 0, SampleUI.VirtualScreenWidth, SampleUI.VirtualScreenHeight);
	public float menuWidth = 450;

	public float sideBorder = 30;

	private void OnGUI() {
		SampleUI.ApplyVirtualScreen();

		GUILayout.BeginArea(screenRect);
		GUILayout.BeginHorizontal();

		GUILayout.Space(sideBorder);

		if(CurrentMenuRoot) {
			GUILayout.BeginVertical();

			GUILayout.Space(15);
			GUILayout.Label(CurrentMenuRoot.name, titleStyle);

			for(var i = 0; i < CurrentMenuRoot.childCount; ++i) {
				var item = CurrentMenuRoot.GetChild(i);

				if(GUILayout.Button(item.name, GUILayout.Height(buttonHeight))) {
					var menuNode = item.GetComponent<MenuNode>();
					if(menuNode && !string.IsNullOrEmpty(menuNode.sceneName))
						SceneManager.LoadScene(menuNode.sceneName);

					else if(item.childCount > 0)
						CurrentMenuRoot = item;
				}

				GUILayout.Space(5);
			}

			GUILayout.FlexibleSpace();

			if(CurrentMenuRoot != itemsTree && CurrentMenuRoot.parent) {
				if(GUILayout.Button("<< BACK <<", GUILayout.Height(buttonHeight)))
					CurrentMenuRoot = CurrentMenuRoot.parent;

				GUILayout.Space(15);
			}

			GUILayout.EndVertical();
		}

		GUILayout.Space(sideBorder);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}