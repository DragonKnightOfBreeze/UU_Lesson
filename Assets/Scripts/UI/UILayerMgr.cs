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
 * 单例模式，动态挂载
 */
using UnityEngine;

namespace UU_Lesson.UI {
	/// <summary></summary>
	public class UILayerMgr : MonoBehaviour {
		#region ［单例模式］

		private static UILayerMgr _Instance;

		/// <summary>得到单例实例，动态挂载</summary>
		/// <returns>单例实例</returns>
		public static UILayerMgr GetInstance() {
			if(_Instance == null)
				_Instance = new GameObject("_" + nameof(UILayerMgr)).AddComponent<UILayerMgr>();
			return _Instance;
		}

		#endregion

		/// <summary>UIPanel的层级深度</summary>
		private int uiPanelDepth = 50;

		/// <summary>设置层级（增加深度，最少增加UIPanelDepth的深度）</summary>
		/// <param name="go"></param>
		public void SetLayer(GameObject go) {
			uiPanelDepth++;

			UIPanel[] panelArray = go.GetComponentsInChildren<UIPanel>();
			if(panelArray == null || panelArray.Length <= 0)
				return;
			for(int i = 0; i < panelArray.Length; i++) //增加深度
				panelArray[i].depth += uiPanelDepth;
		}

		/// <summary>重置深度（其他窗口已经被关闭了）</summary>
		public void ResetDepth() {
			uiPanelDepth = 50;
		}

		/// <summary>检查窗口的数量，如果为0，则重置深度</summary>
		public void CheckOpenWindow() {
			if(UIWindowMgr.GetInstance().CurUIWindowsCount == 0)
				ResetDepth();
		}
	}
}