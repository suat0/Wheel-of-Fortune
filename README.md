# Wheel of Fortune

A mobile-first spin-the-wheel game built with **Unity 2021.3 LTS**. Players spin a reward wheel across progressively harder zones, collecting loot while dodging bombs.

## Gameplay

- Spin the wheel to win rewards (gold, weapons, chests, etc.)
- Every **5th zone** is a **Safe Zone** — no bombs, guaranteed rewards
- Every **30th zone** is a **Super Zone** — high-value multiplied rewards
- Hit a bomb and lose everything — or collect your rewards at a safe zone before it's too late
- Reward values scale up as you progress through zones

## Architecture

```
Assets/_Project/
├── Scripts/
│   ├── Core/           GameManager, WheelController, ZoneManager, RewardManager
│   ├── Data/           ScriptableObject configs (GameConfig, WheelConfig, RewardData)
│   ├── UI/             UIManager, panel views, wheel slice rendering
│   ├── Editor/         Smoke tests & custom inspectors
│   └── Utility/        SafeAreaFitter
├── ScriptableObjects/  Game config, wheel configs, reward definitions
├── Prefabs/            UI prefabs
├── Sprites/            Wheel, rewards, UI, backgrounds, effects
└── Scenes/             Gameplay scene
```

**Key design decisions:**
- All game data is driven by **ScriptableObjects** — zone rules, wheel layouts, and rewards are fully configurable without code changes
- Event-driven architecture — `GameManager` exposes events (`ZoneChanged`, `SpinCompleted`, `BombHit`, etc.) that the UI layer subscribes to
- Wheel spin animations powered by **DOTween**
- Zone type logic is interval-based: safe every N zones, super every M zones (configurable)

## Dependencies

| Package | Purpose |
|---------|---------|
| [DOTween](http://dotween.demigiant.com/) | Spin & UI transition animations |
| TextMeshPro | Text rendering |
| Unity UI (uGUI) | Canvas-based UI |

## Requirements

- Unity **2021.3.45f2** (or any 2021.3 LTS patch)
- Android build support for APK export

## Getting Started

1. Clone the repo
   ```bash
   git clone https://github.com/suat0/Wheel-of-Fortune.git
   ```
2. Open the project in Unity 2021.3 LTS
3. Open `Assets/_Project/Scenes/` and load the gameplay scene
4. Press Play

### Running Tests

Editor smoke tests are available under **Vertigo Case > Tests** in the Unity menu bar. They cover zone rules, wheel result resolution, target angle calculation, and reward stacking.

## Download

Pre-built Android APK is available on the [Releases](https://github.com/suat0/Wheel-of-Fortune/releases) page.

## License

This project was created as a case study. All reward sprites and UI assets are included for demonstration purposes only.
