# Card-Match Prototype (Unity 2021 LTS)

Functional prototype of a card-match game with continuous input, scalable layouts, save/load, scoring, audio SFX, and win flow.

## Features Implemented
- Continuous card flipping with flip animation and match/mismatch resolution.
- Dynamic board generation for multiple grid sizes with auto-fit scaling.
- Level selection (Easy/Medium/Hard) with presets.
- HUD counters for Matches and Turns.
- Save/Load of progress (layout, card states, difficulty, matches/turns).
- Game completed detection with win popup.
- Audio SFX for flip, match, mismatch, and game completed + background music.

## Requirements
- Unity 2021 LTS

## How To Run
1) Open the project in Unity 2021 LTS.
2) Open `Assets/Scenes/Main.unity`.
3) Press Play.

## Setup Checklist (Editor)
1) `Assets/ScriptableObjects/Config/DefaultAppInstaller.asset`
   - Assign `UI Root Prefab`, `Board Root Prefab`, `Board Settings`, `Card Prefab`.
   - Assign `Level Select Prefab`, `HUD Prefab`, `Home Button Prefab`, `Next Button Prefab`, `Win Popup Prefab`.
   - Assign `Level Config`.
   - Assign `Audio Config`.
2) `Assets/ScriptableObjects/Config/Audio/DefaultAudioConfig.asset`
   - Assign `flip`, `match`, `mismatch`, `gameCompleted`, and `music` clips.

## Controls
- Click/tap cards to flip.
- Use Easy/Medium/Hard toggles, then Play.
- Home returns to level select.
- Next advances to the next difficulty (Easy → Medium → Hard).

## Project Structure
- `Assets/Scripts/Board`: board generation, layout, and grid scaling.
- `Assets/Scripts/Cards`: card view, input, matching logic.
- `Assets/Scripts/Core`: bootstrap, event bus, game phase/completion.
- `Assets/Scripts/UI`: UI panels, HUD, win popup, session control.
- `Assets/Scripts/Save`: save data + persistence.
- `Assets/Scripts/Audio`: audio config + controller.
