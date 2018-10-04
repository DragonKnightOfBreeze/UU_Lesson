/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * 资源管理器
 * 
 * ［功能］
 * 管理资源，添加缓存功能
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 单例模式，动态挂载
 * 在场景切换时不消失
 */
using System;
using System.Collections;
using UU_Lesson.Global;
using UnityEngine;

namespace UU_Lesson.Common {
	/// <summary>资源管理器</summary>
	public class ResourcesMgr : MonoBehaviour, IDisposable {
		#region ［单例模式：动态挂载］

		private static ResourcesMgr _Instance;

		public static ResourcesMgr GetInstance() {
			if(_Instance == null)
				_Instance = new GameObject("_" + nameof(ResourcesMgr)).AddComponent<ResourcesMgr>();
			return _Instance;
		}

		#endregion

		/// <summary>资源缓存</summary>
		private readonly Hashtable resourceCache = new Hashtable();

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}
		

		/// <summary>加载资源</summary>
		/// <param name="type">资源类型</param>
		/// <param name="shotPath">资源的短路径</param>
		/// <param name="isCache">是否放入缓存</param>
		/// <returns>预设的克隆体</returns>
		public GameObject Load(ResourceType type, string shotPath, bool isCache = false) {
			GameObject go;
			if(resourceCache.Contains(shotPath)) {
				//从缓存中得到资源
				go = resourceCache[shotPath] as GameObject;
			}
			else {
				//得到完整的路径，格式：Assets/Resources/Prefabs/TYPE/SHOT_PATH
				var fullPath = "Prefabs" + "/" + type + "/" + shotPath;
				//加载资源
				go = Resources.Load<GameObject>(fullPath);
				//如果需要放入缓存，则将短路径作为键，游戏对象作为值加入哈希表
				if(isCache)
					resourceCache.Add(shotPath, go);
			}
			//返回克隆的游戏对象
			return Instantiate(go);
		}


		/// <summary>释放资源</summary>
		public void Dispose() {
			//清空缓存列表
			resourceCache.Clear();
			//把未使用的资源释放
			Resources.UnloadUnusedAssets();
		}

	}
}