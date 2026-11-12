# Unity scenes, prefabs and assets

- **Never modify `Assets/_Project/Scenes/Before.unity`** (or its `.meta`). Before any commit, check
  `git diff --stat -- Assets/_Project/Scenes/Before.unity` is empty.
- All feedback objects/components are added to `Assets/_Project/Scenes/After.unity` only.
- Do not edit `Assets/_Project/Prefabs/Brick.prefab`, `Bouncy.physicsMaterial2D`, or any asset `Before` uses.
  Duplicate it (e.g. `Brick_After.prefab`) and reference the copy only from After.
- Prefer having the user wire things up in the Editor. Hand-editing `.unity`/`.prefab` YAML is a last resort:
  fileIDs and GUIDs must stay consistent, and a broken reference silently becomes "Missing".
- Every new asset needs a `.meta` with a unique GUID. Let Unity generate metas (user refreshes the Editor);
  never copy a `.meta` from another asset, and never delete/regenerate an existing `.meta` (breaks references).
- Keep new content under `Assets/_Project/` in clear folders: `Scripts/Feedback/`, `Audio/`, `Effects/`,
  `Materials/`, `Animations/`.
- Audio: short clips (wav/ogg). Credit every external or generated asset in the README Credits section.
- Particle systems: one-shot, `Stop Action` = Disable/Callback (pooled) or Destroy, sensible `Max Particles`,
  render above bricks (sorting layer/order) without hiding the ball.
- Unity reserializes some files when opened (`ProjectSettings.asset`, `.inputactions`). Commit those changes
  separately from feature work so diffs stay readable.
