using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float smoothTime = 0.15f;

	private Vector3 velocity = Vector3.zero;
	private float zOffset;

	private void Start()
	{
		// Preserve the camera's original Z position (must stay negative in 2D to render in front of sprites)
		zOffset = transform.position.z;
	}

	private void LateUpdate()
	{
		if (target == null) return;

		Vector3 targetPosition = new Vector3(target.position.x, target.position.y, zOffset);

		// SmoothDamp gives a natural "catching up" feel instead of rigidly snapping to the player
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
	}
}