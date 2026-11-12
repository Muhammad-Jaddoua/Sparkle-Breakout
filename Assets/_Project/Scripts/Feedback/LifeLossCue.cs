using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Additional technique — Life-loss cue.
/// When the ball is lost, a soft pink glow flashes around the screen edges and fades out,
/// so a miss is clearly visible even with the sound muted. Purely a UI overlay image
/// (no raycasts, drawn behind the HUD text). A new miss restarts the flash instead of stacking.
/// </summary>
public class LifeLossCue : MonoBehaviour
{
    [Tooltip("Full-screen UI Image with a soft edge-glow sprite. Raycast Target OFF.")]
    [SerializeField] private Image vignette;
    [SerializeField] private Color color = new Color(1f, 0.3f, 0.5f, 1f);
    [Tooltip("Peak opacity of the flash.")]
    [SerializeField, Range(0f, 1f)] private float peakAlpha = 0.4f;
    [Tooltip("How long it takes to fade out (seconds).")]
    [SerializeField, Range(0.1f, 2f)] private float fadeTime = 0.6f;

    private float t = -1f;

    private void OnEnable()
    {
        GameEvents.BallLost += HandleBallLost;
        SetAlpha(0f);
    }

    private void OnDisable()
    {
        GameEvents.BallLost -= HandleBallLost;
        SetAlpha(0f);
    }

    private void HandleBallLost() { t = 0f; }

    private void Update()
    {
        if (t < 0f) return;
        t += Time.deltaTime;
        if (t >= fadeTime) { t = -1f; SetAlpha(0f); return; }
        float u = 1f - t / fadeTime;
        SetAlpha(peakAlpha * u * u);   // quick punch, smooth fade
    }

    private void SetAlpha(float a)
    {
        if (vignette == null) return;
        Color c = color; c.a = a;
        vignette.color = c;
    }
}
