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
 * 挂载到相应的UI窗口上
 *
 * TODO：分离检查方法
 * TODO：更准确的格式检查
 */
using UU_Lesson.Global;
using UnityEngine;
using UU_Lesson.Common;

namespace UU_Lesson.UI {
	/// <summary></summary>
	public class UIWindow_Register : UIWindow_Base {
		[SerializeField]
		private UIInput Inp_NickName;
		[SerializeField]
		private UIInput Inp_Password;
		[SerializeField]
		private UIInput Inp_RePassword;
		[SerializeField]
		private UILabel Tip;

		/// <summary>按钮点击的分支选择</summary>
		/// <param name="go"></param>
		protected override void OnBtnClick(GameObject go) {
			switch(go.name) {
				case "Btn_Login":
					BtnToLogin();
					break;
				case "Btn_Register":
					BtnToReg();
					break;
			}
		}


		#region ［按钮事件］

		private void BtnToLogin() {
			//关闭当前窗口
			Close();
			//指定下一个要打开的窗口（被关闭后自动打开）
			nextOpenWindow = UIWindowType.Panel_Login;
//			//打开注册窗口
//			UIWindowMgr.GetInstance().OpenWindow(UIWindowType.Panel_Login);
		}


		private void BtnToReg() {
			//得到输入的昵称、密码、确认密码
			string nickName = Inp_NickName.value.Trim();
			string password = Inp_Password.value.Trim();
			string rePassword = Inp_RePassword.value.Trim();

			//非空检查
			if(string.IsNullOrEmpty(nickName)) {
				Tip.text = "昵称不能为空！请重新输入。";
				return;
			}
			if(string.IsNullOrEmpty(password)) {
				Tip.text = "密码不能为空！请重新输入。";
				return;
			}
			//匹配检查
			if(nickName.Length > 8) {
				Tip.text = "昵称格式错误！请重新输入。";
				return;
			}
			if(password.Length > 16) {
				Tip.text = "密码格式错误！请重新输入。";
				return;
			}
			if(rePassword != password) {
				Tip.text = "密码不一致！请重新输入。";
				return;
			}

			//注册用户昵称和密码
			PlayerPrefs.SetString(GameConsts.Player_Nickname, nickName);
			PlayerPrefs.SetString(GameConsts.Player_Password, password);

			//准备切换场景
			ScenesMgr.GetInstance().NextSceneType = SceneType.Scene_Village1;
			ScenesMgr.GetInstance().LoadToLoadingScene();
		}

		#endregion
	}
}