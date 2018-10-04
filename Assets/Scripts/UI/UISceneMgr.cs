/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 场景UI管理器
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 动态挂载
 */
using UU_Lesson.Global;
using UnityEngine;
using UU_Lesson.Common;

namespace UU_Lesson.UI {
	/// <summary>UI场景管理器</summary>
	public class UISceneMgr : MonoBehaviour {
		#region ［单例模式：自动挂载］

		private static UISceneMgr _Instance;

		public static UISceneMgr GetInstance() {
			if(_Instance == null)
				_Instance = new GameObject("_" + nameof(UISceneMgr)).AddComponent<UISceneMgr>();
			return _Instance;
		}

		#endregion

		/// <summary>当前的UI场景</summary>
		public UIScene_Base CurUIScene;

		/// <summary>加载UI场景</summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public GameObject LoadUIScene(UISceneType type) {
			//加载资源
			GameObject go = ResourcesMgr.GetInstance()
				.Load(ResourceType.UIScenes_NGUI, type.ToString());
			//空引用检查
			if(go == null)
				Debug.LogError("找不到指定的UI场景！");

			//设置当前UI场景
			CurUIScene = go.GetComponent<UIScene_Base>();

			return go;
		}
	}
}