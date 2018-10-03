//核心层，事件触发监听器
//作用：
//可以监听任何一个指定的游戏对象

using UnityEngine;
using UnityEngine.EventSystems; //事件系统

namespace Kernel {
	public class EventTriggerListener : EventTrigger {
		public delegate void VoidDelegate(GameObject go); //定义委托

		//以下的都可以认为是事件
		public VoidDelegate onClick;
		public VoidDelegate onDown;
		public VoidDelegate onEnter;
		public VoidDelegate onUp;
		public VoidDelegate onExit;
		public VoidDelegate onSelect;
		public VoidDelegate onUndateSelected;


		/// <summary>得到“监听器”组件</summary>
		/// <param name="go">监听的游戏对象</param>
		/// <returns>监听器</returns>
		public static EventTriggerListener Get(GameObject go) {
			var listener = go.GetComponent<EventTriggerListener>();
			if(listener == null)
				listener = go.AddComponent<EventTriggerListener>();
			return listener;
		}


		//这些委托对应的方法到底是什么？
		//？？？

		//点击背景图片，调用的是重载后的OnPointerClick方法
		//接着该方法会在空值检查后调用onClick事件
		//而onClick事件在之后的脚本中又会注册加入一些方法，例如DisplayNextDialogRecord

		#region 重载后的方法

		public override void OnPointerClick(PointerEventData eventData) {
			if(onClick != null)
				onClick(gameObject);
			//简化后的代码在C# 4.0 中不可用
			//onClick?.Invoke(gameObject);
		}

		public override void OnPointerDown(PointerEventData eventData) {
			if(onDown != null)
				onDown(gameObject);
		}

		public override void OnPointerEnter(PointerEventData eventData) {
			if(onEnter != null)
				onEnter(gameObject);
		}

		public override void OnPointerUp(PointerEventData eventData) {
			if(onUp != null)
				onUp(gameObject);
		}

		public override void OnPointerExit(PointerEventData eventData) {
			if(onExit != null)
				onExit(gameObject);
		}

		public override void OnSelect(BaseEventData eventData) {
			if(onSelect != null)
				onSelect(gameObject);
		}

		public override void OnUpdateSelected(BaseEventData eventData) {
			if(onUndateSelected != null)
				onUndateSelected(gameObject);
		}

		#endregion
	}
}