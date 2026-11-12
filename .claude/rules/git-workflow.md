# Git workflow

- Repo root is this Unity project folder; remote `origin`, branch `main`.
- Commit `Assets/`, `Packages/`, `ProjectSettings/`, `README.md`, `LICENSE`, `SOURCE.md`, `START-HERE.md`,
  `CLAUDE.md`, `.claude/rules/`. Never commit `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/`, builds,
  `*.csproj`/`*.sln`, or `.claude/settings.local.json` (all in `.gitignore`).
- `.gitattributes` routes audio (`*.wav`, `*.ogg`, `*.mp3`), images, fonts and video through **Git LFS**.
  Run `git lfs install` once per machine. Do not commit the demo video into the repo; link it instead.
- Every `.meta` file must be committed together with its asset.
- Small, logical commits in imperative mood, one improvement per commit where possible
  (e.g. `Add brick burst particles to After scene`). Keep editor reserialization noise in its own commit.
- Before committing: `git status` and confirm `Before.unity` is untouched.
- Push only when the user asks. Never force-push or rewrite pushed history.
