//核心层，通用触发脚本
//功能：
//1. NPC、敌人触发对话
//2.存盘与继续
//3.加载与启用特定的脚本（例如：产生敌人）
//4.触发UI“对话框”

//注意：方法还需要另外注册到事件中

using UnityEngine;

namespace Kernel {
	public enum CommonTriggerType {
		None,
		NPC1_Dialog, //NPC1对话
		NPC2_Dialog, //NPC2对话
		NPC3_Dialog, //NPC3对话

		Enemy1_Spawn,    //敌人生成1
		Enemy2_Spawn,    //敌人生成2
		Enemy3_Spawn,    //敌人生成3
		EnemyBOSS_Spawn, //敌人生成：BOSS区域

		EnableScript1, //启用脚本1
		EnableScript2, //启用脚本2

		Enemy_Show1,

		SaveDataOrScenes,    //保持
		LoadDataOrScenes,    //加载
		ActiveConfimWindows, //确认框
		ActiveDialogWindows  //对话框
	}

	/// <summary>委托：通用触发</summary>
	/// <param name="ctt"></param>
	public delegate void del_CommonTrigger(CommonTriggerType ctt);

	public class TriggerCommonEvent : MonoBehaviour {
		//定义事件
		public static event del_CommonTrigger eve_CommonTrigger;
		//对话类型
		public CommonTriggerType TriggerType = CommonTriggerType.None;
		//英雄标签名称
		private readonly string Tag_Player = "Player";

		/// <summary>触发进入检测</summary>
		/// <param name="coll"></param>
		private void OnTriggerEnter(Collider coll) {
			if(coll.gameObject.tag == Tag_Player)
				if(eve_CommonTrigger != null)
					eve_CommonTrigger(TriggerType);
		}
	}
}