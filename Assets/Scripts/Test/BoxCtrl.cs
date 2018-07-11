//标题：
//
//功能：
//
//思路：
//
//
//用法：
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Kernel;
//using Global;

namespace UU_Lesson.Test {
	/// <summary>
	/// 
	/// </summary>
	public class BoxCtrl : MonoBehaviour {

		/// <summary>
		/// 移动的目标点
		/// </summary>
		private Vector3 m_TargetPos = Vector3.zero;

		void Update(){
			//点击屏幕
			//鼠标抬起事件，0是鼠标左键，1是鼠标右键
			//touchCount表示触摸屏幕的触摸点数
			if (Input.GetMouseButtonUp(0) || Input.touchCount == 1) {
				//Debug.Log("点击屏幕了");

				//从摄像机发射一个射线，获取鼠标点击的坐标
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				//如果射线碰撞到地面，则返回一个目标点
				if (Physics.Raycast(ray, out hitInfo)) {
					if (hitInfo.collider.gameObject.name.Equals("Ground", StringComparison.CurrentCultureIgnoreCase)) {
						m_TargetPos = hitInfo.point;
						
						Debug.DrawLine(Camera.main.transform.position, m_TargetPos);
					}
				}
			}

			
		}

	}
}