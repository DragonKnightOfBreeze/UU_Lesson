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
 * 挂载到相应的UI窗体上
 *
 * TODO：分离检查方法
 */
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UU_Lesson.Global;
using UnityEngine;
using UnityEngine.Serialization;
using UU_Lesson.Common;

namespace UU_Lesson.UI {
	/// <summary></summary>
	public class UIWindow_Login : UIWindow_Base {
		[SerializeField]
		private UIInput Inp_Nickname;
		[SerializeField]
		private UIInput Inp_Password;
		[SerializeField]
		private UILabel Tip;

		/// <inheritdoc />
		protected override void OnAwake() {
			ShowType = UIWindowShowStyle.CenterToBig;
		}

		/// <inheritdoc />
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
			//TODO：加一个Label作为提示
			//得到输入的昵称、密码
			string nickName = Inp_Nickname.value.Trim();
			string password = Inp_Password.value.Trim();

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
			string oldNickname = PlayerPrefs.GetString(GameConsts.Player_Nickname);
			string oldPassword = PlayerPrefs.GetString(GameConsts.Player_Password);
			if(nickName != oldNickname || password != oldPassword) {
				Tip.text = "昵称或密码错误！请重新输入。";
				return;
			}

			//准备切换场景
			ScenesMgr.GetInstance().NextSceneType = SceneType.Village;
			ScenesMgr.GetInstance().LoadToLoadingScene();
		}

		/// <summary>注册按钮的点击事件</summary>
		private void BtnToReg() {
			//关闭当前窗口
			Close();
			//指定下一个要打开的窗体（被关闭后自动打开）
			nextOpenWindow = UIWindowType.Panel_Register;
//			//打开注册窗口
//			UIWindowMgr.GetInstance().OpenWindow(UIWindowType.Panel_Register);

		}

		#endregion
	}
}