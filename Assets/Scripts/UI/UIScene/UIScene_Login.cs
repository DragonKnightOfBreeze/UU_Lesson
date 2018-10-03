/* 
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * 视图层，登录界面UI
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 挂载到相应的UI根节点上
 */
using System.Collections;
using UU_Lesson.Global;
using UnityEngine;

namespace UU_Lesson.UI.UIScene {
	/// <summary>视图层，登录界面UI</summary>
	public class UIScene_Login : UIScene_Base {
//		protected override void OnAwake() {
//			//加载登录窗体
//			var go = UIWindowMgr.GetInstance().LoadUIWindow(UIWindowType.Panel_Login);
//		} 


		private const float waitTime = 2f;


		protected override void OnStart() {
			StartCoroutine(OpenLoginWindow());
		}


		private IEnumerator OpenLoginWindow() {
			//延迟一定时间
			yield return new WaitForSeconds(waitTime);
			//加载UI场景
			GameObject go = UIWindowMgr.GetInstance().OpenWindow(UIWindowType.Panel_Login);
		}
	}
}