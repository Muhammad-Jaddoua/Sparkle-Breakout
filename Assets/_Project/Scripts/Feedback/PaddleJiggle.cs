using UnityEngine;

/// <summary>
/// Improvement #2 — Reactive animation (jello paddle).
/// On GameEvents.BallHitPaddle the paddle's VISUAL child:
///   1. builds up a deepening squish (wide and flat) while its color turns milky-bright,
///   2. then springs out: the color snaps back to normal as the ball rebounds, and the
///      jelly overshoots into a stretch and wobbles back to rest,
///   3. fizzes a few tiny sparkles at the contact point.
/// Only the visual child is touched. Collider, Rigidbody and the Paddle script are never changed.
/// A damped spring drives everything, so rapid hits blend smoothly and always settle at rest.
/// </summary>
public class PaddleJiggle : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The paddle's visual child (has the SpriteRenderer). Never the object with the collider.")]
    [SerializeField] private Transform visual;
    [SerializeField] private SpriteRenderer visualRenderer;
    [Tooltip("Tiny sparkle particles at the contact point (Emit on demand, World space).")]
    [SerializeField] private ParticleSystem fizz;

    [Header("Squish build-up")]
    [Tooltip("How long the squish keeps building before it springs out (seconds).")]
    [SerializeField, Range(0.03f, 0.3f)] private float buildUpTime = 0.12f;
    [Tooltip("How deep the squish gets. 0.7 = visual about 38% shorter and 22% wider.")]
    [SerializeField, Range(0.1f, 0.9f)] private float squishDepth = 0.7f;
    [Tooltip("Higher = the squish accelerates harder toward the end (more build-up).")]
    [SerializeField, Range(1f, 4f)] private float buildUpCurve = 2.2f;

    [Header("Spring out and jiggle")]
    [Tooltip("Speed of the release (units per second toward a stretch). Bigger = more powerful pop.")]
    [SerializeField, Range(0f, 30f)] private float releaseKick = 18f;
    [Tooltip("Wobbles per second.")]
    [SerializeField, Range(2f, 12f)] private float frequency = 6.5f;
    [Tooltip("0 = never stops wobbling, 1 = no wobble. Low = obnoxiously jiggly.")]
    [SerializeField, Range(0.05f, 0.8f)] private float damping = 0.14f;
    [Tooltip("Safety clamp so rapid hits can never over-stretch the paddle.")]
    [SerializeField, Range(0.3f, 1.2f)] private float maxExtent = 0.75f;

    [Header("Shape")]
    [Tooltip("How much the width changes per unit of squish (width grows when squished).")]
    [SerializeField, Range(0f, 0.8f)] private float widthFactor = 0.32f;
    [Tooltip("How much the height changes per unit of squish.")]
    [SerializeField, Range(0f, 0.9f)] private float heightFactor = 0.55f;

    [Header("Color")]
    [SerializeField] private Color normalColor = new Color(0.30f, 0.88f, 0.82f, 1f);
    [SerializeField] private Color hitColor = new Color(0.86f, 1f, 0.97f, 1f);
    [Tooltip("How fast the color snaps back after the spring-out (per second).")]
    [SerializeField, Range(4f, 40f)] private float colorSnapSpeed = 18f;

    [Header("Fizz")]
    [SerializeField, Range(0, 30)] private int fizzCount = 10;

    private Vector3 baseScale;
    private Vector3 basePos;
    private float x;          // spring offset: >0 squished, <0 stretched
    private float vel;
    private float buildTimer; // >0 while building up
    private float colorT;     // 0 normal .. 1 hit color

    private void Awake()
    {
        if (visual == null) visual = transform.childCount > 0 ? transform.GetChild(0) : null;
        if (visualRenderer == null && visual != null) visualRenderer = visual.GetComponent<SpriteRenderer>();
        if (visual != null)
        {
            baseScale = visual.localScale;
            basePos = visual.localPosition;
        }
    }

    private void OnEnable()  { GameEvents.BallHitPaddle += HandleBallHitPaddle; }

    private void OnDisable()
    {
        GameEvents.BallHitPaddle -= HandleBallHitPaddle;
        x = vel = buildTimer = colorT = 0f;
        Apply();
    }

    private void HandleBallHitPaddle(ImpactInfo info)
    {
        buildTimer = buildUpTime;

        if (fizz != null)
        {
            Transform t = fizz.transform;
            t.position = new Vector3(info.Point.x, info.Point.y, t.position.z);
            fizz.Emit(fizzCount);
        }
    }

    private void Update()
    {
        if (visual == null) return;
        float dt = Mathf.Min(Time.deltaTime, 0.033f);

        if (buildTimer > 0f)
        {
            // Build-up: ease toward the full squish with an accelerating curve.
            buildTimer -= dt;
            float t = 1f - Mathf.Clamp01(buildTimer / buildUpTime);
            float target = squishDepth * Mathf.Pow(t, 1f / buildUpCurve);
            x = Mathf.Max(x, Mathf.MoveTowards(x, target, 12f * dt));
            vel = 0f;
            colorT = Mathf.Max(colorT, t);

            if (buildTimer <= 0f)
            {
                // Release: kick toward a stretch, snap the color back.
                vel = -releaseKick;
                colorT = Mathf.Min(colorT, 1f);
            }
        }
        else
        {
            // Damped spring back to rest (x = 0).
            float omega = 2f * Mathf.PI * frequency;
            float acc = -omega * omega * x - 2f * damping * omega * vel;
            vel += acc * dt;
            x += vel * dt;
            colorT = Mathf.MoveTowards(colorT, 0f, colorSnapSpeed * dt);

            // Settle exactly at rest so nothing drifts.
            if (Mathf.Abs(x) < 0.0005f && Mathf.Abs(vel) < 0.005f) { x = 0f; vel = 0f; }
        }

        x = Mathf.Clamp(x, -maxExtent, maxExtent);
        Apply();
    }

    private void Apply()
    {
        if (visual == null) return;

        float sx = Mathf.Max(0.3f, 1f + x * widthFactor);
        float sy = Mathf.Max(0.25f, 1f - x * heightFactor);
        visual.localScale = new Vector3(baseScale.x * sx, baseScale.y * sy, baseScale.z);

        // Keep the bottom edge fixed so it reads as pressing down into the paddle.
        float halfHeight = 0.5f * baseScale.y;
        visual.localPosition = new Vector3(basePos.x, basePos.y - (1f - sy) * halfHeight, basePos.z);

        if (visualRenderer != null)
            visualRenderer.color = Color.Lerp(normalColor, hitColor, colorT);
    }
}
