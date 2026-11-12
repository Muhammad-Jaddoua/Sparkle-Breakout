# SWE402 Assignment 3 — Improving Game Feel (JuiceLab Dead Starter)

Unity 2D brick-breaker that is deliberately unpolished. The job is to add "juice" (game-feel feedback) to the
`After` scene only, while `Before` stays identical to the starter.

- Brief: https://gamedevkfupm.github.io/swe402/individual%20assignments/game-feel/
- **Due: Monday 2026-11-16, 14:00 Asia/Riyadh (UTC+3).** Submission = GitHub repo link on Blackboard.
- Editor: course targets Unity **6000.3.2f1**; this project is currently saved with **6000.3.27f1**
  (`ProjectSettings/ProjectVersion.txt`). The README must state the version actually used.
- Remote: `origin` → GitHub, branch `main`.

Detailed rules live in `.claude/rules/` and are loaded automatically:
- `assignment-requirements.md` — the five improvements, constraints, grading, bonus tie-breakers
- `unity-scenes-and-assets.md` — Before/After separation, scene/prefab/meta editing safety
- `feedback-scripts.md` — C# conventions for new feedback components (path-scoped to scripts)
- `git-workflow.md` — what to commit, LFS, commit style
- `readme-and-video.md` — README sections, playtest log, video script (path-scoped to README)

## Project map (paths relative to `Assets/_Project`)

| Path | What it is | May I edit? |
|---|---|---|
| `Scenes/Before.unity` | Original unpolished scene | **Never** |
| `Scenes/After.unity` | Duplicate of Before; all feedback goes here | Yes |
| `Scripts/Core/GameEvents.cs` | Static events: `BallHitPaddle`, `BallHitWall`, `BrickDestroyed` (`ImpactInfo`), `BallLost`, `ScoreChanged`, `LivesChanged` (`int`) | No (read-only hook source) |
| `Scripts/Core/ImpactInfo.cs` | `Point`, `Normal`, `Strength`, `Color`, `Center`, `Size`, `Combo` | No |
| `Scripts/Core/GameManager.cs` | Score / lives / combo singleton | No |
| `Scripts/Core/HUD.cs` | Score + lives TMP text; `ScoreHandledExternally`, `RefreshScore()` | No |
| `Scripts/Gameplay/*` | Ball, Paddle, Brick, DeathZone, LevelBuilder | No |
| `Prefabs/Brick.prefab` | Shared brick prefab | Only via a duplicate used in After |
| `Scripts/Feedback/` | **New** feedback components (create here) | Yes |
| `Audio/`, `Effects/`, `Materials/` (new) | New clips, particle prefabs, duplicated materials | Yes |

Key facts from the code:
- Paddle visuals are on a **child** SpriteRenderer (`Paddle.Width` is measured once in `Awake`) — squash the child.
- `Brick` calls `Destroy(gameObject)` right after raising `BrickDestroyed`; use `ImpactInfo.Center/Size/Color`, never the brick reference.
- `GameManager.OnBallLost()` raises `ScoreChanged(0)` (on game over) *before* `LivesChanged` and `BallLost`, then relaunches the ball immediately.
- `GameEvents.ResetStatics()` clears subscribers on play start; still always unsubscribe in `OnDisable`.
- Ball speed is forced every `FixedUpdate`, so never touch ball Rigidbody/velocity/timeScale for effects
  unless it is purely visual (hit-stop via `Time.timeScale` changes game timing — avoid or keep it opt-in and tiny).
- Input uses legacy `Input.GetAxisRaw` / mouse.

## How we work

1. Claude writes C# components and assets; the user wires/tests them in the Unity Editor (Claude cannot run Unity).
   For every new component, give exact Inspector setup steps (GameObject, component, field values) for **After.unity**.
2. Prefer one small, self-contained `MonoBehaviour` per technique with serialized tuning fields and an `enabled`
   toggle, so each improvement maps cleanly to one README table row.
3. After any change, remind the user what to verify in Play mode (repeated hits, rapid brick breaks, ball loss,
   muted audio, and that `Before.unity` is still unpolished).
4. Record tuning decisions and the playtest revision as we go so the README and video script are easy to write.
5. Commit in small logical steps; push only when the user asks.

## Open decisions (update when settled)
- Intended feel: _not chosen yet_ (e.g. playful & bouncy / heavy & powerful / clean & precise).
- Additional technique (#5): _not chosen yet_.
- Which event combines two techniques: _likely BrickDestroyed = particles + sound (+ score pop)_.
