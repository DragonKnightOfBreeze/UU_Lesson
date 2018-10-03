/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 
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

//using Kernel;
//using Global;

namespace UU_LessonTest {
	/// <summary></summary>
	public class MonoTest : MonoBehaviour {
		//脚本启用时执行，在Start()前面执行
		private void Awake() { }

		//脚本开始时执行
		private void Start() { }

		//每帧执行一次
		private void Update() { }

		//每帧之后执行
		private void LateUpdate() { }

		//固定时间执行
		private void FixedUpdate() { }

		//被挂载的游戏物体启用时执行
		private void OnEnable() { }

		//被挂载的游戏物体禁用时执行
		private void OnDisable() { }

		//被挂载的游戏物体被销毁之前执行
		private void OnDestroy() { }

		private void MathfTest() {

			//限制，返回对于最大值和最小值，相距最近的数
			Mathf.Clamp(150, 100f, 200f);

			//返回插值（最小数 + 两数之差 * 指定的在0和1之间的数），
			var t = 0.8f;
			Mathf.Lerp(100f, 200f, t);

			//返回四舍五入后的参数
			Mathf.Round(1.4f);

			//返回绝对值
			Mathf.Abs(1f);

		}
	}
}