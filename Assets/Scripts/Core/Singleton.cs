/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 单例
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 
 */
using UnityEngine;

namespace Core {
	/// <summary>单例</summary>
	public class Singleton<T> : MonoBehaviour
		where T : new() {
		private static T instance;

		/// <summary>得到指定类的单例</summary>
		/// <returns></returns>
		public static T GetInstance() {
			if(instance == null)
				instance = new T();
			return instance;
		}
	}
}