/*******
 * ［前置］
 * 帮助脚本
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 资源加载管理器
 * 
 * ［功能］
 * 本脚本在Unity 的Resources.Load()方法的基础上，增加了“缓存”的处理。
 * 更好的方式：使用对象缓冲池技术。
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 注意：不需要手动挂载。
 */

using UnityEngine;
using System.Collections;

namespace SortedHelp {
	/// <summary>资源加载管理器</summary>
	public class ResourcesMgr : MonoBehaviour {
		#region ［字段］

		/// <summary>自动生成的游戏对象的名字</summary>
		private const string NAME_GO = "_ResourcesMgr";

		/// <summary>本脚本的实例</summary>
		private static ResourcesMgr _Instance;

		/// <summary>容器键值对的集合</summary>
		private Hashtable _ResourcesCache;

		#endregion


		#region ［构造器］

		/// <summary>得到实例（单例）</summary>
		/// <returns></returns>
		public static ResourcesMgr GetInstance() {
			if(_Instance == null)
				_Instance = new GameObject(NAME_GO).AddComponent<ResourcesMgr>();
			return _Instance;
		}

		#endregion


		#region ［Unity消息］

		private void Awake() {
			_ResourcesCache = new Hashtable();
		}

		#endregion


		#region ［公共方法］

		/// <summary>调用资源（带对象缓冲技术）</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">资源路径</param>
		/// <param name="isCache">是否缓存</param>
		/// <returns></returns>
		public T LoadResource<T>(string path, bool isCache = true) where T : Object {
			if(_ResourcesCache.Contains(path))
				return _ResourcesCache[path] as T;
			var TResource = Resources.Load<T>(path);
			if(TResource == null)
				Debug.LogError(GetType() + "\t找不到要调用的资源。" + "\tpath：" + path);
			else if(isCache)
				_ResourcesCache.Add(path, TResource);

			return TResource;
		}

		/// <summary>调用游戏对象（带对象缓冲技术）,返回一个克隆游戏对象</summary>
		/// <param name="path">游戏对象路径</param>
		/// <param name="isCache">是否缓存</param>
		/// <returns></returns>
		public GameObject LoadAsset(string path, bool isCache = true) {
			var go = LoadResource<GameObject>(path, isCache);
			var goClone = Instantiate(go);
			if(goClone == null)
				Debug.LogError(GetType() + "/LoadAsset()/克隆资源不成功，请检查。 path=" + path);
			return goClone;
		}

		#endregion
	}
}