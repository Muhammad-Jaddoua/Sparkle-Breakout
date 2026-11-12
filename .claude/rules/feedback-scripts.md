---
paths:
  - "Assets/**/*.cs"
---

# Feedback script conventions

- New scripts go in `Assets/_Project/Scripts/Feedback/`, one technique per `MonoBehaviour`, named for what it does
  (`BrickBurstParticles`, `PaddleSquash`, `FeedbackAudio`, `ScorePunch`, `BallTrail`, `CameraShake`…).
- **Do not edit** `Scripts/Core/*` or `Scripts/Gameplay/*`. React only through `GameEvents`.
- Subscribe in `OnEnable`, unsubscribe in `OnDisable` — exact pairs, using named methods (no lambdas):
  ```csharp
  private void OnEnable()  { GameEvents.BrickDestroyed += HandleBrickDestroyed; }
  private void OnDisable() { GameEvents.BrickDestroyed -= HandleBrickDestroyed; }
  ```
- Event payloads: `ImpactInfo` (`Point`, `Normal`, `Strength` 0–1, `Color`, `Center`, `Size`, `Combo`) for
  collisions; `int` for score/lives. The brick is already destroyed when `BrickDestroyed` handlers run.
- Animate visual children only: paddle → its child SpriteRenderer transform; ball → add a visual child or use
  `TrailRenderer`/particles. Never write to a Rigidbody2D, Collider2D, `Ball`/`Paddle` fields, or `Time.timeScale`
  (unless explicitly agreed and documented).
- Animations must be restartable and always settle back to the cached rest state: cache the base
  scale/position/color in `Awake`, stop the previous coroutine before starting a new one (or use a spring/decay
  driven in `Update`), and reset to base in `OnDisable`.
- Use `Time.deltaTime` for in-game motion; use `Time.unscaledDeltaTime` only for UI if timeScale is ever changed.
- No per-event allocations in hot paths: pool particle systems / audio sources, no `GetComponent` or `Find` per event.
- Score UI: if the script writes the score text, set `hud.ScoreHandledExternally = true` in `OnEnable`, and in
  `OnDisable` set it `false` and call `hud.RefreshScore()`. If it only scales/colors the text, leave ownership alone.
- Audio: one shared helper with a small `AudioSource` pool or `PlayOneShot`; per-event clip arrays, volume and
  small random pitch range; optional combo-based pitch step (clamped); cap simultaneous voices.
- Expose every tuning value with `[SerializeField]`, `[Header]`, `[Tooltip]`, `[Range]`, and provide a master
  `enabled`/bool toggle (required for camera shake). Defaults must already be the tuned values.
- Comments in English, brief. Code must compile on Unity 6000.3 (`Rigidbody2D.linearVelocity`, TMP via `TMPro`).
