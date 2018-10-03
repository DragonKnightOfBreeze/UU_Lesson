using UnityEngine;

public class SwipeParticlesEmitter : MonoBehaviour {
	public ParticleSystem emitter;
	public float baseSpeed = 4.0f;
	public float swipeVelocityScale = 0.001f;

	private void Start() {
		if(!emitter)
			emitter = GetComponent<ParticleSystem>();
		var _emission = emitter.emission;
		_emission.enabled = false;
	}

	public void Emit(FingerGestures.SwipeDirection direction, float swipeVelocity) {
		Vector3 heading;

		// convert the swipe direction to a 3D vector we can use as our new forward direction for the particle emitter
		if(direction == FingerGestures.SwipeDirection.Up)
			heading = Vector3.up;
		else if(direction == FingerGestures.SwipeDirection.Down)
			heading = Vector3.down;
		else if(direction == FingerGestures.SwipeDirection.Right)
			heading = Vector3.right;
		else
			heading = Vector3.left;

		// orient our emitter towards the swipe direction
		emitter.transform.rotation = Quaternion.LookRotation(heading);

		var v = emitter.velocityOverLifetime;
		v.z = baseSpeed * swipeVelocityScale * swipeVelocity;

		// fire away!
		emitter.Emit(1);
	}
}