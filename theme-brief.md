# Theme Music Brief — Ramayan iOS Game (Hanuman's Flight)

## Purpose
A soothing background theme that plays on loop during gameplay. It should feel
devotional and serene — the calm of Hanuman's devotion carrying the player
through the journey to save Sita — never distracting from play.

## Mood
Serene, devotional, weightless. Dawn over the ocean Hanuman is about to cross.
Hopeful, not melancholic; reverent, not intense.

## Instrumentation
- **Bansuri (bamboo flute)** — the lead voice. Soft, breathy, unhurried phrases.
- **Tanpura drone** (sa–pa) — continuous underneath; this is what makes the loop
  feel seamless.
- Optional, very sparing: soft **tabla or manjira** at low volume, or no
  percussion at all. No vocals — vocals fight gameplay focus.

## Melodic guidance
- Base the melody on **Raga Yaman** (evening raga, serene and devotional) or
  **Hamsadhwani** (auspicious, gently uplifting). Keep phrases simple and
  repetitive, like a lullaby or aarti.
- **Tempo:** ~60–72 BPM, free-flowing (rubato) is fine for the flute as long as
  the drone holds steady time.

## Loop construction (most important part)
- Length: **60–90 seconds**, resolving fully back to the tonic (sa) at the end.
- The tanpura drone must be **identical at the loop point** — no fade-out/fade-in;
  cut on a drone cycle boundary so the join is inaudible.
- Avoid a strong final cadence or a distinctive ending phrase; the piece should
  feel like it could continue forever.
- Keep the loudest moment in the first half; end softer so the restart never
  startles.

## Technical specs for iOS
- Format: **AAC (.m4a) 256 kbps** or MP3 192+ kbps, 44.1 kHz, stereo.
- Loudness: modest, around **−16 LUFS**, so it sits under sound effects.
- Seamless-loop edit verified by listening to the join at least 3 times.
- Playback: `AVAudioPlayer` with `numberOfLoops = -1`.

## Sourcing options
1. **Royalty-free libraries** — search "Indian flute meditation ambient" on
   Pixabay Music (free), MelodyLoops, or Uppbeat. Always confirm the license
   covers commercial mobile games before shipping.
2. **AI generation** — paste the Mood/Instrumentation/Loop sections above into a
   music generator as the prompt.
3. **Commission** — hand this brief to a bansuri player; a 90-second original
   recording gives the game a signature no library track can.

## Licensing rule
Whatever the source, the track must be cleared for **commercial use in a mobile
game**, in writing, before release.
