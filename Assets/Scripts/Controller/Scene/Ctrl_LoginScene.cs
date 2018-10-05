/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * 控制层，登录场景控制
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 *
 * 
 */
using System.Collections;
using UU_Lesson.Global;
using UnityEngine;
using UU_Lesson.UI;

namespace UU_Lesson.Controller.Scene {
	/// <summary>控制层，登录场景控制</summary>
	public class Ctrl_LoginScene : MonoBehaviour {

//		void Awake() {
//			GameObject go = ResourcesMgr.GetInstance().Load(ResourceType.UIScene_NGUI, GameConsts.PATH_UIScene_UIRoot_LoginScene);
//		}

//		void Awake() {
//			UISceneMgr.GetInstance().LoadUIScene(UISceneType.UIRoot_LoginScene);
//		}


		private void Start() {
			//加载UI场景
			UISceneMgr.GetInstance().LoadUIScene(UISceneType.UIRoot_LoginScene);
		}
	}
}