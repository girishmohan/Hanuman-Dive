# Hanuman Dive (work in progress)

Based on Hyper Dive by Prajwalvv (https://github.com/Prajwalvv/Hyper-Dive). Credit the original author, and confirm his permission before publishing.

## Phase 1 / 2 starter: chapter system

New script: `hyper dive/Assets/scripts/chapterManager.cs`

Already hooked up: `SampleScene.unity` has a `RamayanaDirector` object carrying `chapterManager` (step 3).

Optional extras in Unity:
1. Create two UI Texts (title and subtitle) and a distance Text, then drag them into the matching `chapterManager` fields.
2. Drag the Main Camera into `Background Camera` to tint the sky per chapter.
3. Tune `Distance Per Second` and each chapter's `Start Distance`.

## Status
- Step 1: Unity 2022.3 upgrade. Ads/IAP disabled: plugins parked in `hyper dive/_disabled_monetization/`, scripts gated by the `HD_ADS` / `HD_IAP` scripting defines.
- Step 2: oxygen is now Rama Naam energy (`move.ramaNaamEnergy`, HUD shows "Rama Naam NN%").
- Step 3: first obstacle set, `ramayanaObstacleSet.cs` on `RamayanaDirector`. "The Great Leap" (chapter index 1) spawns placeholder shapes:
  - Surasa's Jaws: green wall with a gap (34 damage)
  - Simhika's Shadow: dark orb that drifts toward Hanuman (25)
  - Ocean Rocks: grey squares (10 each)
  - Mainaka Peak: gold diamond pickup (+15 energy)

  Tick `Always Spawn` to playtest without waiting for the chapter. All damage, speed and colour values are in the Inspector.
- Placeholder Hanuman: `A-Z_Materials/hanuman/hanuman_placeholder.png`, used by all 10 player skins in `SampleScene`. Regenerate or tweak it with `python3 tools/make_placeholder_hanuman.py`. The menu, start and skin-shop scenes still show the diver. To use real art, replace that PNG with one of the same size and pose (600x600, arms up).

## Look and feel (Oct 2026 batch)
- Player: refined Muse Hanuman (`hanuman/hanuman_player.png`) everywhere; the 8 AVATAR shop entries are the same Hanuman with different auras (`buyPlayer/Group 27-34.png`, made by `tools/make_hanuman_forms.swift`).
- HUD and pickups: the oxygen tank is now a saffron R-A-M badge (`o2/02_5.png`).
- Font: Baloo 2 (SIL Open Font License, `fonts/Baloo2/OFL.txt`) for all UI text. The text images are redrawn by `tools/render_ui_text.swift`; edit its LABELS table to change wording.
- Falling backdrop: dawn sky palette (ColorCycler on the gameplay camera) plus drifting temple, Lanka, palm, lotus, cloud and mountain silhouettes (`ramayanaBackdrop.cs`).
- Rakshasas: blue and green demons weave upward from 12 s, 40 damage (`ramayanaDemons.cs`, sprites from `tools/make_ramayana_sprites.swift`).
- Music: original Raag Yaman loop per theme-brief.pdf (`tools/make_ramayana_music.py`): bansuri over a sa-pa tanpura, 66 BPM, 72.7 s, 44.1 kHz stereo WAV. Unity re-encodes it for iOS/Android.
- Simulator: `Assets/DeviceSimulator/` adds two modern Dynamic Island iPhone profiles with no bezel overlay.

## Mapping from the original game

| Original | Ramayana version |
|---|---|
| Oxygen (`move.health`, `heartGemLife`) | Rama Naam energy |
| Obstacles (`obstacles.cs`) | Mainaka, Surasa, Simhika (done, placeholder); Lanka walls, arrows, fire |
| Gems (`levelManager.coins`) | Lotus petals |
| Skins (`shopManager`, `levelManager`) | Hanuman forms |

## Before publishing
- Replace placeholder AdMob IDs in `Admanager.cs` and `Admob.cs` with your own.
- Configure your own in-app purchase product IDs in `IAPManager.cs`.
- Replace all placeholder art and audio with original or licensed assets.
- Confirm in writing that the Muse-generated images (menu background, player) may be used commercially.

## Journey and milestones
- Levels follow the METERS counter: 0 / 150 / 400 / 700 / 1000 / 1400 / 1800 / 2300 m, then the endless Infinite Ram from 2500 m (`chapterManager.DefaultChapters`). The start screen's CHAPTERS panel lists them as text rows (`Panel/LevelRows` in start.unity).
- One milestone per chapter (`ramayanaMilestones.Defaults`). Saved as PlayerPrefs `Milestone_0` to `Milestone_7`; delete them to reset.
- The main menu shows the next goal under the title, plus a JOURNEY panel (`ramayanaJourneyMenu.cs`, built at runtime).
- The sky and drifting scenery change per chapter (`ramayanaBackdrop.chapterScenery`). The old neon ColorCycler on the gameplay camera is disabled.
- Rakshasa bumps cost 5% energy.
