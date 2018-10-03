using UnityEngine;

namespace Kernel {
	public class CameraFollow : MonoBehaviour {
		// The target we are following
		public Transform target;
		// The distance in the x-z plane to the target
		public float distance = 6.0f; //原来 6
		// the height we want the camera to be above the target
		public float height = 8.0f; //原来 8
		// How much we 
		public float heightDamping = 4.0f;
		public float rotationDamping;

		private void LateUpdate() {
			// Early out if we don't have a target
			if(!target)
				return;
			// Calculate the current rotation angles
			var wantedRotationAngle = target.eulerAngles.y;
			var wantedHeight = target.position.y + height;
			var currentRotationAngle = transform.eulerAngles.y;
			var currentHeight = transform.position.y;
			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			// Damp the height
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

			// Convert the angle Into a rotation
			var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			transform.position = target.position;
			transform.position -= currentRotation * Vector3.forward * distance;
			// Set the height of the camera
			//transform.position.y = currentHeight;       
			transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
			// Always look at the target
			transform.LookAt(target);
			// Use this for initialization
		}

		private void Start() {
			target = GameObject.FindGameObjectWithTag("Player").transform;
		}

		private float dt = 2.0f;

		// Update is called once per frame
		private void Update() {
			//每隔2秒钟确认一下目标，用插值确认
			dt -= Time.deltaTime;
			if(dt <= 0) {
				target = GameObject.FindGameObjectWithTag("Player").transform;
				dt = 2.0f;

			}
		}
	} //class_end
}