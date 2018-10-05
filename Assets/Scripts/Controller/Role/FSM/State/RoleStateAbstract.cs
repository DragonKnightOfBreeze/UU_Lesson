/*******
 * ［概述］
 * 
 * 
 * ［用法］
 * 
 * 
 * ［备注］ 
 * 
 * 
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 */

using UnityEngine;

namespace UU_Lesson.Controller.Role.FSM {
	/// <summary>角色状态的抽象基类</summary>
	public abstract class RoleStateAbstract {
		
		/// <summary>当前角色的有限状态机管理器，在实例化状态时赋值</summary>
		protected RoleFSMMgr CurRoleFSMMgr { get; private set; }

		protected AnimatorStateInfo CurStateInfo { get; set; }
		
		
		protected RoleStateAbstract(RoleFSMMgr roleFSMMgr) {
			CurRoleFSMMgr = roleFSMMgr;
		}

		/// <summary>进入状态</summary>
		public virtual void OnEnter() { }

		/// <summary>执行状态</summary>
		public virtual void Update() { }

		/// <summary>离开状态</summary>
		public virtual void OnExit() { }
	}
}