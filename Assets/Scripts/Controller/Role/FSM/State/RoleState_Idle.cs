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

namespace UU_Lesson.Controller.Role.FSM {
	/// <summary> 
	///
	/// </summary>
	public class RoleState_Idle:RoleStateAbstract {
		
		public RoleState_Idle(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr) { }

		public override void OnEnter() {
			
			
			//自己的写法
//			CurRoleFSMMgr.CurRoleCtrl.MyAnimator.SetFloat("moveSpeed",0);
		}

		public override void Update() {
			
			//获取当前状态信息
			CurStateInfo = CurRoleFSMMgr.CurRoleCtrl.MyAnimator.GetCurrentAnimatorStateInfo(0);
		}

		public override void OnExit() {
		}
	}
}