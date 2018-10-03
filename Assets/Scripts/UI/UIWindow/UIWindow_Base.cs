/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 *
 * UI窗体管理器
 * 
 * ［功能］
 * 管理UI窗体的加载、打开、关闭、销毁、动画效果等。
 * 这里实际上是没有缓存机制的。
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 
 */
using UnityEngine;
using UU_Lesson.Global;

namespace UU_Lesson.UI {
	/// <summary>所有UI窗口的基类</summary>
	public class UIWindow_Base : UI_Base {
		//定义UI窗口的默认属性

		/// <summary>UI窗口的挂载类型</summary>
		public UIWindowContainerType ContainerType = UIWindowContainerType.Center;

		/// <summary>UI窗口的显示风格</summary>
		public UIWindowShowStyle ShowType = UIWindowShowStyle.Normal;

		/// <summary>打开或关闭的动画效果的持续时间</summary>
		public float Duration = 0.2f;

		/// <summary>当前UI窗体的类型</summary>
		[HideInInspector]
		public UIWindowType CurWindowType;

		/// <summary>下一个要打开的窗口</summary>
		[HideInInspector]
		public UIWindowType nextOpenWindow = UIWindowType.None;

		/// <summary>关闭这个窗口（实际上会销毁，而不是不启用）</summary>
		protected virtual void Close() {
			UIWindowMgr.GetInstance().CloseWindow(CurWindowType);
		}

		/// <summary>这个窗口被销毁时，启动的方法</summary>
		protected override void BeforeOnDestroy() {
			//检查窗口，重置深度
			UILayerMgr.GetInstance().CheckOpenWindow();

			if(nextOpenWindow == UIWindowType.None)
				return;
			UIWindowMgr.GetInstance().OpenWindow(nextOpenWindow);


		}
	}
}