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
using System;
using UnityEngine;


//using Kernel;
//using Global;

namespace UU_LessonTest {
	/// <summary></summary>
	public class BoxCtrl2 : MonoBehaviour {
		public Action<GameObject> OnHit;


		public void Hit() {
			OnHit?.Invoke(gameObject);
		}
	}
}