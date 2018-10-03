/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 游戏常量类
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


//using Kernel;
//using Global;

using System.CodeDom;

namespace UU_Lesson.Global {
	/// <summary>游戏常量类</summary>
	public static class GameConsts {
		#region ［持久化变量名］

		public const string Player_Nickname = "Player_Nickname";
		public const string Player_Password = "Player_Password";
		

		#endregion
		
		
		#region ［场景名称］

		public const string SCENENAME_LoginScene = "LoginScene";
		public const string SceneNAME_LoadingScene = "LoadingScene";
		public const string SCENENAME_Village = "Village";

		#endregion


		#region ［资源路径］

		//public const string PATH_UIScene_UIRoot_LoginScene = "Prefabs/UIScenes(NGUI)/UIRoot_LoginScene";
		/// <summary>路径：UI场景，登录界面</summary>
		public const string PATH_UIScene_UIRoot_LoginScene = "UIRoot_LoginScene";
		/// <summary>路径：UI场景，加载界面</summary>
		public const string PATH_UIScene_UIRoot_LoadingScene = "UIRoot_LoadingScene";
		/// <summary>路径：UI场景，主城</summary>
		public const string PATH_UIScene_UIRoot_MajorCity = "UIRoot";

		/// <summary>路径：UI窗口，登录面板</summary>
		public const string PATH_UIWindow_Panel_Login = "Panel_Login";
		/// <summary>路径：UI窗口，注册面板</summary>
		public const string PATH_UIWindow_Panel_Register = "Panel_Register";

		#endregion
	}
}