//核心层，手工遮挡剔除脚本
//“模块加载”
//优化游戏性能

//对于触发器立方体的两边，分别触发，交换触发

//待完善：如果玩家在进入触发器后，又向后离开了触发器的情况
//待优化：检查玩家离开触发器时的朝向，如果这样取消加载后面的贴图

using UnityEngine;

//using Global;

namespace Kernel {
	internal class TriggerDisplayAndHide : MonoBehaviour {
		//标签：英雄
		public string TagNameByHero = "Player";
		//标签：需要显示的游戏对象
		public string TagNameByBeforeObject = "TagNameDisplayName";
		//标签：需要隐藏的游戏对象
		public string TagNameByAfterObject = "TagNameHideName";


		private static bool _IsPassed;

		private GameObject[] GoBeforeObjectArray; //需要显示的游戏对象
		private GameObject[] GoAfterObjectArray;  //需要隐藏的游戏对象

		private void Start() {
			//得到需要显示的游戏对象
			GoBeforeObjectArray = GameObject.FindGameObjectsWithTag(TagNameByBeforeObject);
			//得到需要隐藏的游戏对象
			GoAfterObjectArray = GameObject.FindGameObjectsWithTag(TagNameByAfterObject);
		}


		/// <summary>进入触发检测</summary>
		/// <param name="coll"></param>
		private void OnTriggerEnter(Collider coll) {
			//发现英雄（如果发现，就显示需要显示的游戏对象）
			if(coll.gameObject.tag == TagNameByHero) {
				if(!_IsPassed)
					foreach(var goItem in GoAfterObjectArray)
						goItem.SetActive(true);
				else
					foreach(var goItem in GoBeforeObjectArray)
						goItem.SetActive(true);
			}
		}


		/// <summary>离开触发检测</summary>
		/// <param name="coll"></param>
		private void OnTriggerExit(Collider coll) {
			//发现英雄（如果发现，就显示需要显示的游戏对象）
			if(coll.gameObject.tag == TagNameByHero) {
				if(!_IsPassed) {
					if(true)
						foreach(var goItem in GoBeforeObjectArray) {
							goItem.SetActive(false);
							_IsPassed = true;
						}
					//else {  //如果玩家返回了
					//	foreach (GameObject goItem in GoAfterObjectArray) {
					//		goItem.SetActive(false);
					//	}
					//}
				}
				else {
					if(true)
						foreach(var goItem in GoAfterObjectArray) {
							goItem.SetActive(false);
							_IsPassed = false;
						}
					//else {  //如果玩家返回了
					//	foreach (GameObject goItem in GoBeforeObjectArray) {
					//		goItem.SetActive(false);
					//	}
				}
			}
		}
	}
}