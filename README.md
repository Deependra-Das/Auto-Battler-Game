# Warband Tactics


## Overview

Warband Tactics is an Auto Battler game where players recruit units from a randomized shop using Battle Coins, build powerful team compositions for buffs, and strategically deploy them to defeat enemies across multi-round stages. Combine Crusader (Fire), Spartan (Nature), and Viking (Thunder) units to activate faction and type (Attacker, Tank, Ranged, Support) synergies, exploit elemental counters, level up by buying XP to expand deployment capacity, and carefully manage your economy and clear rounds without losing lives.

The project implements drag-and-drop feature for cards & units, JSON save/load for stage/round progression management & tracking, Shader Graph & VFX Graph combat effects, and a scalable architecture built with design patterns like Object Pooling, Observer pattern with Event Bus, Service Locator, and Singleton managers.

---

## Features

- Auto Battler combat that requires strategic unit placement & economy management.
- Unit Factions : Command three unique factions: Crusader (Fire), Spartan (Nature), and Viking (Thunder).
- Unit Types : Build teams using Melee Attackers, Melee Tanks, Ranged Attackers, and Support/Healers.
- Elemental Combat : Exploit elemental counters to deal bonus damage against enemy teams.
- Player Level System : Purchase XP with Battle Coins to increase your maximum on-field unit deployment capacity.
- Synergy Buff System : Activate team-wide bonuses by combining matching factions or unit classes.
- Shop : Buy & recruit units from a randomized shop using Battle Coins.
- Battle Economy : Start with certain amount of Battle coins at the start of each stage. Then earn Battle Coins from round wins, losses, and draws to manage your economy.
- Stage Selection : View stage previews, difficulty, enemy level, recommended elements, and round progress & visually track completed, failed, and current round to continue within each stage.
- Stage Progression Persistence : Save Completed Round data, Load & Resume unfinished stages with JSON-based save system that preserves stage progress between sessions.
- Inventory System : Organize, reorder, deploy, and manage recruited units cards.
- Drag & Drop System : Rearrange inventory cards, deploy units, reposition formations, and discard/sell inventory or deployed units to recover Battle Coins.
- Combat VFX — Elemental attacks, Buffs, Healing use visual effects implemented using Shader Graph and VFX Graph.
- Audio Settings : Adjust music, combat, and sound effect volumes through an Audio Mixer.
- Vide Tutorials : Learn gameplay mechanics with integrated video tutorials and beginner guides.
     
---

## Implementation Details

### Design Patterns

**Singleton Pattern**: Used for managing global systems like the GameManager, Gamemanager, EventBusManager, UIManager, AudioManager etc. This ensures there's only one instance handling these operations across the game.

**Observer Pattern**: Used for event-driven interactions, such as Unit deployment, XP/Level updates, Stage/Round state, Gameplay state. Components can "subscribe" to events (like the gameplay state changing) and automatically update themselves when these events occur.

**Object Pooling**: Optimized performance by reusing objects like shop & inventory cards, units, vfx animations etc. instead of frequently instantiating and destroying them. This reduces the performance hit from memory allocations and garbage collection, especially during intense gameplay moments.

**Service Locator Pattern**: Centralized management of game services like player level, units, stage, round, currency, buffs, vfx, audio, etc. The Service Locator helps decouple the game components, making it easier to manage services and dependencies across different systems.

### Scriptable Objects

- Unit Data: Stores unit stats, faction, type, element, cost, and combat attributes etc. This allows for easy customization and modification of unit properties directly from the Unity Editor without needing to modify code.
- 
- Unit Prefab System : Stores Unit Ids & Prefabs which is used to map unit data to their corresponding in-game prefabs for spawning and deployment. this helps with decoupling the prefabs from the data allowing more flexibity for object pooling.

- Player Level Configuration Data: Contains attributes for levels as indexes & the respective experience points required for next level. This is used by the player to level up & increase maximum on-field unit deployment capacity.
  
- Stage Configuration Data : Stores stage tilemaps, enemy data & their positions, round data, max lives for player, currency, recommended elements, etc. allowing for easy update or addition of stage/round configuration for gameplay though Unity Editor  without needing to modify code.

- Buff Data: Contains synergy buffs, stat modifiers, and gameplay effects applied during combat. Using Scriptable Objects for buffs makes it easier to balance and tweak buffs without touching the core logic.

- VFX Data: Centralizes visual effects references for combat, abilities, and elemental attacks allowing for easy addition of new VFX by simply creating new instances in the Unity editor.

- Audio Data: Contains attributes for different audio types & their respective sound clips.


---

## How to Play

- A playable build of the game is available as a ZIP file. Download the ZIP file & extract it. 
- Open the extracted folder content & launch the "Auto-Battler-Game" applicaton file.
- Once the game launches, click on How To Play button on top right corner of the main menu to learn how to play the game.
- Click on the main menu to go to the Stage Selection screen.
- Select a stage from the list of stages. 
- Once the round of the stage starts, use initial amount of Battle Coins to buy & recruit units from the shop.
- Build faction and type synergies to gain buffs to deal additional damage.
- Try to buy Units that counter the elements of the enemy units (Use Elemental Counter Chart).
- Place & arrange your units on the battlefield.
- Click on the Enter Combat button to witness the automated combat.
- Earn battler coins based on the outcome.
- Use battle coins to buy XP & level up to increase the maximum on-field unit deployment capacity between rounds.
- Clear every round before losing all your lives to complete that stage successfully.
  
---

## Architecture Block Diagram


---

## Playable build


---

## Gameplay Video



