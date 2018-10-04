/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * 场景管理器
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 单例模式，动态挂载
 * 在场景切换时不消失
 */
using UU_Lesson.Global;
using UnityEngine;
using UnityEngine.SceneManagement;
using UU_Lesson.UI;

namespace UU_Lesson.Common {
	/// <summary>场景管理器</summary>
	public class ScenesMgr : MonoBehaviour {
		#region ［单例模式］

		private static ScenesMgr _Instance;

		public static ScenesMgr GetInstance() {
			if(_Instance == null)
				_Instance = new GameObject("_" + nameof(ScenesMgr)).AddComponent<ScenesMgr>();
			return _Instance;
		}

		#endregion

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}
		
		/// <summary>当前场景类型</summary>
		public SceneType CurSceneType { get; private set; }
		/// <summary>
		/// 接下来的场景类型
		/// </summary>
		public SceneType NextSceneType { get; set; }

		/// <summary>加载场景</summary>
		/// <param name="type">场景类型（名称）</param>
		public void LoadScene(SceneType type) {
			//加载场景（非异步）
			SceneManager.LoadScene(type.ToString());
			//重置层级
			UILayerMgr.GetInstance().ResetDepth();
			//设置当前场景类型
			CurSceneType = type;
		}

		/// <summary>
		/// 加载到Loading场景
		/// </summary>
		public void LoadToLoadingScene() {
			LoadScene(SceneType.LoadingScene);
		}
	}
}