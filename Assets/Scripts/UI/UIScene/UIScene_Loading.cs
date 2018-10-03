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
 * 挂载到相应脚本上
 */
using UnityEngine;

namespace UU_Lesson.UI {
	/// <summary></summary>
	public class UIScene_Loading : UIScene_Base {
		/// <summary>进度条</summary>
		[SerializeField]
		private UIProgressBar ProgressBar;
		/// <summary>进度条上的文本</summary>
		[SerializeField]
		private UILabel LblProgress;

		
		/// <summary>设置进度条的值，且设置为本的显示</summary>
		/// <param name="value"></param>
		public void SetProgressValue(float value) {
			ProgressBar.value = value;
			LblProgress.text = $"载入中...{(int) (value * 100)}%";
		}
	}
}