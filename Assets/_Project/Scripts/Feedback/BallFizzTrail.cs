using UnityEngine;

/// <summary>
/// Improvement #5 — Ball trail.
/// A fading ribbon (TrailRenderer) plus a few tiny sparkles shed along the ball's path.
/// Both live on children of the ball, so the ball's collider and Rigidbody are never touched.
///
/// When the ball is relaunched after being lost it teleports to its start point; the trail
/// and sparkle emission pause for one frame so no streak is drawn across the screen.
/// </summary>
public class BallFizzTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer ribbon;
    [Tooltip("Tiny sparkles with Emission > Rate over Distance. Simulation Space = World.")]
    [SerializeField] private ParticleSystem fizz;

    private bool clearPending;

    private void OnEnable()
    {
        GameEvents.BallLost += HandleBallLost;
    }

    private void OnDisable()
    {
        GameEvents.BallLost -= HandleBallLost;
        SetActive(true);
    }

    private void HandleBallLost()
    {
        // The ball is teleported right after this event, so wait until LateUpdate to restart.
        clearPending = true;
        SetActive(false);
    }

    private void LateUpdate()
    {
        if (!clearPending) return;
        clearPending = false;
        if (ribbon != null) ribbon.Clear();
        if (fizz != null) fizz.Clear();
        SetActive(true);
    }

    private void SetActive(bool on)
    {
        if (ribbon != null) ribbon.emitting = on;
        if (fizz != null)
        {
            var e = fizz.emission;
            e.enabled = on;
        }
    }
}
