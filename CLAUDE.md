# SWE402 Assignment 3 — Improving Game Feel (JuiceLab Dead Starter)

Unity 2D brick-breaker that is deliberately unpolished. The job is to add "juice" (game-feel feedback) to the
`After` scene only, while `Before` stays identical to the starter so the comparison shows only our feedback work.

## HARD RULES — gameplay must not change (15 marks + bonus eligibility depend on this)
1. **Gameplay stays identical**: level layout, ball speed, paddle controls, collision shapes, points, lives.
   No new gameplay systems (no levels, power-ups, menus, restart flow, hit-stop/`Time.timeScale` tricks).
2. **Never edit shared gameplay scripts** (`Scripts/Core/*`, `Scripts/Gameplay/*`). Add NEW components in
   `Scripts/Feedback/` that only listen to `GameEvents`.
3. **Shared assets are read-only.** If a prefab/material/physics material must change, DUPLICATE it and use the
   copy ONLY in `After.unity`.
4. **`Before.unity` is never modified** (not even opened-and-saved). All feedback goes in `After.unity`.
5. **Animate visual children, never physics objects** (no writes to Transform scale of collider objects,
   Rigidbody2D, Collider2D, or gameplay fields).

Details: `.claude/rules/` (loaded automatically):
- `assignment-requirements.md` — brief: five improvements, constraints, checklist, playtest, grading, bonus
- `unity-scenes-and-assets.md` — Before/After separation, duplication, scene/prefab/meta safety
- `feedback-scripts.md` — C# conventions for new feedback components (path-scoped to `*.cs`)
- `git-workflow.md` — submission contents, LFS, commit style
- `readme-and-video.md` — README sections, credits, AI statement, video script (path-scoped to README)

## Facts
- Brief: https://gamedevkfupm.github.io/swe402/individual%20assignments/game-feel/
- **Due: Monday 2026-11-16, 14:00 Asia/Riyadh (UTC+3).** Submission = one GitHub repo link on Blackboard.
- Editor: course targets Unity **6000.3.2f1**; this project is saved with **6000.3.27f1**
  (`ProjectSettings/ProjectVersion.txt`). The README must state the version actually used.
- Remote: `origin` → GitHub, branch `main`.

## Project map (paths relative to `Assets/_Project`)

| Path | What it is | May I edit? |
|---|---|---|
| `Scenes/Before.unity` | Original unpolished scene | **Never** |
| `Scenes/After.unity` | Duplicate of Before; all feedback goes here | Yes (add feedback only) |
| `Scripts/Core/GameEvents.cs` | Static events: `BallHitPaddle`, `BallHitWall`, `BrickDestroyed` (`ImpactInfo`), `BallLost` (no args), `ScoreChanged`, `LivesChanged` (`int`) | **No** |
| `Scripts/Core/ImpactInfo.cs` | `Point`, `Normal`, `Strength`, `Color`, `Center`, `Size`, `Combo` | **No** |
| `Scripts/Core/GameManager.cs` | Score / lives / combo singleton | **No** |
| `Scripts/Core/HUD.cs` | Score + lives TMP text; `ScoreHandledExternally`, `RefreshScore()` | **No** |
| `Scripts/Gameplay/*` | Ball, Paddle, Brick, DeathZone, LevelBuilder | **No** |
| `Prefabs/Brick.prefab`, `Bouncy.physicsMaterial2D` | Shared assets used by Before | Only via a duplicate used in After |
| `Scripts/Feedback/` | **New** feedback components (create here) | Yes |
| `Audio/`, `Effects/`, `Materials/` (new) | New clips, particle prefabs, duplicated materials | Yes |

Key facts from the code:
- Paddle visuals are on a **child** SpriteRenderer (`Paddle.Width` is measured once in `Awake`) — squash the child.
- Brick's SpriteRenderer is on the brick **root** (which has the collider), and `Brick` calls `Destroy(gameObject)`
  right after raising `BrickDestroyed`. Use `ImpactInfo.Center/Size/Color`; any disappearance animation must be a
  separate pooled visual stand-in, never the brick itself.
- `GameManager.OnBallLost()` order: (on game over: `ScoreChanged(0)`) → `LivesChanged` → `BallLost` → `ball.Launch()`
  (immediate relaunch, no pause).
- `ScoreChanged` also fires with 0 on game-over reset — score effects must not "celebrate" a reset.
- `Ball` forces its speed every `FixedUpdate`; never touch the ball Rigidbody2D/velocity or `Time.timeScale`.
- `GameEvents.ResetStatics()` clears subscribers on play start; still always unsubscribe in `OnDisable`.
- Input uses legacy `Input.GetAxisRaw` / mouse. Upstream comments are in Turkish.

## How we work
1. Claude writes C# components and assets; the user wires/tests them in the Unity Editor (Claude cannot run Unity).
   For every new component, give exact Inspector setup steps (GameObject, component, field values) for **After.unity**.
2. One small, self-contained `MonoBehaviour` per technique with serialized tuning fields and an enable toggle,
   so each improvement maps to one README table row.
3. Explain how each effect works so the user can explain it themselves (AI policy requires this).
4. After any change, remind the user what to verify in Play mode (repeated hits, rapid brick breaks, ball loss,
   game-over reset, muted audio, and that `Before.unity` is still unpolished).
5. Record tuning decisions (old → new values, why) and the playtest revision as we go, for the README and video.
6. Commit in small logical steps only when the user asks; push only when the user asks.

## Open decisions (update when settled)
- Intended feel: **decided — "Playful and bouncy"**: quick springy overshoot, bright brick-colored bursts,
  light cheerful sounds; restrained, never obscuring ball/paddle.
- Audio: the user records all sound effects with their own mouth → self-made assets; credit them as such in the
  README Credits.
- Progress (all in After.unity, committed):
  - #1 Brick impact — **done**: `BrickBurstParticles.cs` (brick-colored confetti + pastel `SparkleStar` sparkles).
  - #2 Reactive animation — **done**: `PaddleJiggle.cs` (squish build-up, teal→milky color shift, spring-out jiggle,
    tiny `FizzDot` fizz). Paddle/ball use AI-generated gem sprites (`GemPaddle`, `GemBall`, `FizzDot`) — credit in README.
    New sprites MUST stay exactly 1x1 unit: `Paddle.Width` is read from the visual sprite and drives bounce angle.
  - #5 Additional technique: **ball trail — done**: `BallFizzTrail.cs` (faint ribbon + sparkles; ribbon opacity ~12%).
  - #3 Event-based audio — **next** (mouth-recorded sounds). #4 Score UI animation — **next**.
- Extras done: `BrickImplode`, `LifeLossCue`, `CameraShake` (toggle `Shake Enabled`), Fredoka HUD theme, audio (13 trimmed mouth clips).
- Playtest revision (Section 4): tester cleared the board and didn't know they won -> `VictoryCelebration` plays a
  green-keyed "VICTORY" banner clip at score 280 (all bricks). **Feedback only: it must never pause the game, change
  Time.timeScale, or add a win/lose state** (brief: no new gameplay systems). Third-party clip: credit in README.
- Unity quirks: an unfocused Unity editor pauses Play mode (use `Application.runInBackground` only temporarily and
  restore `PlayerSettings.runInBackground = false`). Unity MCP tools exist (`mcp__unity-mcp__*`); `Debug.Log`
  formats need string concat, not `{0}` formatting.
- Which event combines two techniques: _likely BrickDestroyed = particles + sound (+ score pop)_.
