/*******
 * ［概述］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * 视图层，主城页面UI
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
using UU_Lesson.Global;
using UU_Lesson.UI;

namespace UU_Lesson.UI {
	/// <summary> 
	///
	/// </summary>
	public class UIScene_MajorCity : UIScene_Base {
		/// <summary>按钮点击的分支选择</summary>
		/// <param name="go">按钮对象</param>
		protected override void OnBtnClick(GameObject go) {
			switch(go.gameObject.name) {
				//角色头像
				case "Btn_HeadImg":
					OpenRoleInfo();
					break;
					
			}
		}

		private void OpenRoleInfo() {
			UIWindowMgr.GetInstance().OpenWindow(UIWindowType.Panel_RoleInfo);
		}
	}
}