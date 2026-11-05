# Audio Setup

Add these audio clips to this folder and assign them in `AudioManager`:

- `background_music`
- `coin_collect`
- `jump`
- `game_over`

Recommended settings:

- Background music: loop enabled through `AudioManager`
- SFX clips: short WAV/MP3 files
- Keep volume balanced:
  - Music: `0.45`
  - SFX: `0.85`

The game works even if clips are not assigned. Audio calls are null-safe.

