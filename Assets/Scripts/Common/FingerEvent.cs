using System;
using UnityEngine;
using UU_Lesson.Global;

namespace UU_Lesson.Common {
	public class FingerEvent : MonoBehaviour {
		//单例实例
		public static FingerEvent Instance;

		/// <summary>滑动委托</summary>
		public Action<Direction4> OnFingerDrag;
		/// <summary>缩放委托</summary>
		public Action<ZoomType> OnZoom;
		/// <summary>玩家点击地面委托</summary>
		public Action OnPlayerClickGround;

		/// <summary>手指上一次的位置</summary>
		private Vector2 _OldFingerPos;
		/// <summary>手指滑动的方向</summary>
		private Vector2 _Dir;

		private int _IntPreFinger = 1;

		private Vector2 _TempFinger1Pos;
		private Vector2 _TempFinger2Pos;
		private Vector2 _OldFinger1Pos;
		private Vector2 _OldFinger2Pos;

		private void Awake() {
			Instance = this;
		}

		private void Update() {

			OnZoom_MouseScrollWheel();



		}


		private void OnEnable() {
			//启动时调用，这里开始注册手势操作的事件。

			//按下事件： OnFingerDown就是按下事件监听的方法，这个名子可以由你来自定义。方法只能在本类中监听。下面所有的事件都一样！！！
			FingerGestures.OnFingerDown += OnFingerDown;
			//抬起事件
			FingerGestures.OnFingerUp += OnFingerUp;
			//开始拖动事件
			FingerGestures.OnFingerDragBegin += OnFingerDragBegin;
			//拖动中事件...
			FingerGestures.OnFingerDragMove += OnFingerDragMove;
			//拖动结束事件
			FingerGestures.OnFingerDragEnd += OnFingerDragEnd;
			//上、下、左、右、四个方向的手势滑动
			FingerGestures.OnFingerSwipe += OnFingerSwipe;
			//连击事件 连续点击事件
			FingerGestures.OnFingerTap += OnFingerTap;
			//按下开始事件
			FingerGestures.OnFingerStationaryBegin += OnFingerStationaryBegin;
			//按下中事件
			FingerGestures.OnFingerStationary += OnFingerStationary;
			//按下结束事件
			FingerGestures.OnFingerStationaryEnd += OnFingerStationaryEnd;
			//长按事件
			FingerGestures.OnFingerLongPress += OnFingerLongPress;

		}

		private void OnDisable() {
			//关闭时调用，这里销毁手势操作的事件
			//和上面一样
			FingerGestures.OnFingerDown -= OnFingerDown;
			FingerGestures.OnFingerUp -= OnFingerUp;
			FingerGestures.OnFingerDragBegin -= OnFingerDragBegin;
			FingerGestures.OnFingerDragMove -= OnFingerDragMove;
			FingerGestures.OnFingerDragEnd -= OnFingerDragEnd;
			FingerGestures.OnFingerSwipe -= OnFingerSwipe;
			FingerGestures.OnFingerTap -= OnFingerTap;
			FingerGestures.OnFingerStationaryBegin -= OnFingerStationaryBegin;
			FingerGestures.OnFingerStationary -= OnFingerStationary;
			FingerGestures.OnFingerStationaryEnd -= OnFingerStationaryEnd;
			FingerGestures.OnFingerLongPress -= OnFingerLongPress;
		}

		//把Unity屏幕坐标换算成3D坐标
		private Vector3 GetWorldPos(Vector2 screenPos) {
			var mainCamera = Camera.main;
			return mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y,
			                                                 Mathf.Abs(transform.position.z - mainCamera.transform.position.z)));
		}


		#region ［事件方法］

		//按下时调用
		private void OnFingerDown(int fingerIndex, Vector2 fingerPos) {
			//int fingerIndex 是手指的ID 第一按下的手指就是 0 第二个按下的手指就是1。。。一次类推。
			//Vector2 fingerPos 手指按下屏幕中的2D坐标

			_IntPreFinger = 1;

			//将2D坐标转换成3D坐标
			transform.position = GetWorldPos(fingerPos);



		}

		//抬起时调用
		private void OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown) {
			if(_IntPreFinger == 1) {
				_IntPreFinger = -1;

				//检测是否碰撞到UI（只包括按钮 ），如果是，则直接返回
				//如果UI摄像机存在
				if(UICamera.currentCamera != null) {
					Ray rayUI = UICamera.currentCamera.ScreenPointToRay(Input.mousePosition);
					if(Physics.Raycast(rayUI, Mathf.Infinity, 1 << LayerMask.NameToLayer("UI"))) { 
						return;
					}
				}
				
				
				OnPlayerClickGround?.Invoke();
			}
		}

		//开始滑动
		private void OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos) {
			_IntPreFinger = 2;
			_OldFingerPos = fingerPos;
		}

		//滑动结束
		private void OnFingerDragEnd(int fingerIndex, Vector2 fingerPos) {
			_IntPreFinger = 4;
			Debug.Log("OnFingerDragEnd fingerIndex =" + fingerIndex + " fingerPos =" + fingerPos);
		}

		//滑动中
		private void OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta) {
			_IntPreFinger = 3;
			//transform.position = GetWorldPos(fingerPos);


			_Dir = fingerPos - _OldFingerPos;
			//向右
			if(_Dir.y < _Dir.x && _Dir.y > -_Dir.x) {
				Debug.Log("向右");
				OnFingerDrag?.Invoke(Direction4.Right);
			}
			//向左
			else if(_Dir.y > _Dir.x && _Dir.y < -_Dir.x) {
				Debug.Log("向左");
				OnFingerDrag?.Invoke(Direction4.Left);
			}
			//向下
			else if(_Dir.y > _Dir.x && _Dir.y > -_Dir.x) {
				Debug.Log("向下");
				OnFingerDrag?.Invoke(Direction4.Down);
			}
			//向上
			else if(_Dir.y < _Dir.x && _Dir.y < -_Dir.x) {
				Debug.Log("向上");
				OnFingerDrag?.Invoke(Direction4.Down);

			}


		}

		//上下左右四方方向滑动手势操作
		private void OnFingerSwipe(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {
			//结果是 Up Down Left Right 四个方向
			Debug.Log("OnFingerSwipe " + direction + " with finger " + fingerIndex);

		}

		//连续按下事件， tapCount就是当前连续按下几次
		private void OnFingerTap(int fingerIndex, Vector2 fingerPos, int tapCount) {

			Debug.Log("OnFingerTap " + tapCount + " times with finger " + fingerIndex);

		}

		//按下事件开始后调用，包括 开始 结束 持续中状态只到下次事件开始！
		private void OnFingerStationaryBegin(int fingerIndex, Vector2 fingerPos) {

			Debug.Log("OnFingerStationaryBegin " + fingerPos + " times with finger " + fingerIndex);
		}


		private void OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime) {

			Debug.Log("OnFingerStationary " + fingerPos + " times with finger " + fingerIndex);

		}

		private void OnFingerStationaryEnd(int fingerIndex, Vector2 fingerPos, float elapsedTime) {

			Debug.Log("OnFingerStationaryEnd " + fingerPos + " times with finger " + fingerIndex);
		}


		//长按事件
		private void OnFingerLongPress(int fingerIndex, Vector2 fingerPos) {

			Debug.Log("OnFingerLongPress " + fingerPos);
		}

		#endregion


		#region ［封装的私有方法］

		/// <summary>调用委托：当鼠标滑轮滑动时，缩放页面</summary>
		private void OnZoom_MouseScrollWheel() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

			if(Input.GetAxis("Mouse ScrollWheel") < 0)
				OnZoom?.Invoke(ZoomType.Out);
			else if(Input.GetAxis("Mouse ScrollWheel") > 0)
				OnZoom?.Invoke(ZoomType.Out);

#endif
		}

		/// <summary>调用委托，当两个手指同时触摸，向外滑动时，缩放页面 TODO：逻辑可能不对</summary>
		private void OnZoom_Touch() {
#if UNITY_ANDROID || UNITY_IPHONE

//如果触摸的手指数为2
			if (Input.touchCount == 2) {
				//如果两个手指都在移动
				if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) {
					_TempFinger1Pos = Input.GetTouch(0).position;
					_TempFinger2Pos = Input.GetTouch(1).position;

					//如果过去两个手指的距离，小于新接收到的两个手指的距离
					if (Vector2.Distance(_OldFinger1Pos, _OldFinger2Pos) < Vector2.Distance(_TempFinger1Pos, _TempFinger2Pos)) {
						//放大
						OnZoom.Invoke(ZoomType.Out);
					}
					else {
						//缩小
						OnZoom.Invoke(ZoomType.In);
					}

					_OldFinger1Pos = _TempFinger1Pos;
					_OldFinger2Pos = _TempFinger2Pos;
				}
			}

		#endif
		}

		#endregion
	}
}