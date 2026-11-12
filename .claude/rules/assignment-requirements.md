# Assignment requirements (source of truth: the course brief)

Individual assignment. Use the course starter (not the completed upstream JuiceLab demo), Unity 6000.3.2f1.

## The five required improvements (8 marks each: correct trigger, reliable repeats, stated result)
| # | Improvement | Required behaviour | Success check |
|---|---|---|---|
| 1 | Brick impact effect | Particle effect at the destroyed brick | Appears at the brick and finishes cleanly |
| 2 | Reactive animation | Paddle or ball reacts on collision (squash/stretch or recoil) | Returns to normal; collider and physics unchanged |
| 3 | Event-based audio | Distinct sounds for paddle contact, brick destruction, ball loss | Each plays on the correct event, stays balanced on repeats |
| 4 | Interface feedback | Score display animates when points are added | Briefly emphasises the change, shows the correct score, returns to normal |
| 5 | Additional technique | A *different* technique, e.g. ball trail, brick disappearance animation, life-loss cue, restrained camera shake (safest: pick one of these) | Clear purpose, works reliably |

- The three sounds together count as **one** improvement.
- Variations of the same particle effect are **not** separate techniques.
- At least **one event must combine two complementary techniques** (e.g. sound + particles on brick destruction).
- Choose an intended feel, state it in the README; every effect's timing/intensity must serve it.
- Unity components, animation clips, C#, tweening tools and AI are allowed; we must integrate and tune the result.
  More effects ≠ better feel.

## Hard constraints (breaking these costs the 15 "unchanged gameplay" marks and bonus eligibility)
- Do **not** change: level layout, ball speed, paddle controls, collision shapes, points, lives.
- Do **not** add gameplay systems (no new levels, power-ups, menus, restart flow). One board only; restarting
  Play mode gives a fresh board; game over resets score/lives but not the board — leave it that way.
- Animate **visual child objects**, never physics objects / colliders / rigidbodies.
- `Before.unity` stays unchanged; all feedback goes in `After.unity`.
- Add **new** scripts/components; do not edit shared gameplay scripts (`Scripts/Core`, `Scripts/Gameplay`).
- To change a shared prefab/material, **duplicate** it and use the duplicate only in `After`.
- Do not submit JuiceLab's completed feedback modules or any other finished game-feel solution as our own.
- Not needed: a live Before/After toggle, a standalone build, a separate report.

## Tune-and-verify checklist (run before calling an improvement done)
- [ ] Fires on the intended event only, never accidentally or continuously.
- [ ] Ball and paddle stay readable throughout play.
- [ ] Repeated collisions: no permanent stretch, displacement, or pile-up of growing effects.
- [ ] Temporary effects finish or are reused (pooled); audio stays balanced under rapid repeats.
- [ ] Score is correct during rapid brick hits (and after the game-over reset to 0).
- [ ] With audio muted, brick destruction and ball loss are still visible (game state or our feedback).
- [ ] Camera shake (if used) has a documented Inspector setting that disables it.
- [ ] Reopen `Before.unity` and confirm it still has the original unpolished behaviour.

## Playtest and revise
- One other person plays Before and After. Ask: which feels more satisfying and why; which feedback makes events
  clearer; is anything distracting or hard to notice.
- Make **at least one** revision based on what they observed (small, justified tuning is enough).
- README records their observations, our change, and what we saw afterwards. **No tester name.**

## Grading (100 + up to 10 bonus)
| Criterion | Marks |
|---|---|
| Five required improvements (8 each) | 40 |
| Coherent timing, intensity, readability, intended feel | 25 |
| Working Before/After scenes with unchanged core gameplay | 15 |
| Playtest evidence and justified revision | 10 |
| Complete project, README, video, attribution, AI-use statement | 10 |

**Top-three bonus (+10 each):** ranked on the first 65 marks (rows 1+2), not in-game score. Tie-breakers in order:
tuning & readability → reliability during repeated play → clarity of the Before/After improvement → short live
demo. Eligibility requires all required work done and original gameplay rules intact. AI use neither earns nor
loses marks. Aim for polish and restraint, not quantity.
