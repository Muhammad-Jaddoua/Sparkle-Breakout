---
paths:
  - "Assets/**/*.cs"
---

# Feedback script conventions

- **Do not edit** `Scripts/Core/*` or `Scripts/Gameplay/*` (shared gameplay; Before uses them). React only through
  `GameEvents`. Scripts in `Scripts/Feedback/` are ours and may be edited and tuned freely.
- New scripts go in `Assets/_Project/Scripts/Feedback/`, one technique per `MonoBehaviour`, named for what it does
  (`PaddleSquash`, `FeedbackAudio`, `ScorePunch`, `BallTrail`, `LifeLossFlash`, `CameraShake`…).
- Feedback must never alter gameplay: no new gameplay rules, no changes to score, lives, combo, speed or input.
- Subscribe in `OnEnable`, unsubscribe in `OnDisable` — exact pairs, using named methods (no lambdas):
  ```csharp
  private void OnEnable()  { GameEvents.BrickDestroyed += HandleBrickDestroyed; }
  private void OnDisable() { GameEvents.BrickDestroyed -= HandleBrickDestroyed; }
  ```
- Event payloads: `ImpactInfo` (`Point`, `Normal`, `Strength` 0–1, `Color`, `Center`, `Size`, `Combo`) for
  collisions; `int` for score/lives; `BallLost` has none. The brick is already destroyed when `BrickDestroyed`
  handlers run — use payload values only. `ScoreChanged(0)` on game-over reset is not "points added".
- Animate visual children only: paddle → its child SpriteRenderer transform; ball → a collider-less visual child,
  `TrailRenderer` or particles. Never write to a Rigidbody2D, Collider2D, the transform of an object that has
  one, `Ball`/`Paddle`/`Brick` fields, or `Time.timeScale` (no hit-stop: it changes game timing).
- Animations must be restartable and always settle back to the cached rest state: cache base
  scale/position/color in `Awake`, stop the previous coroutine before starting a new one (or use a spring/decay
  in `Update`), and reset to base in `OnDisable`. Clamp intensity so rapid repeats never accumulate.
- Use `Time.deltaTime` for in-game motion.
- No per-event allocations in hot paths: pool particle systems / audio sources, no `GetComponent`/`Find` per event.
- Score UI: if the script writes the score text, set `hud.ScoreHandledExternally = true` in `OnEnable`, and in
  `OnDisable` set it `false` and call `hud.RefreshScore()`. If it only scales/colors the text, leave ownership alone.
  The displayed number must always equal `GameManager.Instance.Score`.
- Audio: one shared helper with a small `AudioSource` pool or `PlayOneShot`; distinct clips for paddle, brick and
  ball loss; per-event volume and small random pitch range; optional combo pitch step (clamped); cap voices.
- Expose every tuning value with `[SerializeField]`, `[Header]`, `[Tooltip]`, `[Range]`, plus an enable toggle
  (camera shake: a clearly named Inspector bool that disables it). Defaults must already be the tuned values.
- Comments in English, brief. Code must compile on Unity 6000.3 (`Rigidbody2D.linearVelocity`, TMP via `TMPro`).
