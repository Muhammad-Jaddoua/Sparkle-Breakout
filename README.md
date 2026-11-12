# SWE402 Assignment 3: Improving Game Feel

A brick-breaker from the course's JuiceLab starter, with game-feel feedback added in a separate `After` scene.

## Student
Name and student ID: **Muhammad Jaddoua, 202246240**

## Run
- **Unity version:** 6000.3.27f1 (Unity 6.3; the course specifies 6000.3.2f1, same 6000.3 line)
- **Scenes** (both in `Assets/_Project/Scenes/`):
  - `Before.unity` — the original, unpolished starter. Unchanged.
  - `After.unity` — a duplicate of `Before` with all the feedback added.
- **How to run:** open the project in Unity Hub with Unity 6000.3 and let the packages import. Open a scene, set the Game view to 16:9, press Play and click inside the Game view.
- **Controls:** move the paddle with the mouse, or A/D, or the left/right arrow keys. The ball launches automatically. Losing all three lives resets score and lives; stop and restart Play mode for a fresh board.
- **Camera shake setting:** select `Main Camera` in `After`, and untick **Shake Enabled** on the `Camera Shake` component to turn the shake off.

Gameplay is unchanged between the scenes: level layout, ball speed, paddle controls, collision shapes, points and lives. All feedback is added as new scripts and objects. No starter gameplay script was edited, and all animation is done on visual child objects, never on colliders or physics bodies.

## Intended feel
**Playful and bouncy.** Everything is sparkly and party-like: a jello-gem paddle that squishes and wobbles on every hit, confetti and pastel sparkles when bricks pop, a rounded bubbly HUD font, and cheerful mouth-made sound effects. Effects are quick and springy, with overshoot, and kept small or faint enough that the ball and paddle always stay readable.

## Improvements
| Improvement | Trigger | Implementation file/object | Purpose |
|---|---|---|---|
| Brick impact | `GameEvents.BrickDestroyed` | `BrickImplode.cs` + `BrickBurstParticles.cs` on `BrickImplodeFX` and `BrickBurstFX` (`Confetti` and `Sparkles` particle systems) | The brick collapses into itself, then bursts into brick-coloured confetti and pastel sparkles at that spot, so a break feels like a satisfying pop. |
| Reactive animation | `GameEvents.BallHitPaddle` | `PaddleJiggle.cs` on `Paddle` (animates only its `Visual` child), plus `PaddleFizz` particles | The jello paddle builds up a squish while its colour brightens, snaps back, springs out taller and wobbles to rest. Tiny sparkles fizz from the contact point. The collider and physics never change. |
| Event-based audio | `BallHitPaddle`, `BrickDestroyed`, `BallLost` | `FeedbackAudio.cs` on `FeedbackAudio`; clips in `Assets/_Project/Audio/` | Three distinct sound families (paddle boing, brick pop, ball-lost wah), each with several clips picked at random, small pitch variation, rising pitch on brick streaks and a capped voice pool. |
| Interface feedback | `GameEvents.ScoreChanged` (score), `GameEvents.BallLost` (lives) | `TextPunch.cs` on `ScoreText` and `LivesText` | The score text springs up with a pink flash whenever points are added, then returns to normal. The lives text pops on a lost ball. The number itself is still written by `HUD`, so it is always correct. |
| Additional technique | Continuous (ball movement); `BallLost` | `BallFizzTrail.cs` on `Ball/BallFX` (ribbon trail + sparkles) | A faint shooting-star trail keeps the ball easy to follow. Also added: `LifeLossCue.cs` (soft pink edge flash) and `CameraShake.cs` (small shake, switchable) so a lost ball is clear even with the sound muted. |

**Event that combines two techniques:** a destroyed brick combines the implode animation, the confetti and sparkle burst, and the pop sound.

**Tuned parameters (examples):**
- `PaddleJiggle`: Squish Depth 0.7, Release Kick 18, Damping 0.14
- `BrickImplode`: Collapse Time 0.10 s
- `FeedbackAudio`: Brick Sound Delay 0.12 s, so the sound lands on the pop
- Ball trail ribbon opacity about 12%
- `CameraShake`: Magnitude 0.3

## Playtest and revision
**What the tester noticed:** A playtester cleared all the bricks and could not tell what had happened. Their words were roughly: "how do I win, I don't know if I won, why are there no more blocks?" In `Before` (and in my first `After`), clearing the board just left the ball bouncing in an empty arena, with nothing to say the player had won.

**What I changed:** I added a victory celebration (`VictoryCelebration.cs`, object `VictoryCelebration` plus the `VictoryBanner` overlay). When the score reaches 280, which is all 28 bricks cleared at 10 points each, a short "VICTORY" banner clip plays over the game with its sound, with the green screen removed by a small shader. It is feedback only: it does not pause or end the game, and the score, lives and rules are unchanged, so the ball keeps bouncing as in the starter. I also erased a leftover "12.37 seconds" timer from the clip, because it would have been confusing. It can replay after a game-over reset brings the score back below 280.

**What I observed afterwards:** I had the same tester play the new version and clear the board. They said it was "much better" and that they now knew when they had won.

## Demonstration video
Link: _to be added_

## AI use
- **Tool:** Claude Code (Anthropic; models Claude Opus 5.5 and Claude Sonnet 5.5), used together with the Unity MCP bridge so it could edit and test in the Unity Editor.
- **What it produced:** the C# feedback scripts (`PaddleJiggle`, `BallFizzTrail`, `BrickBurstParticles`, `BrickImplode`, `FeedbackAudio`, `TextPunch`, `LifeLossCue`, `CameraShake`), the editor setup of particle systems and UI, the generated gem, sparkle, fizz and vignette sprites, `VictoryCelebration.cs` and the green-screen chroma-key shader for the victory banner, the `ffmpeg` and Python script that converted, trimmed and levelled my recorded sounds, the project notes (`CLAUDE.md` and `.claude/rules/`), and a first draft of this README.
- **How I tested and revised it:** I played both scenes in Unity, listened to every sound clip, and tuned the feel by giving direct feedback (for example, making the ball trail fainter and the paddle more jiggly). The assistant also ran checks in the editor, such as repeated hits returning the paddle to exactly its original size, rapid score hits showing the correct number, and the camera returning to its original position. I can explain how each effect works.

## Credits
- Starter adapted from [JuiceLab](https://github.com/serkoceyhan/JuiceLab) by Serkan Ceyhan, MIT. See `LICENSE` and `SOURCE.md`.
- **Sound effects:** all sounds were recorded by me with my own voice and mouth, and trimmed and levelled with ffmpeg and Python.
- **Font:** [Fredoka](https://github.com/google/fonts/tree/main/ofl/fredoka), SIL Open Font License 1.1 (`Assets/_Project/Fonts/Fredoka-OFL.txt`).
- **Victory banner clip:** the "VICTORY" green-screen clip is from the *Happy Wheels* victory screen (Happy Wheels is by Jim Bonacci / Fancy Force; all rights belong to its creators). I obtained it as a green-screen video from YouTube (source URL: _add the link you downloaded it from_). It is used only for this coursework. I removed the on-screen timer text with ffmpeg and removed the green background in Unity with my own shader (`Assets/_Project/Shaders/UIChromaKey.shader`).
- **Sprites:** the gem paddle, gem ball, sparkle star, fizz dot and vignette images were generated with AI assistance (Claude Code scripts) specifically for this project. The confetti square and pop circle are simple shapes made in the Unity Editor.
- **Tools:** Unity 6000.3, FFmpeg (audio conversion and trimming only).
