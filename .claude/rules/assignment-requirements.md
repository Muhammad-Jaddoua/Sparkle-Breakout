# Assignment requirements (source of truth: the course brief)

## The five required improvements (8 marks each)
| # | Improvement | Required behaviour | Success check |
|---|---|---|---|
| 1 | Brick impact effect | Particle effect when a brick is destroyed | Appears at the destroyed brick and finishes cleanly |
| 2 | Reactive animation | Paddle or ball reacts on collision (squash/stretch, recoil…) | Returns to normal; collider/physics unchanged |
| 3 | Event-based audio | Distinct sounds for paddle contact, brick destruction, ball loss | Each plays on the correct event, stays balanced on repeats |
| 4 | Interface feedback | Score display animates when points are added | Briefly emphasises, shows the correct score, returns to normal |
| 5 | Additional technique | A *different* technique (ball trail, brick disappearance, life-loss cue, restrained camera shake…) | Clear purpose, works reliably |

- The three sounds together count as **one** improvement.
- Variations of the same particle effect are **not** separate techniques.
- At least **one event must combine two complementary techniques** (e.g. sound + particles on brick destruction).
- An intended feel must be chosen and stated in the README; every effect's timing/intensity should serve it.

## Hard constraints (breaking these costs the 15 "unchanged gameplay" marks and bonus eligibility)
- Do **not** change: level layout, ball speed, paddle controls, collision shapes, points, lives.
- Animate **visual children**, never physics objects / colliders / rigidbodies.
- Do **not** add gameplay systems (no new levels, power-ups, menus, restart flow).
- `Before.unity` must keep its original behaviour. Add new components instead of editing shared gameplay scripts.
  If a shared prefab/material must change, duplicate it and use the copy only in `After`.
- Do not import JuiceLab's completed feedback modules or any finished game-feel package as our own work.

## Tune-and-verify checklist (run before calling an improvement done)
- [ ] Fires on the right event only, never continuously by accident.
- [ ] Ball and paddle stay readable at all times.
- [ ] Repeated collisions: no permanent stretch, drift/displacement, or effect pile-up.
- [ ] Temporary effects finish or are pooled/reused; no leaked GameObjects in the Hierarchy.
- [ ] Audio balanced under rapid repeats (pitch/volume variation, voice limiting).
- [ ] Score is correct during rapid brick hits and after game-over reset to 0.
- [ ] With audio muted, brick destruction and ball loss are still visible.
- [ ] Camera shake (if used) has a documented Inspector toggle to disable it.
- [ ] Reopen `Before.unity` and confirm it is still unpolished.

## Playtest
- One other person plays Before and After. Ask: which feels more satisfying and why; which feedback clarifies events;
  what is distracting or hard to notice.
- Make **at least one** revision from their observations; README records what they noticed, what changed, what was
  observed afterwards. **No tester name.**

## Grading (100 + 10 bonus)
| Criterion | Marks |
|---|---|
| Five improvements (correct trigger, reliable repeats, stated result) | 40 |
| Coherent timing, intensity, readability, intended feel | 25 |
| Working Before/After scenes, unchanged core gameplay | 15 |
| Playtest evidence and justified revision | 10 |
| Complete project, README, video, attribution, AI-use statement | 10 |

Top-three bonus (+10) is ranked on the first 65 marks. Tie-breakers: tuning & readability → reliability during
repeated play → clarity of the Before/After comparison. Aim for polish and restraint, not quantity.
