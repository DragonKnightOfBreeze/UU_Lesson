/*******
 * ［概述］
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
 * ［备注］
 * 
 */

using UnityEngine;
using UnityScript.Macros;

namespace UU_Lesson.UI {
	/// <summary> 
	///
	/// </summary>
	public class UIWindow_RoleInfo : UIWindow_Base {
		/// <summary>按钮点击的分支选择</summary>
		/// <param name="go">按钮对象</param>
		protected override void OnBtnClick(GameObject go) {
			switch(go.gameObject.name) {
				case "Btn_Close":
					Close();
					break;
			}
		}
	}
}