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

using System.Collections.Generic;
using UU_Lesson.Global;

namespace UU_Lesson.Controller.Role.FSM {
	/// <summary> 
	/// FSM管理器
	/// </summary>
	public class RoleFSMMgr  {

		/// <summary>
		/// 当前角色控制脚本
		/// </summary>
		public RoleCtrl CurRoleCtrl { get; private set; }

		/// <summary>
		/// 当前角色状态的枚举
		/// </summary>
		public RoleState curRoleStateEnum { get; private set; } = RoleState.None;

		/// <summary>
		/// 当前角色状态
		/// </summary>
		private RoleStateAbstract curRoleState;

		public Dictionary<RoleState, RoleStateAbstract> RoleStateDict;
		
		
		public RoleFSMMgr(RoleCtrl curRoleCtrl) {
			CurRoleCtrl = curRoleCtrl;
			RoleStateDict = new Dictionary<RoleState, RoleStateAbstract> {
				[RoleState.Idle] = new RoleState_Idle(this),
				[RoleState.Run] = new RoleState_Run(this),
				[RoleState.NormalAtk] = new RoleState_Attack(this),
				[RoleState.Hurt] = new RoleState_Hurt(this),
				[RoleState.Die] = new RoleState_Die(this)
			};

			//如果字典中可以对应，则当前状态即为当前状态枚举对应的状态。
			if(RoleStateDict.ContainsKey(curRoleStateEnum))
				curRoleState = RoleStateDict[curRoleStateEnum];
		}

		/// <summary>
		/// 执行状态（每帧）
		/// </summary>
		public void UpdateState() {
			curRoleState?.Update();
		}

		/// <summary>
		/// 切换状态
		/// </summary>
		/// <param name="newState">新的状态</param>
		public void ChangeState(RoleState newState) {
			if(curRoleStateEnum == newState)
				return;
			
			//调用先前状态的离开方法
			curRoleState?.OnExit();
			//更改当前状态枚举
			curRoleStateEnum = newState;
			//更改当前状态
			curRoleState = RoleStateDict[newState];
			//调用新状态的进入方法
			curRoleState.OnEnter();
		}
		
	}
}