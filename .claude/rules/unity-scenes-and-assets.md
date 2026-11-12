# Unity scenes, prefabs and assets

## Before / After separation
- **Never modify `Assets/_Project/Scenes/Before.unity`** (or its `.meta`) — don't even save it after opening.
  Before any commit, `git diff --stat -- Assets/_Project/Scenes/Before.unity` must be empty.
- All feedback objects/components are added to `Assets/_Project/Scenes/After.unity` only.
- In After, **do not change gameplay values** on existing objects: `Ball` (speed, angles), `Paddle` (xLimit,
  followSpeed), `LevelBuilder` (grid, sizes, colors, prefab rules), `GameManager` (startingLives), `Brick`
  (hitPoints, scoreValue), colliders, rigidbodies, `DeathZone`, walls, camera size and base position
  (a camera shake may only offset temporarily and must return exactly to the base).
- Shared assets are read-only: `Prefabs/Brick.prefab`, `Bouncy.physicsMaterial2D`, and anything Before references.
  To change one, **duplicate** it (e.g. `Brick_After.prefab`), change only visuals on the copy, and reference the
  copy only from After. A duplicated prefab must keep identical collider, physics material, hitPoints and scoreValue.
- Visual additions go on **child objects without colliders** (e.g. a sprite child, TrailRenderer, particles);
  never scale/move a GameObject that carries a Collider2D or Rigidbody2D.

## Editing safety
- Prefer having the user wire things up in the Editor. Hand-editing `.unity`/`.prefab` YAML is a last resort:
  fileIDs and GUIDs must stay consistent, and a broken reference silently becomes "Missing".
- Every new asset needs a `.meta` with a unique GUID. Let Unity generate metas (user refreshes the Editor);
  never copy a `.meta` from another asset, and never delete/regenerate an existing `.meta` (breaks references).
- Unity reserializes some files when opened (`ProjectSettings.asset`, `.inputactions`). Commit those changes
  separately from feature work so diffs stay readable.

## New content
- Keep it under `Assets/_Project/` in clear folders: `Scripts/Feedback/`, `Audio/`, `Effects/`, `Materials/`,
  `Animations/`.
- Audio: short, trimmed clips (wav/ogg), normalised to similar loudness. Our clips are self-recorded (mouth
  sounds); credit them as self-made, and credit every external or AI-generated asset, in the README.
- Particle systems: one-shot or emit-on-demand, finish cleanly (`Stop Action` Disable/Callback when pooled, or
  Destroy), sensible `Max Particles`, render above bricks (sorting layer/order) without hiding the ball/paddle.
