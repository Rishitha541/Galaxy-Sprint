# Unity 3D Endless Runner Game

A beginner-to-intermediate Unity portfolio project built with clean C# scripts, third-person endless runner movement, procedural spawning, object pooling, scoring, UI menus, audio hooks, animation triggers, and a stylized low-poly environment workflow.

This project is designed for a Game Developer internship portfolio and resume showcase.

## Features

- Third-person endless runner gameplay
- Automatic forward running
- 3-lane movement using `A/D` or arrow keys
- Jump using `Spacebar`
- Slide using `Left Ctrl` or `Right Ctrl`
- Gradually increasing game speed
- Procedural road generation
- Random obstacle spawning
- Multiple obstacle types: barrier, box, wall
- Random coin spawning and coin counter
- Distance score and coin score bonus
- High score saving with `PlayerPrefs`
- Main menu, pause menu, game over screen, restart, and quit buttons
- Object pooling for roads, coins, obstacles, trees, and rocks
- Audio hooks for background music, coin, jump, and game over sounds
- Animator trigger support for run, jump, slide, and death animations
- Unity Editor scene builder for fast setup

## Quick Start

1. Open this folder in Unity Hub as a Unity project.
2. Use Unity 2022 LTS or newer.
3. Let Unity import packages and compile scripts.
4. If Unity asks for TextMeshPro essentials, import them.
5. In the Unity menu, click `Tools > Endless Runner > Build Complete Demo Scene`.
6. Open `Assets/Scenes/MainScene.unity`.
7. Press Play.

The builder creates the playable demo scene, materials, low-poly prefabs, UI panels, object pool entries, tags, player, camera, lighting, and manager objects.

## Controls

| Action | Key |
| --- | --- |
| Move Left | `A` or `Left Arrow` |
| Move Right | `D` or `Right Arrow` |
| Jump | `Spacebar` |
| Slide | `Left Ctrl` or `Right Ctrl` |
| Pause / Resume | `Escape` |

## Folder Structure

```text
Unity-3D-Endless-Runner-Game/
|-- Assets/
|   |-- Art/
|   |   |-- Marketing/
|   |   `-- Materials/
|   |-- Audio/
|   |-- Editor/
|   |   `-- EndlessRunnerSceneBuilder.cs
|   |-- Prefabs/
|   |   `-- PREFAB_SETUP.md
|   |-- Scenes/
|   |   `-- SCENE_HIERARCHY.md
|   `-- Scripts/
|       |-- Camera/
|       |   `-- FollowCamera.cs
|       |-- Core/
|       |   `-- GameManager.cs
|       |-- Player/
|       |   `-- PlayerController.cs
|       |-- Systems/
|       |   |-- AudioManager.cs
|       |   |-- CoinSystem.cs
|       |   |-- ObjectPool.cs
|       |   |-- Obstacle.cs
|       |   |-- PooledObject.cs
|       |   `-- SpawnManager.cs
|       `-- UI/
|           `-- UIManager.cs
|-- Packages/
|   `-- manifest.json
|-- ProjectSettings/
|   `-- ProjectVersion.txt
|-- .gitignore
`-- README.md
```

## Core Scripts

`PlayerController.cs` handles lane switching, auto-running, jumping, sliding, and obstacle collision.

`GameManager.cs` controls game state, score, speed progression, high score saving, pause, restart, quit, and game over.

`UIManager.cs` manages the main menu, HUD, pause menu, game over screen, score, high score, and coin display.

`SpawnManager.cs` procedurally spawns road tiles, obstacles, coins, trees, and rocks ahead of the player.

`ObjectPool.cs` reuses objects instead of repeatedly instantiating and destroying them during gameplay.

`CoinSystem.cs` handles coin rotation, collection, scoring, sound, and returning coins to the pool.

`AudioManager.cs` plays background music and game sound effects when clips are assigned.

`FollowCamera.cs` provides a smooth third-person chase camera.

`EndlessRunnerSceneBuilder.cs` builds a complete playable demo scene from the Unity editor menu.

## Audio Setup

The game runs without audio clips, but the audio hooks are ready.

To add sound:

1. Place clips in `Assets/Audio`.
2. Open `MainScene`.
3. Select `Managers/AudioManager`.
4. Assign background music, coin, jump, and game over clips.

## Animation Setup

The game works without a character animator, but it supports these animator parameters:

| Parameter | Type | Used For |
| --- | --- | --- |
| `IsRunning` | Bool | Running animation |
| `Jump` | Trigger | Jump animation |
| `Slide` | Trigger | Slide animation |
| `Death` | Trigger | Death animation |

Assign an Animator to the player when you add a custom character model.

## GitHub Upload

Upload the full `Unity-3D-Endless-Runner-Game` folder to a GitHub repository.

Suggested repository name:

```text
Unity-3D-Endless-Runner-Game
```

Suggested portfolio link:

```text
https://github.com/shivamkr67/Unity-3D-Endless-Runner-Game
```

## Resume Line

**3D Endless Runner Game | Unity 3D, C#, Blender**  
Built a third-person endless runner with 3-lane movement, jumping, sliding, procedural obstacle and coin spawning, object pooling, UI menus, audio events, high score saving, and stylized low-poly environment assets.
