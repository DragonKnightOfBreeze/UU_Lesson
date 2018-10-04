/*******
 * ［概述］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * 初始化场景控制
 * 
 * ［功能］
 * 
 * 
 * ［思路］ 
 *
 *
 * ［备注］
 * 
 */

using System.Collections;
using UnityEngine;
using UU_Lesson.Global;

namespace UU_Lesson.Common {
	/// <summary> 
	///
	/// </summary>
	public class Ctrl_InitScene : MonoBehaviour {

		private const float waitTime = 4f;

		void Start() {
			StartCoroutine(LoadLogin());
		}


		private IEnumerator LoadLogin() {
			yield return new WaitForSeconds(waitTime);
			ScenesMgr.GetInstance().NextSceneType = SceneType.LoginScene;
			ScenesMgr.GetInstance().LoadToLoadingScene();
		}
	}
}