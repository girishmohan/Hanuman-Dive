# Hanuman Dive

A devotional endless-faller for iOS and Android, built in Unity. Hanuman leaps across the ocean toward Lanka to find Sita, dodging spikes, Surasa's jaws, Simhika's shadow and Ravana's rakshasas, and chanting his way forward on **Rama Naam** energy.

<p align="center">
  <img src="images/menu.png" width="200" alt="Main menu: Hanuman Dive with the next goal">
  &nbsp;
  <img src="images/journey.png" width="200" alt="Journey panel: chapters, distances and milestones">
  &nbsp;
  <img src="images/gameplay.png" width="200" alt="Gameplay in the Lanka chapter with a milestone and a rakshasa">
  &nbsp;
  <img src="images/forms_shop.png" width="200" alt="AVATAR shop with Hanuman avatars">
</p>

**▶ Play in your browser: https://girishmohan.github.io/Hanuman-Dive/**
On a phone, drag to steer. On a computer, use the mouse or the ← → arrow keys.

> Work in progress. This game is a fork of [Hyper Dive](https://github.com/Prajwalvv/Hyper-Dive) by **Prajwalvv**, re-themed around the Ramayana. Full credit for the original game goes to him (see [Credits](#credits)).

## The journey

| Level | Starts at | Milestone to collect |
|---|---|---|
| Hanuman Leaves in Search of Sita | 0 m | Rama's Ring (appears by 20 m) |
| The Great Leap | 150 m | Menaka's Blessing |
| The Golden City of Ravana | 400 m | Lankini's Gate |
| The Search for Sita | 700 m | Ring for Sita: meet Sita (needs Rama's Ring from the same run) |
| Rakshasa Battle | 1000 m | Hanuman's Gada |
| Lanka Burns | 1400 m | Light the Fire |
| The Mountain of Healing Herbs | 1800 m | Sanjeevani herb |
| Victory · Jai Shri Ram | 2300 m | Rama and Sita Reunited |
| Infinite Ram | 2500 m + | Endless: keep flying |

Distances use the in-game **METERS** counter. Each chapter has its own painted background (cross-fading as you enter it), drifting scenery and a banner. A missed milestone comes back about 60 m later while its chapter lasts. Completed milestones are saved and shown on the main menu's **JOURNEY** panel, next to a "Next goal" line under the title.

## What's in the game

- **Rama Naam energy.** It slowly drains as you fall. Collect saffron **RAM** tablets to refill it; when it hits zero the run ends.
- **The Great Leap hazards.**
  - Surasa's Jaws: a wall with a narrow gap to slip through.
  - Simhika's Shadow: an orb that chases you.
  - Ocean rocks.
  - Mainaka Peak: a golden pickup that restores energy.
- **Rakshasas.** Ravana's demon warriors weave upward toward Hanuman from about 12 seconds into every run. Each bump costs 5% energy.
- **Vanara allies.** Angada, a vanara archer and Jambavan rise between the obstacles every 15 to 25 seconds. Meet one for +10% energy and a cheer.
- **The original obstacles** are still here: spiked mines, saw blades and canyon walls. The mines hold off for the first 8 seconds.
- **Eight Hanuman avatars** to unlock with gems in the **AVATAR** shop (500 to 100,000 gems). Each is the same Hanuman with a different aura.
- **Gems:** diamonds are worth 10 / 25 / 50 / 80 / 90, and each new chapter pays +50, so a decent run earns about 150–250 gems.
- **Story beats:** a character greets each chapter (Jambavan, the vanara archer, Lanka's guards, Sita, Angada, Rama & Sita) with a line on the banner.
- **Fair spawning:** a spawn guard makes sure every row of hazards leaves Hanuman a gap.
- **Why you died:** Game Over names the cause (energy ran out, Patala Lok spikes, the Asuras, Surasa...) with a tip, and first runs show a reminder that RAM tablets refill energy.
- **Chapter scenery.** The sky and drifting silhouettes follow the story: palms in Kishkindha, waves over the ocean, towers and flags in Lanka, Ashoka trees, flames as Lanka burns, herb mountains, and temples, diyas and bells for Victory.
- **Original music.** A serene Raag Yaman bansuri over a tanpura drone, as a seamless 73-second loop.
- **Fits any phone.** It draws edge to edge, and the HUD keeps clear of notches, the Dynamic Island and the home bar.

![Hanuman, Rama & Sita, Menaka and the rakshasas](images/cast.png)

## Controls

| Where | How |
|---|---|
| Phone | Drag left or right to steer Hanuman |
| Unity editor | Drag with the mouse, or use the ← → arrow keys (A / D) |

## Open the project

1. Install **Unity 2022.3.62f3** via Unity Hub. On Apple Silicon Macs, install Rosetta 2 too (`softwareupdate --install-rosetta`), because Unity's package manager needs it.
2. In Unity Hub, choose **Add → Add project from disk** and select the **`hyper dive`** folder inside this repo.
3. Open `Assets/Scenes/start.unity` for the menu, or `SampleScene.unity` to jump straight into a run.
4. To see it as a phone, switch the Game view to **Simulator** and pick **Modern iPhone 6.3in (Dynamic Island)**. That profile lives in `Assets/DeviceSimulator/`.

Ads and in-app purchases are switched off for now. Their plugins are parked in `hyper dive/_disabled_monetization/`, and the scripts compile as stubs unless you add the `HD_ADS` / `HD_IAP` scripting defines.

## Where things live

| Area | Files |
|---|---|
| Chapters and distance | `scripts/chapterManager.cs` |
| Ramayana hazards (Surasa, Simhika, rocks, Mainaka) | `scripts/ramayanaObstacleSet.cs` |
| Milestones | `scripts/ramayanaMilestones.cs` |
| Main-menu Journey panel and next goal | `scripts/ramayanaJourneyMenu.cs` |
| Rakshasas | `scripts/ramayanaDemons.cs` |
| Vanara allies | `scripts/ramayanaVanaras.cs` |
| Drifting scenery | `scripts/ramayanaBackdrop.cs` |
| Hanuman, steering, Rama Naam energy | `scripts/move.cs` |
| Safe-area handling for notches | `scripts/safeAreaFitter.cs` |
| Setup notes and status | [`RAMAYANA_SETUP.md`](RAMAYANA_SETUP.md) |

All paths are under `hyper dive/Assets/`. Everything in the game objects lives on the **RamayanaDirector** object in `SampleScene`, where damage, speed and timing can be tuned in the Inspector.

### Regenerating art and music

These scripts are in `tools/` and run on macOS:

| Script | Makes |
|---|---|
| `swift tools/render_ui_text.swift` | All button and label images in Baloo 2, plus the RAM badge |
| `swift tools/make_hanuman_forms.swift` | The eight Hanuman avatars |
| `swift tools/make_ramayana_sprites.swift` | Scenery silhouettes and milestone icons (the rakshasa and character art comes from Muse) |
| `python3 tools/make_ramayana_music.py` | The Raag Yaman theme loop |

## Web version

1. In Unity, choose **Tools → Hanuman Dive → Build WebGL**. This writes `WebBuild/` using the portrait page template in `Assets/WebGLTemplates/HanumanDive`.
2. Run `tools/publish_web.sh`. It copies the build to the `gh-pages` branch, which GitHub Pages serves.

## Before publishing

- [x] Permission from Prajwalvv to publish a game based on Hyper Dive (given; record kept).
- [x] Credit Prajwalvv inside the game: main menu and Journey panel (add it to any future store listing too).
- [x] Muse terms allow commercial use of the generated artwork (terms saved).
- [x] `Deadly Hazards & Obstacles` art pack is a free asset.
- [ ] For the App Store only: set your own bundle ID and company name, and re-enable current ad and IAP SDKs with your own IDs.

## Credits

- **Original game:** [Hyper Dive](https://github.com/Prajwalvv/Hyper-Dive) by Prajwalvv: gameplay, levels, obstacles, shop and UI.
- **Ramayana edition:** [girishmohan](https://github.com/girishmohan).
- **Hanuman, Rama & Sita, Menaka, rakshasas and menu art:** generated with Muse.
- **Font:** [Baloo 2](https://github.com/EkType/Baloo2) by Ek Type, SIL Open Font License 1.1 (`hyper dive/Assets/A-Z_Materials/fonts/Baloo2/OFL.txt`).
- **Music, rakshasas, scenery and UI text art:** original, generated by the scripts in `tools/`.

## License

Hanuman Dive is published with the permission of Prajwalvv, the author of the original Hyper Dive. The original Hyper Dive README states an MIT License, but its repository has no LICENSE file, so ask before reusing this code in another project.
