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
 * 挂载到物体GO上
 */

using System;
using UnityEngine;

//using Kernel;
//using Global;

namespace UU_LessonTest {
	/// <summary></summary>
	public class BoxCtrl : MonoBehaviour {
		/// <summary>移动的目标点</summary>
		private Vector3 m_TargetPos = Vector3.zero;


		/// <summary>速度 强制可在Inspector中动态修改私有字段</summary>
		[SerializeField]
		private float m_Speed = 10f;

		/// <summary>每帧方法</summary>
		private void Update() {

			#region ［点击屏幕向前移动］

			//点击屏幕
			//鼠标抬起事件，0是鼠标左键，1是鼠标右键
			//touchCount表示触摸屏幕的触摸点数
			if(Input.GetMouseButtonUp(0) || Input.touchCount == 1) {
				//Debug.Log("点击屏幕了");

				//从摄像机发射一个射线，获取鼠标点击的坐标
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				//如果射线碰撞到地面，则返回一个目标点
				if(Physics.Raycast(ray, out hitInfo))
					if(hitInfo.collider.gameObject.name.Equals("Ground", StringComparison.CurrentCultureIgnoreCase))
						m_TargetPos = hitInfo.point;
			}
			//如果目标点不是原点，就进行移动
			if(m_TargetPos != Vector3.zero) {
				//调试时高亮显示连线
				Debug.DrawLine(Camera.main.transform.position, m_TargetPos);
				//移动
				//这个判断是为了移动后的后续抖动
				if(Vector3.Distance(m_TargetPos, transform.position) > 0.1f) {
					transform.LookAt(m_TargetPos);
					transform.Translate(Vector3.forward * Time.deltaTime * m_Speed);
				}
			}

			#endregion ［点击屏幕向前移动］

		}
	}
}