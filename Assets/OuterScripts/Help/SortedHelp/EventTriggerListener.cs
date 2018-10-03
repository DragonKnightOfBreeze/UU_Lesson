/*******
 * ［前置］
 * 帮助脚本
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 事件触发监听器
 * 
 * ［功能］
 * 实现对于任何游戏对象的事件监听处理
 * 例如：让UI图片响应点击事件
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * //给任意一个UI对象的点击事件注册指定的方法。
 * if (goButton != null) {
 *		EventTriggerListener.GetListener(goButton).onClick += Method1;
 *
 */
using UnityEngine;
using UnityEngine.EventSystems;

namespace SortedHelp {
	/// <summary>事件触发监听器</summary>
	public class EventTriggerListener : EventTrigger {
		//定义空委托
		public delegate void VoidDelegate(GameObject go);

		//定义各种对象互动触发事件
		public VoidDelegate onClick;
		public VoidDelegate onDown;
		public VoidDelegate onEnter;
		public VoidDelegate onExit;
		public VoidDelegate onUp;
		public VoidDelegate onSelect;
		public VoidDelegate onUpdateSelect;


		/// <summary>得到指定游戏对象的“监听器”组件，如果没有就添加</summary>
		/// <param name="go">监听的游戏对象</param>
		/// <returns>监听器</returns>
		public static EventTriggerListener GetListener(GameObject go) {
			var listener = go.GetComponent<EventTriggerListener>();
			if(listener == null)
				listener = go.AddComponent<EventTriggerListener>();
			return listener;
		}


		/// <summary>重载后的点击方法</summary>
		/// <param name="eventData"></param>
		public override void OnPointerClick(PointerEventData eventData) {
			onClick?.Invoke(gameObject);
		}


		/// <summary>重载后的按下方法</summary>
		/// <param name="eventData"></param>
		public override void OnPointerDown(PointerEventData eventData) {
			onDown?.Invoke(gameObject);
		}


		/// <summary>重载后的鼠标进入方法</summary>
		/// <param name="eventData"></param>
		public override void OnPointerEnter(PointerEventData eventData) {
			onEnter?.Invoke(gameObject);
		}


		/// <summary>重载后的鼠标离开方法</summary>
		/// <param name="eventData"></param>
		public override void OnPointerExit(PointerEventData eventData) {
			onExit?.Invoke(gameObject);
		}


		/// <summary>重载后的鼠标按键抬起方法</summary>
		/// <param name="eventData"></param>
		public override void OnPointerUp(PointerEventData eventData) {
			onUp?.Invoke(gameObject);
		}


		/// <summary>重载后的选择方法</summary>
		/// <param name="eventBaseData"></param>
		public override void OnSelect(BaseEventData eventBaseData) {
			onSelect?.Invoke(gameObject);
		}

		/// <summary>重载后的更新选择方法</summary>
		/// <param name="eventBaseData"></param>
		public override void OnUpdateSelected(BaseEventData eventBaseData) {
			onUpdateSelect?.Invoke(gameObject);
		}
	}
}