using UnityEngine;

/// <summary>
/// Improvement #1 — Brick impact effect ("party" burst).
/// When a brick is destroyed: brick-colored confetti tumbles out of the brick's area,
/// and a few pastel rainbow sparkles twinkle upward from its center.
///
/// Both particle systems live for the whole scene (World simulation space) and are
/// fired with Emit(), so nothing is created or destroyed per hit and every particle
/// fades out on its own — no pile-up, finishes cleanly.
/// </summary>
public class BrickBurstParticles : MonoBehaviour
{
    [Header("Particle systems (children of this object)")]
    [Tooltip("Square confetti. Looping off, Play On Awake off, Emission off, Simulation Space = World.")]
    [SerializeField] private ParticleSystem confetti;

    [Tooltip("Star sparkles. Same base settings. Their colors come from the system's own Start Color (rainbow).")]
    [SerializeField] private ParticleSystem sparkles;

    [Header("Amounts")]
    [SerializeField, Range(0, 40)] private int confettiCount = 12;
    [SerializeField, Range(0, 20)] private int sparkleCount = 5;

    [Tooltip("Extra confetti per combo step (bricks broken without touching the paddle).")]
    [SerializeField, Range(0, 6)] private int extraPerCombo = 2;

    [Tooltip("Upper limit on confetti so long combos never flood the screen.")]
    [SerializeField, Range(1, 60)] private int maxConfetti = 24;

    [Header("Confetti color")]
    [Tooltip("How much lighter some pieces are than the brick (0 = exact brick color).")]
    [SerializeField, Range(0f, 1f)] private float highlightAmount = 0.45f;

    private void OnEnable()  { GameEvents.BrickDestroyed += HandleBrickDestroyed; }
    private void OnDisable() { GameEvents.BrickDestroyed -= HandleBrickDestroyed; }

    private void HandleBrickDestroyed(ImpactInfo info)
    {
        int combo = Mathf.Max(0, info.Combo - 1);

        if (confetti != null)
        {
            MoveTo(confetti, info.Center);

            // Spread the confetti over the brick's own rectangle.
            var shape = confetti.shape;
            shape.scale = new Vector3(info.Size.x, info.Size.y, 1f);

            // Each piece is randomly between the brick color and a lighter tint.
            Color brick = info.Color;
            brick.a = 1f;
            var main = confetti.main;
            main.startColor = new ParticleSystem.MinMaxGradient(brick, Color.Lerp(brick, Color.white, highlightAmount));

            confetti.Emit(Mathf.Min(confettiCount + extraPerCombo * combo, maxConfetti));
        }

        if (sparkles != null)
        {
            MoveTo(sparkles, info.Center);
            sparkles.Emit(sparkleCount);
        }
    }

    private static void MoveTo(ParticleSystem ps, Vector2 point)
    {
        Transform t = ps.transform;
        t.position = new Vector3(point.x, point.y, t.position.z);
    }
}
