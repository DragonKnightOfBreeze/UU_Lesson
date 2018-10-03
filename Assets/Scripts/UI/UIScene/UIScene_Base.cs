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
	/// <summary>所有场景UI的基类</summary>
	public class UIScene_Base : UI_Base {
		/// <summary>容器 居中</summary>
		[SerializeField]
		public Transform Container_Center;

		//private static BaseUIScene _Instance;

		//public  static BaseUIScene GetSubInstance() {
		//	if (_Instance == null) {
		//		_Instance = new BaseUIScene();
		//	}
		//	return _Instance;
		//}


//		/// <summary>
//		/// 容器：居中
//		/// </summary>
//		[SerializeField]
//		public Transform Container_Center;

		////定义UI场景的默认属性
		///// <summary>
		///// UI场景的类型
		///// </summary>
		//public UISceneType CurUISceneType = UISceneType.None;
	}
}