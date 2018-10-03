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
 * 
 */
using UnityEngine;

namespace UU_Lesson.UI {
	/// <summary></summary>
	public class UI_Base : MonoBehaviour {
		/// <summary>UI的名字</summary>
		[HideInInspector]
		public string UIName;


		private void Awake() {
			OnAwake();
		}

		private void Start() {
			GetButtons();
			OnStart();
		}

		private void OnDestroy() {
			BeforeOnDestroy();
		}

		#region ［虚方法］

		/// <summary>在子类中重载的方法，在父类的Awake()中调用</summary>
		protected virtual void OnAwake() { }

		/// <summary>在子类中重载的方法，在父类的start()中调用</summary>
		protected virtual void OnStart() { }

		/// <summary>在子类中重载的方法，在父类的OnDestroy()中调用</summary>
		protected virtual void BeforeOnDestroy() { }

		/// <summary>按钮点击的分支选择</summary>
		/// <param name="go">按钮对象</param>
		protected virtual void OnBtnClick(GameObject go) { }

		#endregion


		private void GetButtons() {
			//得到按钮数组
			UIButton[] Btn_Arr = gameObject.GetComponentsInChildren<UIButton>(true);
			for(int i = 0; i < Btn_Arr.Length; i++)
				UIEventListener.Get(Btn_Arr[i].gameObject).onClick = OnBtnClick;
		}
	}
}