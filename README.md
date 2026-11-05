# Galaxy Sprint

A 3D space-themed endless runner built in Unity 6. A runner sprints down a glowing neon track through deep space, dodging obstacles, sliding under laser gates and collecting energy crystals while the speed keeps rising.

Galaxy Sprint is my theme module of the capstone project **SPEEDVERSE** (B.Tech, School of Computer Science and Artificial Intelligence, SR University, 2025-2026).

## Gameplay

- The character runs forward automatically on a three-lane track.
- Switch lanes, jump over low barriers, and slide under laser gates.
- Collect cyan crystals to raise your score. Distance also adds to the score.
- Speed increases as the run continues. Touching any obstacle ends the run.
- Main menu, HUD, pause menu, game over screen and high score are included.

## Controls

| Action | Keys |
|---|---|
| Move left / right | `A` / `D` or Left / Right arrow |
| Jump | `Space` |
| Slide | `Left Ctrl`, `Right Ctrl`, `Down Arrow` or `S` |

## What I built on top of the base project

- **Space environment:** nebula skybox (SpaceSkies Free), dark space lighting and fog.
- **Neon track:** dark metal road with glowing cyan lane lines (emissive materials).
- **Space objects:** glowing crystal coins, violet space crates, red barriers, and floating planets that stay in the distance (spheres parented to the camera).
- **Laser gate:** a new slide-under obstacle (two posts and a glowing beam), added to the object pool and the spawner.
- **Character:** a textured humanoid from Mixamo with run and crouched-run (slide) animations, driven by Animator triggers.
- **UI:** "GALAXY SPRINT" title and a neon colour scheme.

## How it works

| Script | Role |
|---|---|
| `PlayerController` | Lane movement, jump, slide (the collider shrinks for 0.75 s) and animator control |
| `SpawnManager` | Generates road tiles, obstacles, coin lines and side decorations ahead of the player |
| `ObjectPool` | Reuses objects instead of creating and destroying them, so the game stays smooth |
| `Obstacle` | Ends the game when the player touches an object tagged `Obstacle` |
| `GameManager` | Game state, speed increase, score and game over |
| `UIManager` | Menus, HUD, pause and restart |
| `EndlessRunnerSceneBuilder` (Editor) | Builds the base scene from the menu `Tools > Endless Runner > Build Complete Demo Scene` |

## Run the project

1. Install **Unity 6** (developed with `6000.5.9f1`).
2. Clone or download this repository.
3. In Unity Hub choose **Add > Add project from disk** and select the project folder (the one containing `Assets`, `Packages` and `ProjectSettings`).
4. Open the project. The first launch takes a few minutes while Unity rebuilds the `Library` folder.
5. Open `Assets/Scenes/MainScene.unity` and press **Play**.

> Unity Hub may show a yellow warning that the Built-In Render Pipeline is deprecated. It is only a notice and does not affect the game.
>
> Do not run `Tools > Endless Runner > Build Complete Demo Scene` again after opening `MainScene`. It rebuilds the scene from scratch and removes the space theme.

## Tools used

Unity 6, C#, Visual Studio, Mixamo, Unity Asset Store, GitHub.

## Credits

- Gameplay base: open-source **Unity 3D Endless Runner Game** (lane movement, spawning, object pooling, scoring, UI).
- Skybox: **SpaceSkies Free** by Pulsar Bytes (Unity Asset Store).
- Character and animations: **Mixamo** (Adobe).
- Space theme, laser gate, planets, materials, character setup and animation hooks: **Rishitha Gangidi**.

## Author

Rishitha Gangidi (2303A52149)
Guide: Dr. Vijaya Chandra Jadala, Assistant Professor, School of CS&AI, SR University

## Future scope

Android build with touch controls, power-ups (shield, speed boost), bloom glow, jump animation, sound effects and an online leaderboard.
