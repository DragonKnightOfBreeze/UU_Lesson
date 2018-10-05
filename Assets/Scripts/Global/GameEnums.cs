/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 公共层，游戏枚举
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 枚举类型应该和预设的名称对应上
 * 比定义一堆常量更好的解决方式
 */


//using Kernel;
//using Global;

namespace UU_Lesson.Global {
	/// <summary>游戏枚举</summary>
	public static class GameEnums { }


	/// <summary>场景类型</summary>
	public enum SceneType {
		/// <summary>初始化场景</summary>
		InitScene,
		/// <summary>登录场景</summary>
		LoginScene,
		/// <summary>加载场景</summary>
		LoadingScene,
		/// <summary>村庄1</summary>
		Scene_Village1
	}


	/// <summary>方向（上下左右）</summary>
	public enum Direction4 {
		Up,
		Down,
		Left,
		Right
	}


	/// <summary>缩放方式</summary>
	public enum ZoomType {
		In,
		Out
	}


	/// <summary>资源类型</summary>
	public enum ResourceType {
		/// <summary>场景UI</summary>
		UIScenes_NGUI,
		/// <summary>窗口UI</summary>
		UIWindows_NGUI,
		/// <summary>角色</summary>
		Roles,
		/// <summary>特效</summary>
		Effects
	}


	#region ［UI相关］

	/// <summary>UI窗口的类型</summary>
	public enum UIWindowType {
		/// <summary>未设置</summary>
		None,
		/// <summary>登录窗口</summary>
		Panel_Login,
		/// <summary>注册窗口</summary>
		Panel_Register,
		/// <summary>角色信息窗口</summary>
		Panel_RoleInfo
	}


	/// <summary>UI场景的类型</summary>
	public enum UISceneType {
		None,
		/// <summary>登录场景</summary>
		UIRoot_LoginScene,
		/// <summary>加载场景</summary>
		UIRoot_LoadingScene,
		/// <summary>主城场景</summary>
		UIRoot_MajorCity
	}


	/// <summary>UI窗口容器的类型</summary>
	public enum UIWindowContainerType {
		/// <summary>左上方</summary>
		TL,
		/// <summary>右上方</summary>
		TR,
		/// <summary>左下方</summary>
		BL,
		/// <summary>右下方</summary>
		BR,
		/// <summary>中间</summary>
		Center
	}


	/// <summary>窗口的显示风格</summary>
	public enum UIWindowShowStyle {
		/// <summary>正常打开</summary>
		Normal,
		/// <summary>从中间放大</summary>
		CenterToBig,
		/// <summary>从上往下</summary>
		FromTop,
		/// <summary>从下往上</summary>
		FromDown,
		/// <summary>从左向右</summary>
		FromLeft,
		/// <summary>从右向左</summary>
		FromRight
	}

	#endregion


	#region ［角色相关］

	/// <summary>角色类型</summary>
	public enum RoleType {
		/// <summary>未定义</summary>
		None,
		/// <summary>玩家（当前玩家，本地玩家）</summary>
		Player,
		/// <summary>玩家（其他玩家，联机玩家）</summary>
		OtherPlayer,
		/// <summary>敌人（普通敌人）</summary>
		Enemy,
		/// <summary>BOSS（强大的敌人）</summary>
		Boss,
		/// <summary>NPC（非玩家友好角色）</summary>
		NPC
	}

	/// <summary>角色状态</summary>
	public enum RoleState {
		/// <summary>未定义</summary>
		None,
		/// <summary>待机状态</summary>
		Idle,
		/// <summary>奔跑状态</summary>
		Run,
		/// <summary>普通攻击状态</summary>
		NormalAtk,
		/// <summary>受伤状态</summary>
		Hurt,
		/// <summary>死亡状态</summary>
		Die,
		/// <summary>使用技能状态</summary>
		Skill
	}

	/// <summary>
	/// 角色动画状态的名字（这个真的有必要吗？）
	/// </summary>
	public enum RoleAnimatorStateName {
		Idle_Normal,
		Idle_Fight,
		Run,
		Hurt,
		Die,
		NormalAtk1,
		NormalAtk2,
		NormalAtk3
	}

	/// <summary>
	/// FSM状态转换条件
	/// </summary>
	public enum FSMCondition {
		ToIdleNormal,
		TOIdleFight,
		ToRun,
		
	}
	
	#endregion
}