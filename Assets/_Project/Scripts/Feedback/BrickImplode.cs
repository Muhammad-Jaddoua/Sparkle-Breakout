using UnityEngine;

/// <summary>
/// Additional technique — Brick disappearance animation ("collapsing black hole").
/// The real brick is destroyed instantly by the game. This draws a purely visual copy
/// (no collider, no Rigidbody) at the brick's spot that winds up slightly, then collapses
/// into its center while spinning and flashing bright, and finally triggers the confetti burst.
///
/// A fixed pool of ghost sprites is reused, so rapid brick breaks never allocate or pile up.
/// </summary>
public class BrickImplode : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Sprite used by the brick (same as the Brick prefab's sprite).")]
    [SerializeField] private Sprite brickSprite;
    [Tooltip("Fired when the collapse finishes. Turn its 'Burst On Event' OFF.")]
    [SerializeField] private BrickBurstParticles burst;

    [Header("Timing")]
    [Tooltip("Short swell before the collapse (seconds).")]
    [SerializeField, Range(0f, 0.1f)] private float windUpTime = 0.035f;
    [SerializeField, Range(1f, 1.4f)] private float windUpScale = 1.12f;
    [Tooltip("Collapse duration (seconds). Keep short so the game feels snappy.")]
    [SerializeField, Range(0.04f, 0.3f)] private float collapseTime = 0.10f;

    [Header("Look")]
    [Tooltip("Spin during the collapse, in degrees.")]
    [SerializeField, Range(0f, 360f)] private float spinDegrees = 140f;
    [Tooltip("How much the brick brightens toward white as it collapses.")]
    [SerializeField, Range(0f, 1f)] private float flashAmount = 0.8f;
    [SerializeField] private int sortingOrder = 1;
    [SerializeField, Range(4, 32)] private int poolSize = 16;

    private class Ghost
    {
        public Transform t;
        public SpriteRenderer sr;
        public bool active;
        public float time;
        public Vector3 size;
        public Color color;
        public ImpactInfo info;
    }

    private Ghost[] pool;
    private int oldest;

    private void Awake()
    {
        pool = new Ghost[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject("BrickGhost");
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = brickSprite;
            sr.sortingOrder = sortingOrder;
            sr.enabled = false;
            pool[i] = new Ghost { t = go.transform, sr = sr };
        }
    }

    private void OnEnable()  { GameEvents.BrickDestroyed += HandleBrickDestroyed; }

    private void OnDisable()
    {
        GameEvents.BrickDestroyed -= HandleBrickDestroyed;
        if (pool != null) foreach (var g in pool) { g.active = false; g.sr.enabled = false; }
    }

    private void HandleBrickDestroyed(ImpactInfo info)
    {
        Ghost g = null;
        for (int i = 0; i < pool.Length; i++) if (!pool[i].active) { g = pool[i]; break; }
        if (g == null)
        {
            // Pool full: finish the oldest ghost right now and reuse it.
            g = pool[oldest];
            Finish(g);
        }
        oldest = (oldest + 1) % pool.Length;

        Vector2 spriteSize = brickSprite != null ? (Vector2)brickSprite.bounds.size : Vector2.one;
        g.info = info;
        g.time = 0f;
        g.color = new Color(info.Color.r, info.Color.g, info.Color.b, 1f);
        g.size = new Vector3(info.Size.x / spriteSize.x, info.Size.y / spriteSize.y, 1f);
        g.t.position = new Vector3(info.Center.x, info.Center.y, 0f);
        g.t.rotation = Quaternion.identity;
        g.t.localScale = g.size;
        g.sr.color = g.color;
        g.sr.enabled = true;
        g.active = true;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < pool.Length; i++)
        {
            Ghost g = pool[i];
            if (!g.active) continue;

            g.time += dt;
            float scale, spin = 0f, flash = 0f;

            if (g.time < windUpTime)
            {
                float u = g.time / windUpTime;
                scale = Mathf.Lerp(1f, windUpScale, 1f - (1f - u) * (1f - u));   // ease-out swell
            }
            else
            {
                float u = Mathf.Clamp01((g.time - windUpTime) / collapseTime);
                float suck = u * u * u;                                           // accelerates like a black hole
                scale = windUpScale * (1f - suck);
                spin = spinDegrees * suck;
                flash = flashAmount * u;
                if (u >= 1f) { Finish(g); continue; }
            }

            g.t.localScale = g.size * Mathf.Max(0f, scale);
            g.t.rotation = Quaternion.Euler(0f, 0f, spin);
            g.sr.color = Color.Lerp(g.color, Color.white, flash);
        }
    }

    private void Finish(Ghost g)
    {
        g.active = false;
        g.sr.enabled = false;
        if (burst != null) burst.Burst(g.info);
    }
}
