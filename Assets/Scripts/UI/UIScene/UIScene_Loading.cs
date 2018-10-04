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
 * 
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
		private UILabel Lab_Progress;
		/// <summary>
		/// 发光图（相当于进度条的“头部”，随进度条向右推进）
		/// </summary>
		[SerializeField]
		private UISprite Spr_ProgressLight;

		
		/// <summary>设置进度条的值，文本的显示，发光图的推进</summary>
		/// <param name="value"></param>
		public void SetProgressValue(float value) {
			ProgressBar.value = value;
			Lab_Progress.text = $"载入中...{(int) (value * 100)}%";
			
			//发光图的父物体的位置在进度条的开始处，(675,0,0)为进度条的结束处
			//发光图相当于进度条的“头”，随着进度条的向右推进而推进
			Spr_ProgressLight.transform.localPosition = new Vector3(675f*value,0,0);
		}
	}
}