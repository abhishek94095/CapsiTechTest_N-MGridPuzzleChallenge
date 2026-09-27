# Grid Puzzle Challenge

A grid-based mobile puzzle game built in Unity with a decoupled gameplay model, deterministic movement, undo history, moving obstacles, and gameplay power-ups.

## Build

The Android build is available below for evaluation.

### Android Build

- [Download Android Build](./AppBuild/AndroidBuild.apk)

## Technology & Packages

### Unity Input System

[Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest/)

Provides a unified input API for touch, mouse, and keyboard input, keeping input handling separate from gameplay logic.

### VContainer

[VContainer](https://github.com/hadashiA/VContainer)

A lightweight dependency injection framework for Unity that makes dependencies explicit and improves separation of responsibilities without relying on service locators or global gameplay singletons.

### UniTask

[UniTask](https://github.com/Cysharp/UniTask)

Provides allocation-efficient async/await support designed for Unity, making asynchronous workflows easier to compose while fitting naturally into Unity's player loop.

### MessagePipe

[MessagePipe](https://github.com/Cysharp/MessagePipe)

Provides strongly typed in-process messaging, allowing gameplay systems, UI, and audio to communicate without unnecessary direct dependencies.

### SRDebugger

[SRDebugger](https://github.com/StompyRobot/SRDebugger)

Provides an in-game debugging and inspection interface that helps speed up development and debugging without requiring custom debug tooling.

## Game Overview

The game uses a rectangular grid where the player moves one cell at a time using directional input.

The objective is to reach the Goal while navigating obstacles and managing a limited number of moves.

### Core Gameplay

- N×M logical grid.
- Discrete Up, Down, Left, and Right movement.
- Limited moves per level.
- Undo for normal player movement.
- Static obstacles.
- Moving obstacles with predefined paths.
- Player death when hit by a moving obstacle.
- Win state when the player reaches the Goal.
- Game-over state when the player dies or exhausts the available moves.
- Three playable levels with different board sizes and move limits.

### Power-Ups

The game includes two power-ups as an additional gameplay feature.

#### Hammer

- Starts with 3 charges.
- Enters target-selection mode.
- Targets an adjacent active obstacle.
- Removes the obstacle from the logical board.
- The existing obstacle GameObject is hidden rather than destroyed.
- Power-up actions are not recorded in movement history.

#### Rocket

- Starts with 3 charges.
- Enters target-selection mode.
- Targets a Stone cell.
- Changes the Stone cell to Normal.
- The action is not recorded in movement history.

Power-up charges are not restored by Undo.

Invalid targets and cancellation do not consume a charge.

## Levels

The game currently contains three playable levels.

| Level | Board Size | Move Limit |
|---|---:|---:|
| Level 1 | 3 × 3 | 6 |
| Level 2 | 5 × 5 | 12 |
| Level 3 | 6 × 7 | 18 |

Level configuration is data-driven through `BoardData` ScriptableObjects.

## Architecture

The project separates logical gameplay state from Unity presentation.

### Main Layers

```text
Application
    │
    ├── GameFlow
    ├── LevelController
    ├── GameSession
    ├── MoveHistory
    ├── PowerUpController
    └── GameAudioController
    │
    ▼
Gameplay
    │
    ▼
Core

Infrastructure
    │
    ▼
Core
```

The logical board does not depend on rendered GameObjects.

Unity-specific presentation is handled separately by `BoardPresenter`.

Dependency injection is provided through VContainer, with `ApplicationLifetimeScope` acting as the composition root.

See:

- [Architecture Documentation](ARCHITECTURE.md)
- [Functional Data Flow](DATA_FLOW.md)

## Gameplay Data Flow

The main gameplay pipeline is:

```text
User Gesture
    ↓
SwipeInputController
    ↓
Grid Direction Event
    ↓
PlayerMovement
    ↓
Board / Grid Update
    ↓
BoardPresenter
    ↓
Rendered Board / UI
```

Gameplay systems also publish events that allow independent systems such as UI and audio to react without directly depending on gameplay presentation.

## Game State

The game uses four application states:

```text
MainMenu
Playing
Won
Lost
```

When gameplay leaves the `Playing` state, the rendered board is cleared.

The `LevelController` manages:

- Starting a level.
- Retrying the current level.
- Starting the next level.
- Returning to the Main Menu.
- Resetting movement history.
- Resetting level-specific power-up state.
- Starting and stopping moving pieces.

## Undo

Normal player movement is recorded in `MoveHistory`.

An undo operation restores the previous player position and restores the consumed move.

If a player movement captured an obstacle, the captured obstacle is restored by Undo.

Power-up actions are intentionally excluded from movement history and cannot be undone.

## Input

The input system supports:

- Touch/swipe input.
- Mouse input.
- Keyboard directional input.
- Undo input.

During power-up targeting mode, normal movement input is suppressed and pointer selection is used for target selection.

## Audio

Gameplay audio is event-driven.

`GameAudioController` listens to gameplay events and maps them to configured audio entries.

Audio configuration is data-driven through `GameAudioConfig`.

Gameplay systems do not directly control `AudioSource` components.

## Project Structure

The Unity project follows the project's numbered folder convention:

```text
Assets/
├── 00-Scenes
├── 01-Scripts
├── 02-Prefabs
├── 03-SOData
├── 04-Art
├── 05-External
├── 06-Tools
└── 07-Audio
```

## Running the Project

1. Open the project in Unity 6.3 LTS.
2. Open `MainScene`.
3. Enter Play Mode.
4. Select a level from the Main Menu.
5. Use swipe, mouse, or keyboard input to move.
6. Reach the Goal before running out of moves.
7. Use Undo or the available power-ups when appropriate.

## Development Practices

The project follows these engineering principles:

- SOLID design.
- Composition over inheritance.
- Dependency injection through VContainer.
- Event-driven communication where appropriate.
- Logical gameplay state separated from presentation.
- No service locator.
- No unnecessary runtime hierarchy searches.
- Direct serialized references or dependency injection instead of component discovery where possible.
- Deterministic board transitions.
- Minimal allocations in gameplay hot paths.
- Data-driven level configuration.
- Atomic Git commits using semantic commit messages.

## Git History

Development was performed incrementally using semantic commit messages.

Examples:

```text
feat: establish board domain
feat: render board with prefabs
feat: implement core board gameplay
feat: add gameplay audio system
feat: add level selection and game flow
docs: add architecture and data flow documentation
```

The repository contains the incremental development history required for the assignment.

## Documentation

Additional technical documentation:

- [System Architecture](ARCHITECTURE.md)
- [Functional Data Flow](DATA_FLOW.md)
