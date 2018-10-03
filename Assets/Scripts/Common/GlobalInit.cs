/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 *
 *
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 挂载到一个空游戏对象上，进行细调。
 */
using UnityEngine;

namespace UU_Lesson.Common {
	/// <summary>全局初始化</summary>
	public class GlobalInit : MonoBehaviour {
		public static GlobalInit Instance;

		/// <summary>UI动画曲线（需要细改）</summary>
		public AnimationCurve UIAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

		private void Awake() {
			Instance = this;
			DontDestroyOnLoad(gameObject); //不要再加载时销毁
		}
	}
}