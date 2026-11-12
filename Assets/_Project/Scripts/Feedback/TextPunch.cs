using TMPro;
using UnityEngine;

/// <summary>
/// Improvement #4 — Interface feedback.
/// Makes a HUD text spring up in size with a bouncy overshoot and flash a color, then settle
/// back to exactly its normal size and color.
///   Score mode: punches when the score goes UP (not on the game-over reset to 0).
///   Lives mode: punches on every lost ball (so a miss is visible even with the sound off).
/// Only the text's scale and color are touched. The shown number is still written by HUD,
/// so it is always correct; ScoreHandledExternally is left alone.
/// A damped spring drives the scale, so rapid triggers blend smoothly and cannot pile up.
/// </summary>
public class TextPunch : MonoBehaviour
{
    public enum Trigger { ScoreIncrease, BallLost }

    [SerializeField] private Trigger trigger = Trigger.ScoreIncrease;
    [SerializeField] private TMP_Text target;

    [Header("Spring")]
    [Tooltip("Kick strength. Bigger = bigger pop.")]
    [SerializeField, Range(1f, 30f)] private float kick = 12f;
    [Tooltip("Wobbles per second.")]
    [SerializeField, Range(2f, 12f)] private float frequency = 5.5f;
    [Tooltip("0 = never stops wobbling, 1 = no wobble.")]
    [SerializeField, Range(0.05f, 0.8f)] private float damping = 0.2f;
    [Tooltip("Safety clamp on how far the text can grow or shrink.")]
    [SerializeField, Range(0.1f, 1f)] private float maxExtent = 0.6f;

    [Header("Color flash")]
    [SerializeField] private Color flashColor = new Color(1f, 0.45f, 0.65f, 1f);
    [Tooltip("How fast the flash fades back to normal (per second).")]
    [SerializeField, Range(1f, 20f)] private float flashDecay = 5f;

    private Vector3 baseScale;
    private Color baseColor;
    private float x, vel, colorT;
    private int lastScore;

    private void Awake()
    {
        if (target == null) target = GetComponent<TMP_Text>();
        baseScale = target.rectTransform.localScale;
        baseColor = target.color;
    }

    private void OnEnable()
    {
        lastScore = GameManager.Instance != null ? GameManager.Instance.Score : 0;
        GameEvents.ScoreChanged += HandleScoreChanged;
        GameEvents.BallLost += HandleBallLost;
    }

    private void OnDisable()
    {
        GameEvents.ScoreChanged -= HandleScoreChanged;
        GameEvents.BallLost -= HandleBallLost;
        x = vel = colorT = 0f;
        Apply();
    }

    private void HandleScoreChanged(int score)
    {
        bool increased = score > lastScore;
        lastScore = score;                       // also tracks the reset to 0
        if (trigger == Trigger.ScoreIncrease && increased) Pop();
    }

    private void HandleBallLost()
    {
        if (trigger == Trigger.BallLost) Pop();
    }

    private void Pop()
    {
        vel = Mathf.Min(vel + kick, kick * 1.5f);   // blends with a pop already in progress
        colorT = 1f;
    }

    private void Update()
    {
        float dt = Mathf.Min(Time.deltaTime, 0.033f);
        float omega = 2f * Mathf.PI * frequency;
        vel += (-omega * omega * x - 2f * damping * omega * vel) * dt;
        x = Mathf.Clamp(x + vel * dt, -maxExtent, maxExtent);
        colorT = Mathf.MoveTowards(colorT, 0f, flashDecay * dt);

        if (Mathf.Abs(x) < 0.0005f && Mathf.Abs(vel) < 0.005f) { x = 0f; vel = 0f; }
        Apply();
    }

    private void Apply()
    {
        target.rectTransform.localScale = baseScale * (1f + x);
        target.color = Color.Lerp(baseColor, flashColor, colorT);
    }
}
