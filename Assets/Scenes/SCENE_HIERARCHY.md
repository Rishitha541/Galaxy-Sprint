# Scene Hierarchy

The fastest setup is:

```text
Tools > Endless Runner > Build Complete Demo Scene
```

That menu command creates `Assets/Scenes/MainScene.unity` with the hierarchy below.

```text
MainScene
|-- Managers
|   |-- GameManager
|   |   `-- GameManager.cs
|   |-- UIManager
|   |   `-- UIManager.cs
|   |-- AudioManager
|   |   `-- AudioManager.cs
|   |-- ObjectPool
|   |   `-- ObjectPool.cs
|   `-- SpawnManager
|       `-- SpawnManager.cs
|-- Player
|   |-- CharacterController
|   |-- PlayerController.cs
|   `-- Player_Visual
|-- Main Camera
|   `-- FollowCamera.cs
|-- Directional Light
|-- Canvas
|   |-- MainMenuPanel
|   |   |-- TitleText
|   |   |-- StartButton
|   |   `-- QuitButton
|   |-- HUDPanel
|   |   |-- ScoreText
|   |   |-- HighScoreText
|   |   |-- CoinText
|   |   `-- PauseButton
|   |-- PausePanel
|   |   |-- ResumeButton
|   |   |-- RestartButton
|   |   `-- QuitButton
|   `-- GameOverPanel
|       |-- FinalScoreText
|       |-- FinalHighScoreText
|       |-- RestartButton
|       `-- QuitButton
`-- EventSystem
```

## Inspector Setup

The scene builder assigns the required references automatically. Use the sections below only if you are building the scene manually.

### GameManager

Recommended values:

- Starting Speed: `8`
- Max Speed: `24`
- Speed Increase Per Second: `0.18`
- Distance Score Multiplier: `1`
- Coin Score Value: `25`

### Player

Required:

- Tag: `Player`
- Add `CharacterController`
- Add `PlayerController.cs`
- Assign Animator if available

Recommended `CharacterController` values:

- Height: `2`
- Radius: `0.35`
- Center Y: `1`

### Camera

Attach `FollowCamera.cs` to Main Camera.

Recommended offset:

```text
X: 0
Y: 5.5
Z: -8
```

### ObjectPool

Add pool configs:

| Pool ID | Prefab | Initial Size |
| --- | --- | --- |
| RoadTile | Road tile prefab | 8 |
| Coin | Coin prefab | 30 |
| Barrier | Barrier obstacle prefab | 10 |
| Box | Box obstacle prefab | 10 |
| Wall | Wall obstacle prefab | 8 |
| Tree | Tree decoration prefab | 12 |
| Rock | Rock decoration prefab | 12 |

### SpawnManager

Assign:

- Player transform
- ObjectPool reference

Default pool IDs already match the table above.

### UIManager

Assign all panel GameObjects, TMP text fields, and buttons in the Inspector.
