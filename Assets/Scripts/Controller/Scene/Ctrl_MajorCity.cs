/*******
 * ［概述］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * 
 * 
 * ［功能］
 * 
 * 
 * ［思路］ 
 *
 *
 * ［备注］
 * 需要主动挂载到空物体上。
 */

using UnityEngine;
using UU_Lesson.Global;
using UU_Lesson.UI;

namespace UU_Lesson.Common {
	/// <summary> 
	///
	/// </summary>
	public class Ctrl_MajorCity : MonoBehaviour {


		void Awake() {
			//加载主城UI
			UISceneMgr.GetInstance().LoadUIScene(UISceneType.UIRoot_MajorCity);
		}
	}
}