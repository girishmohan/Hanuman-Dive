# Hanuman: Ramayana Edition (work in progress)

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
