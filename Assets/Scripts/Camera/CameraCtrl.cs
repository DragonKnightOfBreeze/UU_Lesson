/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 摄像机控制
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 挂载到根物体上（CameraFollowAndRotate）
 * CameraFollowAndRotate
	CameraUpAndDown
		CameraZoomContainer
			CameraContainer
				MainCamera
 *
 */
using UnityEngine;

//using Kernel;
//using Global;

namespace UU_Lesson.NCamera {
	/// <summary>摄像机控制</summary>
	public class CameraCtrl : MonoBehaviour {
		public static CameraCtrl Instance;

		/// <summary>控制摄像机的上下位移</summary>
		private Transform traCameraUpAndDown;
		/// <summary>控制摄像机的缩放</summary>
		private Transform traCameraZoomContainer;
		/// <summary>摄像机的容器</summary>
		private Transform traCameraContainer;

		private void Awake() {
			Instance = this;
		}

		private void Start() {
			traCameraUpAndDown = transform.Find("CameraUpAndDown");
			traCameraZoomContainer = traCameraUpAndDown.transform.Find("CameraZoomContainer");
			traCameraContainer = traCameraZoomContainer.transform.Find("CameraContainer");
		}

		/// <summary>绘制球形实线（与旋转状态时的差不多）</summary>
		private void OnDrawGizmos() {

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, 15f);

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, 14f);

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, 12f);
		}

		/// <summary>摄像机方位的初始化 （在Start()中调用）</summary>
		public void InitCamera() {
			const float minAngle = 35f;
			const float maxAngle = 80f;

			traCameraUpAndDown.localEulerAngles = new Vector3(0, 0,
			                                                  Mathf.Clamp(traCameraUpAndDown.localEulerAngles.z, minAngle, maxAngle));
		}

		/// <summary>摄像机的左右旋转</summary>
		/// <param name="type">0：左，1：右</param>
		/// <param name="rotateSpeed">旋转角度</param>
		public void SetCameraRotate(int type, float rotateSpeed = 20) {
			var direction = type == 0 ? -1 : 1;
			transform.Rotate(0, rotateSpeed * Time.deltaTime * direction, 0);
		}

		/// <summary>摄像机的上下旋转</summary>
		/// <param name="type">0：上，1：下</param>
		/// <param name="rotateSpeed">旋转速度</param>
		public void SetCameraUpAndDown(int type, float rotateSpeed = 20) {
			var direction = type == 0 ? 1 : -1;
			traCameraUpAndDown.Rotate(0, 0, rotateSpeed * Time.deltaTime * direction);

			const float minAngle = 35f;
			const float maxAngle = 80f;

			//限制上下旋转角度
			traCameraUpAndDown.localEulerAngles = new Vector3(0, 0,
			                                                  Mathf.Clamp(traCameraUpAndDown.localEulerAngles.z, minAngle, maxAngle));


		}

		/// <summary>设置摄像机缩放</summary>
		/// <param name="type">0：放大，1：缩小</param>
		/// <param name="zoomSpeed">缩放速度</param>
		public void SetCameraZoom(int type, float zoomSpeed = 10f) {
			var direction = type == 0 ? 1 : -1;
			traCameraContainer.Translate(Vector3.forward * zoomSpeed * Time.deltaTime * direction);

			const float minZoomDis = -5f;
			const float maxZoomDis = 5f;

			//限制缩放距离
			traCameraContainer.localPosition = new Vector3(0, 0, Mathf.Clamp(traCameraContainer.localPosition.z, minZoomDis, maxZoomDis));
		}

		public void AutoLookAt(Vector3 pos) {
			traCameraZoomContainer.LookAt(pos);
		}
	}
}