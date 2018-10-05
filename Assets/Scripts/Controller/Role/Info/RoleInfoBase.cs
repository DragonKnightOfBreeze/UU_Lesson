using System;
using System.Collections;
using System.Collections.Generic;

namespace UU_Lesson.Controller.Role.Info {
	/// <summary>
	/// 角色信息的基类
	/// </summary>
	public class InfoBase {

		/// <summary>
		/// 角色的编号（本地）
		/// </summary>
		public int RoleID;

		/// <summary>
		/// 角色的服务器编号
		/// </summary>
		public int RoleServerID;
		
		/// <summary>
		/// 角色的名称（统一化的）
		/// </summary>
		public string Name;

		/// <summary>
		/// 最大生命值
		/// </summary>
		public int MaxHP;

		/// <summary>
		/// 当前生命值
		/// </summary>
		public int CurHP;

		/// <summary>
		/// 最大魔法值
		/// </summary>
		public int MaxMP;

		/// <summary>
		/// 当前生命值
		/// </summary>
		public int CurMP;

	}
}