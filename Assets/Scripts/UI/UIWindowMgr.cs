/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 *
 * `------
 * 窗口UI管理器
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 单例模式，动态挂载
 * 
 * TODO：不把GlobalInit作为单例，而作为一个伴随着UI控制脚本挂载的脚本。
 */
using System;
using System.Collections.Generic;
using UnityEditor;
using UU_Lesson.Global;
using UnityEngine;
using UU_Lesson.Common;

namespace UU_Lesson.UI {
	/// <summary>UI窗口管理器</summary>
	public class UIWindowMgr : MonoBehaviour {
		#region ［单例模式］

		private static UIWindowMgr _Instance;

		public static UIWindowMgr GetInstance() {
			if(_Instance == null)
				_Instance = new GameObject("_" + nameof(UIWindowMgr)).AddComponent<UIWindowMgr>();
			return _Instance;
		}

		#endregion

		/// <summary>集合：当前显示的UI窗口（名字，UI窗口脚本）</summary>
		private Dictionary<UIWindowType, UIWindow_Base> _DicCurUIWindows;
//		/// <summary>
//		/// 集合：所有已加载的UI窗口（名字，UI窗口脚本）
//		/// <para>是否和资源管理器中的缓存集合重复了？</para>
//		/// </summary>
//		private Dictionary<UIWindowType, UIWindow_Base> _DicAllUIWindows;


		public int CurUIWindowsCount => _DicCurUIWindows.Count;

		private void Awake() {
			//初始化集合
//			_DicAllUIWindows = new Dictionary<UIWindowType, UIWindow_Base>();
			_DicCurUIWindows = new Dictionary<UIWindowType, UIWindow_Base>();
		}


		#region ［公共方法］

		/// <summary>加载UI窗口，并进行初始化设置</summary>
		/// <param name="type">UI窗口的类型</param>
		/// <returns></returns>
		public GameObject OpenWindow(UIWindowType type) {
			if(type == UIWindowType.None)
				return null;

			GameObject go;
			UIWindow_Base baseUIWindow;

			_DicCurUIWindows.TryGetValue(type, out baseUIWindow);

//			//如果存在，则表明已经打开，则返回空
//			if(baseUIWindow != null)
//				return null;

			//如果存在，只需设置一下层级深度即可
			if(baseUIWindow != null) {
				go = baseUIWindow.gameObject;
				//层级管理（每次打开一个新窗口，则深度增加）
				UILayerMgr.GetInstance().SetLayer(go);
				return go;
			}


			//加载窗口
			go = ResourcesMgr.GetInstance().Load(ResourceType.UIWindows_NGUI, type.ToString(), true);
			//空引用检查
			if(go == null)
				Debug.LogError("指定的UI窗口不存在！");
			//得到脚本组件
			baseUIWindow = go.GetComponent<UIWindow_Base>();
			//增加到当前UI窗口集合中去
			_DicCurUIWindows.Add(type, baseUIWindow);
			baseUIWindow.CurWindowType = type;


			//设置父节点
			Transform traParent = null;
			switch(baseUIWindow.ContainerType) {
				case UIWindowContainerType.TL:
					break;
				case UIWindowContainerType.TR:
					break;
				case UIWindowContainerType.BL:
					break;
				case UIWindowContainerType.BR:
					break;
				case UIWindowContainerType.Center:
					traParent = UISceneMgr.GetInstance().CurUIScene.Container_Center;
					break;
			}
			//设置父节点，初始化位置和缩放
			go.transform.parent = traParent;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;

			//默认不显示窗口
			NGUITools.SetActive(go, false);
			//按照显示类型，进行处理
			HandleWindowShow(baseUIWindow, true);


			//层级管理（每次打开一个新窗口，则深度增加）
			UILayerMgr.GetInstance().SetLayer(go);
			return go;
		}


		/// <summary>关闭窗口（实际上是销毁了）</summary>
		/// <param name="type">UI窗口的类型</param>
		public void CloseWindow(UIWindowType type) {
			UIWindow_Base baseUIWindow;
			_DicCurUIWindows.TryGetValue(type, out baseUIWindow);
			//如果不存在，则表明并没有打开，则直接返回
			if(baseUIWindow == null)
				return;

			//必须要先移除，后销毁
			//从已打开的窗口的字典中删去。
			_DicCurUIWindows.Remove(type);
			Destroy(baseUIWindow.gameObject);
			//HandleWindowShow(baseUIWindow,false);
		}

		/// <summary>销毁窗口</summary>
		/// <param name="type">UI窗口的类型</param>
		public void DestroyWindow(UIWindowType type) {
			UIWindow_Base baseUIWindow;

			_DicCurUIWindows.TryGetValue(type, out baseUIWindow);
			if(baseUIWindow == null)
				return;

			_DicCurUIWindows.Remove(type);
			Destroy(baseUIWindow.gameObject);

		}

		#endregion


		#region ［私有方法］

		/// <summary>按照窗口的显示类型，进行处理</summary>
		/// <param name="baseUIWindow"></param>
		/// <param name="isOpen">打开/关闭</param>
		private void HandleWindowShow(UIWindow_Base baseUIWindow, bool isOpen) {

			switch(baseUIWindow.ShowType) {
				case UIWindowShowStyle.Normal:
					ShowNormal(baseUIWindow, isOpen);
					break;
				case UIWindowShowStyle.CenterToBig:
					ShowCenterToBig(baseUIWindow, isOpen);
					break;
				case UIWindowShowStyle.FromTop:
					ShowFromDir(baseUIWindow, Direction4.Up, isOpen);
					break;
				case UIWindowShowStyle.FromDown:
					ShowFromDir(baseUIWindow, Direction4.Down, isOpen);
					break;
				case UIWindowShowStyle.FromLeft:
					ShowFromDir(baseUIWindow, Direction4.Left, isOpen);
					break;
				case UIWindowShowStyle.FromRight:
					ShowFromDir(baseUIWindow, Direction4.Right, isOpen);
					break;
			}
		}

		#endregion


		#region ［私有方法：各种显示效果］

		/// <summary>显示效果：普通</summary>
		/// <param name="baseUIWindow"></param>
		/// <param name="isOpen">打开/关闭</param>
		private void ShowNormal(UIWindow_Base baseUIWindow, bool isOpen) {
			/* 自己的写法 */
//			NGUITools.SetActive(go, isOpen);

			/* 老师的写法 */
			if(isOpen)
				NGUITools.SetActive(baseUIWindow.gameObject, true);
			else
				DestroyWindow(baseUIWindow.CurWindowType);

		}

		/// <summary>显示效果：中间变大
		///     <remarks>需要在空游戏对象上挂载GlobalInit脚本</remarks>
		///     ></summary>
		/// <param name="baseUIWindow"></param>
		/// <param name="isOpen"></param>
		private void ShowCenterToBig(UIWindow_Base baseUIWindow, bool isOpen) {
			GameObject go = baseUIWindow.gameObject;
			TweenScale ts = go.GetComponent<TweenScale>() ?? go.AddComponent<TweenScale>();

			//动画曲线（这个真的要单例吗？）
			ts.animationCurve = GlobalInit.Instance.UIAnimationCurve;
			//从0到1进行缩放
			ts.from = Vector3.zero;
			ts.to = Vector3.one;
			//过程需要的时间
			ts.duration = baseUIWindow.Duration;

			/* 自己的写法 */
//			NGUITools.SetActive(go, isOpen);
//			if(!isOpen)
//				ts.Play(baseUIWindow);

			/* 老师的写法*/
			ts.SetOnFinished(() => {
				if(!isOpen)
					DestroyWindow(baseUIWindow.CurWindowType);
			});
			NGUITools.SetActive(go, true);
			if(!isOpen)
				ts.Play(baseUIWindow);
		}

		/// <summary>显示效果：从不同的方向加载</summary>
		/// <remarks>需要在空游戏对象上挂载GlobalInit脚本</remarks>
		/// >
		/// <param name="baseUIWindow"></param>
		/// <param name="dir"></param>
		/// <param name="isOpen"></param>
		private void ShowFromDir(UIWindow_Base baseUIWindow, Direction4 dir, bool isOpen) {
			GameObject go = baseUIWindow.gameObject;
			TweenPosition tp = go.GetComponent<TweenPosition>() ?? go.AddComponent<TweenPosition>();

			//动画曲线（这个真的要单例吗？）
			tp.animationCurve = GlobalInit.Instance.UIAnimationCurve;
			Vector3 fromDir = Vector3.zero;
			switch(dir) {
				case Direction4.Up:
					fromDir = new Vector3(0, 1000, 0);
					break;
				case Direction4.Down:
					fromDir = new Vector3(0, -1000, 0);
					break;
				case Direction4.Left:
					fromDir = new Vector3(-1400, 0, 0);
					break;
				case Direction4.Right:
					fromDir = new Vector3(1400, 0, 0);
					break;
			}
			//移动
			tp.from = fromDir;
			tp.to = Vector3.one;
			//过程需要的时间
			tp.duration = baseUIWindow.Duration;

			/* 自己的写法 */
//			NGUITools.SetActive(baseUIWindow.gameObject, isOpen);
//			if(!isOpen)
//				tp.Play(baseUIWindow);

			/* 老师的写法*/
			tp.SetOnFinished(() => {
				if(!isOpen)
					DestroyWindow(baseUIWindow.CurWindowType);
			});
			NGUITools.SetActive(go, true);
			if(!isOpen)
				tp.Play(baseUIWindow);
		}

		#endregion
	}
}