using UnityEngine;

/// <summary>
/// Additional technique — Restrained camera shake.
/// A very short, small nudge of the camera when the ball is lost. Off switch: untick
/// "Shake Enabled" in the Inspector. The camera always returns to its exact original position,
/// and size and rotation are never touched.
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Tooltip("Untick to disable the camera shake completely.")]
    [SerializeField] private bool shakeEnabled = true;
    [Tooltip("Largest offset in world units. Keep small.")]
    [SerializeField, Range(0f, 0.5f)] private float magnitude = 0.3f;
    [SerializeField, Range(0.05f, 0.8f)] private float duration = 0.28f;
    [Tooltip("How fast the shake wobbles.")]
    [SerializeField, Range(5f, 60f)] private float frequency = 32f;

    private Vector3 basePosition;
    private float t = -1f;
    private float seed;

    private void Awake() { basePosition = transform.position; }

    private void OnEnable()  { GameEvents.BallLost += HandleBallLost; }

    private void OnDisable()
    {
        GameEvents.BallLost -= HandleBallLost;
        t = -1f;
        transform.position = basePosition;
    }

    private void HandleBallLost()
    {
        if (!shakeEnabled) return;
        t = 0f;
        seed = Random.value * 100f;
    }

    private void LateUpdate()
    {
        if (t < 0f) return;
        t += Time.deltaTime;
        if (t >= duration)
        {
            t = -1f;
            transform.position = basePosition;   // exact return
            return;
        }

        float fade = 1f - t / duration;
        fade *= fade;
        float nx = Mathf.PerlinNoise(seed, t * frequency) - 0.5f;
        float ny = Mathf.PerlinNoise(seed + 50f, t * frequency) - 0.5f;
        transform.position = basePosition + new Vector3(nx, ny, 0f) * (2f * magnitude * fade);
    }
}
