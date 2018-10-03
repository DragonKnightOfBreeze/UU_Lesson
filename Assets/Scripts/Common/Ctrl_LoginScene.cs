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

namespace UU_Lesson.Common {
	/// <summary>控制层，登录场景控制</summary>
	public class Ctrl_LoginScene : MonoBehaviour {
		//延迟时间
		private const float waitTime = 3f;

//		void Awake() {
//			GameObject go = ResourcesMgr.GetInstance().Load(ResourceType.UIScene_NGUI, GameConsts.PATH_UIScene_UIRoot_LoginScene);
//		}

//		void Awake() {
//			UISceneMgr.GetInstance().LoadUIScene(UISceneType.UIRoot_LoginScene);
//		}


		private IEnumerator Start() {
			//等待一段时间
			yield return new WaitForSeconds(waitTime);
			//加载UI场景
			UISceneMgr.GetInstance().LoadUIScene(UISceneType.UIRoot_LoginScene);
		}
	}
}