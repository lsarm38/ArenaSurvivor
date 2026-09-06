using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Screen Shake")]
    [SerializeField] private PlayerHealth playerHealth; // optional — shake triggers whenever this takes damage
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    private Vector3 velocity = Vector3.zero;
    private float zOffset;
    private float shakeTimeRemaining;

    private void Start()
    {
        // Preserve the camera's original Z position (must stay negative in 2D to render in front of sprites)
        zOffset = transform.position.z;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDamaged += HandlePlayerDamaged;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDamaged -= HandlePlayerDamaged;
        }
    }

    private void HandlePlayerDamaged()
    {
        shakeTimeRemaining = shakeDuration;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, zOffset);

        // SmoothDamp gives a natural "catching up" feel instead of rigidly snapping to the player
        Vector3 followedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        if (shakeTimeRemaining > 0f)
        {
            // Unscaled, not Time.deltaTime — otherwise this freezes mid-shake
            // whenever Time.timeScale hits 0 (e.g. on death or level-up pause),
            // since LateUpdate keeps running every frame regardless of timeScale
            shakeTimeRemaining -= Time.unscaledDeltaTime;

            float strength = shakeMagnitude * (shakeTimeRemaining / shakeDuration);
            Vector2 shakeOffset = Random.insideUnitCircle * strength;
            followedPosition += new Vector3(shakeOffset.x, shakeOffset.y, 0f);
        }

        transform.position = followedPosition;
    }
}