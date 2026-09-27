# Functional Data Flow

## Overview

The gameplay pipeline follows the assignment's required flow:

```text
User Gesture
    ↓
Input Controller
    ↓
Grid Matrix Update
    ↓
UI Rendering Framework
```

The implementation expands this flow with game-state validation, history tracking, UI updates, audio events, and gameplay feedback.

## Main Gameplay Flow

```mermaid
flowchart LR
    User["User Gesture"]
    Input["SwipeInputController"]
    Direction["GridDirection Event"]
    Player["PlayerMovement"]
    Board["Board"]
    Grid["Grid"]
    History["MoveHistory"]
    Session["GameSession"]
    Presenter["BoardPresenter"]
    UI["Gameplay UI"]
    Audio["GameAudioController"]

    User --> Input
    Input --> Direction
    Direction --> Player

    Player --> Board
    Board --> Grid

    Board --> Player
    Player --> History
    Player --> Session

    Board --> Presenter
    Presenter --> UI

    Player --> Audio
```

## Player Movement

```mermaid
sequenceDiagram
    participant User
    participant Input as SwipeInputController
    participant Player as PlayerMovement
    participant Flow as GameFlow
    participant Board
    participant Session as GameSession
    participant History as MoveHistory
    participant Presenter as BoardPresenter

    User->>Input: Swipe direction
    Input->>Player: GridDirection

    Player->>Flow: Check current state

    alt State is Playing
        Player->>Board: TryMovePlayer(direction)

        alt Valid movement
            Board-->>Player: Move result
            Player->>Session: Consume move
            Player->>History: Add Move
            Player->>Presenter: Update presentation
        else Invalid movement
            Board-->>Player: Movement rejected
        end
    else State is not Playing
        Player->>Player: Ignore input
    end
```

## Invalid Movement

Invalid movement does not create a history entry and does not consume a move.

```text
Swipe
  ↓
PlayerMovement
  ↓
Board.TryMovePlayer()
  ↓
Invalid
  ↓
No board transition
  ↓
No MoveHistory entry
  ↓
No move consumed
```

## Undo Flow

```mermaid
flowchart LR
    User["Undo Button / Undo Input"]
    Event["UndoRequestedEvent"]
    Player["PlayerMovement"]
    History["MoveHistory"]
    Board["Board"]
    Session["GameSession"]
    Presenter["BoardPresenter"]
    UI["GameplayHud"]

    User --> Event
    Event --> Player
    Player --> History
    History --> Player
    Player --> Board
    Board --> Presenter
    Player --> Session
    Session --> UI
    History --> UI
```

Undo restores the previous normal player movement.

Power-up actions are not added to `MoveHistory`, so using Undo does not restore power-up charges or reverse a previously used power-up.

## Moving Obstacle Flow

```mermaid
flowchart LR
    Moving["MovingPieceController"]
    Board["Board"]
    Player["Player Piece"]
    Death["PlayerDeathEvent"]
    Flow["GameFlow"]
    Presenter["BoardPresenter"]

    Moving --> Board
    Board --> Player

    Player --> Death
    Death --> Flow
    Flow --> Moving

    Moving --> Presenter
```

Moving obstacles stop when gameplay is no longer active.

## Power-Up Flow

```mermaid
flowchart TD
    Button["Power-Up Button"]
    Controller["PowerUpController"]
    Mode["Power-Up Targeting Mode"]
    Input["SwipeInputController"]
    Target["BoardTargetSelectedEvent"]
    Board["Board"]
    Presenter["BoardPresenter"]
    Charge["Power-Up Charge"]
    Moving["MovingPieceController"]

    Button --> Controller
    Controller --> Mode

    Mode --> Input
    Input --> Target
    Target --> Controller

    Controller --> Board
    Board --> Presenter

    Controller --> Charge
    Controller --> Moving

    Mode -->|Cancel / Invalid Target| Controller
    Controller -->|End Mode| Mode
```

### Hammer

```text
Activate Hammer
    ↓
Enter targeting mode
    ↓
Select obstacle
    ↓
Validate target
    ↓
Remove obstacle from logical board
    ↓
Hide existing obstacle visual
    ↓
Consume Hammer charge
    ↓
End targeting mode
```

### Rocket

```text
Activate Rocket
    ↓
Enter targeting mode
    ↓
Select Stone
    ↓
Validate target
    ↓
Change Stone cell to Normal
    ↓
Update presentation
    ↓
Consume Rocket charge
    ↓
End targeting mode
```

Power-up actions are not normal movement actions and are not recorded in `MoveHistory`.

## Win / Loss Flow

```mermaid
flowchart TD
    Gameplay["Playing"]
    Goal["Goal Reached"]
    Death["Player Death"]
    Won["Won"]
    Lost["Lost"]
    Board["BoardInitializer"]
    Presenter["BoardPresenter"]
    Result["Result UI"]

    Gameplay --> Goal
    Gameplay --> Death

    Goal --> Won
    Death --> Lost

    Won --> Board
    Lost --> Board

    Board --> Presenter
    Presenter -->|Clear| Result
```

When `GameFlow` leaves `Playing`, the rendered board is cleared.

The logical level lifecycle is then controlled by `LevelController`, which can start the next level, retry the current level, or return to the Main Menu.

## Level Selection Flow

```mermaid
flowchart TD
    MainMenu["Main Menu"]
    Select["Select Level"]
    LevelController["LevelController"]
    Data["BoardData"]
    Factory["BoardFactory"]
    Initializer["BoardInitializer"]
    Presenter["BoardPresenter"]
    Session["GameSession"]
    Gameplay["Playing"]

    MainMenu --> Select
    Select --> LevelController
    LevelController --> Data
    LevelController --> Initializer
    Initializer --> Factory
    Factory --> Data
    Factory --> Initializer
    Initializer --> Presenter
    LevelController --> Session
    LevelController --> Gameplay
```

Each level provides its own board configuration and move limit through `BoardData`.

## Overall Runtime Flow

```text
Main Menu
    ↓
Level Selection
    ↓
LevelController
    ↓
BoardData
    ↓
BoardFactory
    ↓
Logical Board
    ↓
BoardPresenter
    ↓
Gameplay

Gameplay
    ↓
User Input
    ↓
Input Controller
    ↓
Gameplay System
    ↓
Board / Grid
    ↓
Game State
    ├── Move History
    ├── Remaining Moves
    ├── UI
    └── Audio

Gameplay
    ├── Goal → Won
    └── Death / Move Limit → Lost

Won / Lost / Main Menu
    ↓
BoardPresenter.Clear()
    ↓
Rendered Board Removed
```
