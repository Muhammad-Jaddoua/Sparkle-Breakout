# Git workflow

- Repo root is this Unity project folder; remote `origin`, branch `main`. Submission = this repo's link.
- Required in the repo: `Assets/`, `Packages/`, `ProjectSettings/`, `README.md`. Also commit `LICENSE`, `SOURCE.md`
  (keep both unchanged), `START-HERE.md`, `CLAUDE.md`, `.claude/rules/`.
- Never commit `Library/`, `Temp/`, `Logs/` (brief requires excluding them), nor `UserSettings/`, `obj/`, builds,
  `*.csproj`/`*.sln`, `.claude/settings.local.json` (all in `.gitignore`). No standalone build needed.
- `.gitattributes` routes audio (`*.wav`, `*.ogg`, `*.mp3`), images, fonts and video through **Git LFS**.
  Run `git lfs install` once per machine. Do not commit the demo video; link it from the README.
- Every `.meta` file must be committed together with its asset.
- Small, logical commits in imperative mood, one improvement per commit where possible
  (e.g. `Add paddle squash to After scene`). Keep editor reserialization noise in its own commit.
- Before committing: `git status`, and confirm `Before.unity`, `Scripts/Core/`, `Scripts/Gameplay/`,
  `Prefabs/Brick.prefab` and `Bouncy.physicsMaterial2D` are untouched (`git diff --stat` on them is empty).
- Commit or push only when the user asks. Never force-push or rewrite pushed history.
