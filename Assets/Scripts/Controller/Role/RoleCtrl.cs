/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 *
 * ［标题］
 *
 *
 * ［功能］
 *
 *
 * ［思路］
 *
 *
 * ［用法］
 * 挂载到角色游戏对象上
 */

using System;
using UU_Lesson.Global;
using UnityEngine;
using UnityEngine.UI;
using UU_Lesson.Common;
using UU_Lesson.Controller.Role.AI;
using UU_Lesson.Controller.Role.FSM;
using UU_Lesson.Controller.Role.Info;
using UU_Lesson.NCamera;

//using Kernel;
//using Global;

namespace UU_Lesson.Controller.Role {
	/// <summary>主角控制</summary>
	[RequireComponent(typeof(CharacterController),typeof(Animator))]
	public class RoleCtrl : MonoBehaviour {

		#region ［字段和属性］
		
		
		private CharacterController cc;

		
		/// <summary>移动速度</summary>
		[SerializeField]
		private float _MoveSpeed = 10f;
		/// <summary>移动的目标点</summary>
		private Vector3 m_TargetPos = Vector3.zero;
		/// <summary>转身速度</summary>
		private float _RotationSpeed;
		/// <summary>转身的目标方向</summary>
		private Quaternion _TargetQuaternion;

		[HideInInspector]
		public Animator MyAnimator;

		/// <summary>当前角色的类型</summary>
		public RoleType CurRoleType { get; set; } = RoleType.None;

		/// <summary>当前角色的有限状态机管理器</summary>
		public RoleFSMMgr CurRoleFSMMgr { get; set; }

		/// <summary>当前角色的信息</summary>
		public InfoBase CurRoleInfo { get; set; }

		/// <summary>当前角色的AI</summary>
		public IRoleAI CurRoleAI { get; set; }
		
		#endregion
		
		
		#region ［Unity消息］

		private void Start() {
			MyAnimator = gameObject.GetComponent<Animator>();
			cc = gameObject.GetComponent<CharacterController>();
			CameraCtrl.Instance.InitCamera();
			
			//实例化有限状态机管理器
			CurRoleFSMMgr = new RoleFSMMgr(this);
		}


		private void Awake() {
			FingerEvent.Instance.OnFingerDrag += OnFingerDrag;
			FingerEvent.Instance.OnPlayerClickGround += OnPlayerClickGround;
			FingerEvent.Instance.OnZoom += OnZoom;
		}


		void OnDestroy() {
			if(FingerEvent.Instance == null)
				return;
			FingerEvent.Instance.OnFingerDrag -= OnFingerDrag;
			FingerEvent.Instance.OnPlayerClickGround -= OnPlayerClickGround;
			FingerEvent.Instance.OnPlayerClickGround -= OnPlayerClickGround;
		}

		private void Update() {
			
			//执行AI，后续的代码可以删去
			CurRoleAI?.Execute();
			//执行FSM
			CurRoleFSMMgr?.UpdateState();
			
			PlayerMove();
		}
		
		#endregion
		
		
		#region ［公共方法］

		/// <summary>
		/// 初始化方法
		/// </summary>
		/// <param name="roleType"></param>
		/// <param name="infoBase"></param>
		/// <param name="roleAI"></param>
		public void Init(RoleType roleType,InfoBase infoBase, IRoleAI roleAI) {
			CurRoleType = roleType;
			CurRoleInfo = infoBase;
			CurRoleAI = roleAI;
		}
		
		#endregion

		
		
		#region ［私有方法］

		/// <summary>
		/// 角色移动方法
		/// </summary>
		/// <remarks>需要重构</remarks>
		private void PlayerMove() {
			if(cc == null)
				return;

			////点击屏幕
			////鼠标抬起事件，0是鼠标左键，1是鼠标右键
			////touchCount表示触摸屏幕的触摸点数
			//if (Input.GetMouseButtonUp(0) || Input.touchCount == 1) {
			//	//Debug.Log("点击屏幕了");

			//	//从摄像机发射一个射线，获取鼠标点击的坐标
			//	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//	RaycastHit hitInfo;

			//	//如果射线碰撞到地面，则返回一个目标点
			//	if (Physics.Raycast(ray, out hitInfo)) {
			//		if (hitInfo.collider.gameObject.name.Equals("Ground", StringComparison.CurrentCultureIgnoreCase)) {
			//			m_TargetPos = hitInfo.point;
			//			//？？
			//			_RotationSpeed = 0;

			//			//
			//			BoxCtrl2 boxCtrl2 = hitInfo.collider.GetComponent<BoxCtrl2>();
			//			boxCtrl2?.Hit();
			//		}
			//	}
			//}

			//着地控制
			if(!cc.isGrounded)
				cc.Move(transform.position + new Vector3(0, -1000, 0) - transform.position);

			//如果目标点不是原点，就进行移动
			if(m_TargetPos != Vector3.zero)
				if(Vector3.Distance(m_TargetPos, transform.position) > 0.1f) {
					var direction = m_TargetPos - transform.position;
					direction = direction.normalized; //归一化
					direction = direction * Time.deltaTime * _MoveSpeed;

					//角色注视当前的前方
					//transform.LookAt(m_TargetPos);
					//transform.LookAt(new Vector3(m_TargetPos.x,transform.position.y,m_TargetPos.z));
					//点击键盘按键，获取水平与垂直偏移值（从-1到1）
					//float floMovingXPos = -Input.GetAxis("Horizontal");
					//float floMovingYPos = -Input.GetAxis("Vertical");
					////采用俯视视角
					//transform.LookAt(new Vector3(transform.position.x + floMovingXPos, transform.position.y, transform.position.z + floMovingYPos));

					//让角色缓慢转身
					if(_RotationSpeed <= 1) {
						_RotationSpeed += 5f * Time.deltaTime;
						//要看向的方向的四元数
						_TargetQuaternion = Quaternion.LookRotation(direction);
						transform.rotation = Quaternion.Lerp(transform.rotation, _TargetQuaternion,
						                                     Time.deltaTime * _RotationSpeed);
					}

					//角色往某个单位向量代表的方向，以对应的速度移动
					cc.Move(direction);
				}
		}
		

		/// <summary>摄像机自动跟随 在Update中调用</summary>
		private void CameraAutoFollow() {
			//非空检查
			if(CameraCtrl.Instance == null)
				return;
			//同步位置
			CameraCtrl.Instance.transform.position = gameObject.transform.position;
			CameraCtrl.Instance.AutoLookAt(gameObject.transform.position);

//			/* 按键测试 */
//
//			if(Input.GetKey(KeyCode.A))
//				CameraCtrl.Instance.SetCameraRotate(0);
//			else if(Input.GetKey(KeyCode.D))
//				CameraCtrl.Instance.SetCameraRotate(1);
//			else if(Input.GetKey(KeyCode.W))
//				CameraCtrl.Instance.SetCameraUpAndDown(0);
//			else if(Input.GetKey(KeyCode.A))
//				CameraCtrl.Instance.SetCameraRotate(1);
//			else if(Input.GetKey(KeyCode.I))
//				CameraCtrl.Instance.SetCameraZoom(0);
//			else if(Input.GetKey(KeyCode.O))
//				CameraCtrl.Instance.SetCameraZoom(1);

		}
		
		#endregion
		
		
		#region ［触摸屏幕事件］
		
		/// <summary>要注册的方法：缩放控制</summary>
		/// <param name="obj"></param>
		private void OnZoom(ZoomType obj) {
			switch(obj) {
				case ZoomType.In:
					CameraCtrl.Instance.SetCameraZoom(0);
					break;
				case ZoomType.Out:
					CameraCtrl.Instance.SetCameraZoom(1);
					break;
			}
		}

		/// <summary>要注册的方法：点击地面</summary>
		private void OnPlayerClickGround() {
			
//			//检测是否碰撞到UI（只包括按钮 ），如果是，则直接返回
//			
//			//如果UI摄像机存在
//			if(UICamera.currentCamera != null) {
//				Ray rayUI = UICamera.currentCamera.ScreenPointToRay(Input.mousePosition);
//				if(Physics.Raycast(rayUI, Mathf.Infinity, 1 << LayerMask.NameToLayer("UI"))) {
//					Debug.Log("碰撞到了UI层！");
//					return;
//				}
//			}
			
			//从摄像机发射一个射线，获取鼠标点击的坐标
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;

			//如果射线碰撞到地面，则返回一个目标点
			if(Physics.Raycast(ray, out hitInfo))
				if(hitInfo.collider.gameObject.name.Equals("Ground", StringComparison.CurrentCultureIgnoreCase)) {
					m_TargetPos = hitInfo.point;
					_RotationSpeed = 0;
				}
		}

		/// <summary>要注册的方法：手指滑动，控制摄像机旋转</summary>
		/// <param name="obj"></param>
		private void OnFingerDrag(Direction4 obj) {
			switch(obj) {
				case Direction4.Left:
					CameraCtrl.Instance.SetCameraRotate(0);
					break;
				case Direction4.Right:
					CameraCtrl.Instance.SetCameraRotate(1);
					break;
				case Direction4.Up:
					CameraCtrl.Instance.SetCameraUpAndDown(0);
					break;
				case Direction4.Down:
					CameraCtrl.Instance.SetCameraUpAndDown(1);
					break;
			}
		}


		#endregion
		
		
		
	}
}