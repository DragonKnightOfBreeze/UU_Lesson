/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 公共层，游戏枚举
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

namespace UU_Lesson.Global {
	/// <summary>游戏枚举</summary>
	public static class GameEnums {
		public static string Tostring(ResourceType enumMember) {
			var str = string.Empty;
			switch(enumMember) {
				case ResourceType.UIScene_NGUI:
					str = "Prefabs/UIScenes((NGUI)/";
					break;
				case ResourceType.UIWindow_NGUI:
					str = "Prefabs/UIWindows(NGUI)/";
					break;
				case ResourceType.Role:
					str = "Prefabs/Characters/";
					break;
				case ResourceType.Effect:
					str = "Prefabs/Effects/";
					break;
			}
			return str;
		}
	}


	/// <summary>场景类型</summary>
	public enum SceneType {
		/// <summary>登录场景</summary>
		LoginScene,
		LoadingScene,
		Village
	}


	/// <summary>方向（上下左右）</summary>
	public enum Direction4 {
		Up,
		Down,
		Left,
		Right
	}


	/// <summary>缩放方式</summary>
	public enum ZoomType {
		In,
		Out
	}


	/// <summary>资源类型</summary>
	public enum ResourceType {
		/// <summary>场景UI</summary>
		UIScene_NGUI,
		/// <summary>窗口UI</summary>
		UIWindow_NGUI,
		/// <summary>角色</summary>
		Role,
		/// <summary>特效</summary>
		Effect
	}


	#region ［UI相关］

	/// <summary>UI窗体的类型</summary>
	public enum UIWindowType {
		/// <summary>未设置</summary>
		None,
		/// <summary>登录窗体</summary>
		Panel_Login,
		/// <summary>注册窗体</summary>
		Panel_Register
	}


	/// <summary>UI场景的类型</summary>
	public enum UISceneType {
		None,
		/// <summary>登录场景</summary>
		UIRoot_LoginScene,
		/// <summary>加载场景</summary>
		UIRoot_LoadingScene,
		/// <summary>主城场景</summary>
		UIRoot_MajorCity
	}


	/// <summary>UI窗体容器的类型</summary>
	public enum UIWindowContainerType {
		/// <summary>左上方</summary>
		TL,
		/// <summary>右上方</summary>
		TR,
		/// <summary>左下方</summary>
		BL,
		/// <summary>右下方</summary>
		BR,
		/// <summary>中间</summary>
		Center
	}


	/// <summary>窗口的显示风格</summary>
	public enum UIWindowShowStyle {
		/// <summary>正常打开</summary>
		Normal,
		/// <summary>从中间放大</summary>
		CenterToBig,
		/// <summary>从上往下</summary>
		FromTop,
		/// <summary>从下往上</summary>
		FromDown,
		/// <summary>从左向右</summary>
		FromLeft,
		/// <summary>从右向左</summary>
		FromRight
	}

	#endregion
}