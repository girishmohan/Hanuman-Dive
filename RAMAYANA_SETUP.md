# Hanuman: Ramayana Edition (work in progress)

Based on Hyper Dive by Prajwalvv (https://github.com/Prajwalvv/Hyper-Dive). Credit the original author, and confirm his permission before publishing.

## Phase 1 / 2 starter: chapter system

New script: `hyper dive/Assets/scripts/chapterManager.cs`

Hook it up in Unity:
1. Open the gameplay scene (check `SampleScene.unity` / `start.unity` for the one with the player and score UI).
2. Create an empty GameObject named `ChapterManager` and add the `chapterManager` component. The 8 Ramayana chapters fill in automatically.
3. Optional: create two UI Texts (title and subtitle) and a distance Text, then drag them into the matching fields. Drag the Main Camera into `Background Camera` to tint the sky per chapter.
4. Press Play. Chapter banners appear as distance grows. Tune `Distance Per Second` and each chapter's `Start Distance`.

## Mapping from the original game

| Original | Ramayana version |
|---|---|
| Oxygen (`move.health`, `heartGemLife`) | Rama Naam energy |
| Obstacles (`obstacles.cs`) | Mainaka, Surasa, Lanka walls, arrows, fire |
| Gems (`levelManager.coins`) | Lotus petals |
| Skins (`shopManager`, `levelManager`) | Hanuman forms |

## Before publishing
- Replace placeholder AdMob IDs in `Admanager.cs` and `Admob.cs` with your own.
- Configure your own in-app purchase product IDs in `IAPManager.cs`.
- Replace all placeholder art and audio with original or licensed assets.
