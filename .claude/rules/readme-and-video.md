---
paths:
  - "README.md"
---

# README and demonstration video

## README must contain (replace the starter template)
- Name and student ID (ask the user; never invent).
- Unity version actually used, scene paths (`Assets/_Project/Scenes/Before.unity`, `.../After.unity`),
  controls (mouse or A/D / ←/→; ball launches automatically), run instructions.
- Intended feel, one or two sentences on how the effects serve it.
- Five-row table: improvement | trigger (GameEvents event) | implementation file/object | purpose.
  Mark which event combines two techniques. Note the Inspector toggle that disables camera shake if used.
- Playtest: what the tester noticed, what was changed, what was observed afterwards. **No tester name.**
- Accessible video link (unlisted YouTube / Drive with "anyone with link").
- Credits: keep "Starter adapted from JuiceLab by Serkan Ceyhan, MIT — see LICENSE and SOURCE.md"; list every
  external/generated audio, texture, font, code snippet and tool.
- **AI use statement**: tools used (Claude Code, model), what they produced (scripts, setup steps, README drafts…),
  and how the user reviewed, tested and tuned the output in Unity.
- Keep `LICENSE` and `SOURCE.md` unchanged.

## Video: 2–3 min, narrated, with game audio, Before/After labelled
1. 20–30 s of `Before` gameplay.
2. `After`: paddle contact, brick destruction, score changes, a deliberate ball loss — name all five improvements.
3. Walk through one script / animation / Inspector config: its trigger and one tuned parameter (before → after value, why).
4. Playtest feedback and the resulting revision.
