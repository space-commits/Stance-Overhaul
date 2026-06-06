# Stance-Overhaul Project Instructions

## Purpose
Implements procedural stance system for SPT - adds animated offsets to player weapon positioning and aiming. Stances enable tactical variety (high/low ready, active aim/canted) with gameplay benefits to enhance depth. Procedurally animates offsets via the game's existing ProceduralWeaponAnimations and Player.FirearmController systems.

## Key Features
- **Multiple stance types**: High ready, low ready, active aim (canting), bracing, etc.
- **Procedural animation**: Smooth animated transitions between stances using spring animators
- **Gameplay benefits**: Stances confer benefits (faster ADS, stability, ergonomics) to incentivize usage
- **Bracing system**: Different bracing directions with collision detection
- **Health/stamina effects**: Stances interact with player health and stamina states
- **Integration with common library**: Uses RealismCommonLib events, state, patches, and modifiers

## Project Structure
```
Stance-Overhaul/
├── Plugin.cs                    # BepInEx entry point
├── PluginConfig.cs              # Stance system configuration
├── Controllers/
│   └── StanceController.cs      # Main stance system controller, state machine
├── Enums/
│   ├── EStanceType.cs           # Stance types: HighReady, LowReady, ActiveAim, etc.
│   ├── EStanceState.cs          # State machine: Idle, Transitioning, Active
│   ├── EBracingDirection.cs     # Bracing directions: Left, Right, Front, etc.
│   └── ECurveType.cs            # Animation curve types
├── Handlers/
│   ├── StanceMovementHandler.cs # Movement speed and handling per stance
│   ├── StanceHealthHandler.cs   # Health/damage interaction with stances
│   ├── StanceStaminaHandler.cs  # Stamina cost per stance
│   ├── CollisionHandler.cs      # Collision detection for bracing
│   ├── MountingHandler.cs       # Mounting surfaces (bracing surfaces)
│   ├── PositionOffsetHandler.cs # Position offset calculations
│   └── IControllerHelper.cs     # Interface for handler lifecycle
├── Patches/
│   └── *Patch.cs                # Stance-related Harmony patches
├── PatchHooks/
│   └── *.cs                     # Patch hook implementations
├── SpringAnimators/
│   ├── SpringAnimator.cs        # Base spring physics animator
│   └── *SpringAnimator.cs       # Specific spring animator implementations
├── Stances/
│   ├── StanceBase.cs            # Base stance class
│   ├── HighReadyStance.cs       # High ready stance implementation
│   ├── LowReadyStance.cs        # Low ready stance implementation
│   ├── ActiveAimStance.cs       # Active aim (canting) stance implementation
│   └── ...                      # Other stance implementations
└── State/
    ├── StanceState.cs           # Per-player stance state
    └── ...                      # State management
```

## Core Systems

### Stance Controller (Main State Machine)
- **StanceController.cs**: Attached to player.gameObject, manages stance transitions and active stance
- Implements IControllerHelper for lifecycle (RunOnAwake, RunOnDestroy, RunOnUpdate)
- Manages state transitions (Idle → Transitioning → Active)
- Applies stance benefits (movement speed, ADS speed, stability)
- Integrates with RealismCommonLib events and state via event subscriptions
- Updates spring animators each frame to animate stance transitions

### Stance Types (Enumerations)
- **EStanceType**: High ready, low ready, active aim, braced, etc. - defines available stances
- **EStanceState**: Idle (no stance), Transitioning (animating between stances), Active (stance engaged)
- **EBracingDirection**: Left, right, front, rear for bracing direction when applicable
- **ECurveType**: Animation easing curves for smooth transitions

### Stance Implementations
Each stance inherits from `StanceBase` and defines:
- Position offset (relative to weapon/body) - X/Y/Z offset vector
- Rotation offset for weapon canting - Euler angles
- Animation curves and timing - smoothing curves for transitions
- Gameplay benefits - movement multiplier, ADS speed modifier, stability bonus, etc.
- Stance-specific logic - e.g., bracing checks for braced stance
- Transition in/out behavior - entry/exit animations

### Animation System (Spring Animators)
**SpringAnimators**: Physics-based smooth animation for stance transitions
- Base class `SpringAnimator` implements spring physics (Hooke's law differential equation)
- Separate animators for: position X/Y/Z, rotation, aim offset, etc.
- Each animator smoothly interpolates current value toward target value
- Used to animate weapon offset during stance transitions
- Configurable spring stiffness and damping for feel
- Applied via ProceduralWeaponAnimations patching

### Handler System (Gameplay Integration)
Handlers apply stance effects and check conditions - inherit from IControllerHelper:
- **StanceMovementHandler**: Applies movement speed multiplier based on active stance
- **StanceHealthHandler**: Prevents stance changes if player is injured, adjusts aiming stability
- **StanceStaminaHandler**: Applies stamina cost or recovery bonus per stance
- **CollisionHandler**: Detects terrain/obstacles for bracing validation
- **MountingHandler**: Detects valid bracing surfaces (sandbags, windows, etc.)
- **PositionOffsetHandler**: Calculates final weapon offset from stance + other factors
- Each handler implements RunOnAwake, RunOnDestroy, RunOnUpdate(deltaTime) interface

## Integration with RealismCommonLib

### Events Used
- **PlayerEvents.OnPlayerInitArgs**: Create StanceController on player spawn
- **InputEvents.ChangeStanceInput**: Change stance on player input (keybind)
- **PlayerEvents.AimStateChanged**: Adjust stances based on ADS state
- **ReloadEvents**: May restrict stances during reloads

### State Accessed
```csharp
var player = Plugin.PlayerStateInstance.Player;
var weapon = Plugin.WeaponStateInstance;  // IsPistol, IsShotgun, IsManuallyOperated
var health = Plugin.HealthStateInstance;  // health, injuries, etc.
```

### Patches Created
- **Input patches**: Detect stance change input (keybind) → raise InputEvents.ChangeStanceInput
- **Animation patches**: Intercept ProceduralWeaponAnimations for offset injection
- **FirearmController patches**: Apply stance benefits to aiming speed and movement

### Configuration Integration
- Uses PluginConfig.cs for all stance parameters
- Configurable per-stance: speed multipliers, ADS times, stamina costs, gameplay benefits
- Configurable global: stance smoothing, transition timing, input delay, etc.
- Access pattern: `PluginConfig.ConfigName.Value`

## Key Integration Points with Game

### ProceduralWeaponAnimations Integration
- **Target class**: Player.FirearmController.ProceduralWeaponAnimations
- **Target properties**: Position offset, rotation offset, aim offset
- **Method**: Patches intercept animation calculation methods
- **Strategy**: Inject stance-based position/rotation offsets into existing animation system
- **Result**: Weapon appears in different positions while maintaining game's procedural animations

### Player.FirearmController Integration
- **Target class**: Player.FirearmController (weapon aiming and firing controller)
- **Patched methods**: ADS speed calculations, weapon offset, firing checks
- **Strategy**: Modify animation speed and offset values based on active stance
- **Result**: Different stances feel distinct with different aiming/firing speeds and weapon positions

### Input System Integration
- Detect stance change keybind input (patches on input detection)
- Raise InputEvents.ChangeStanceInput event
- StanceController listens to event and triggers state transition

## Common Development Tasks
1. **Add new stance**: Create class inheriting StanceBase in Stances/, define offset/curves/benefits, register in StanceController
2. **Add stance benefit**: Create handler inheriting IControllerHelper in Handlers/, implement RunOnUpdate to apply effect
3. **Add animation**: Add SpringAnimator in SpringAnimators/, update StanceController update loop to animate it
4. **Add config option**: Add ConfigEntry in PluginConfig.cs, reference in stance/handler logic
5. **Debug stance transitions**: Check EStanceState enum and StanceController transition state machine
6. **Tweak animations**: Modify spring stiffness, damping, target offsets in SpringAnimator or stance definitions

## State Machine
StanceController maintains state transitions:
- **Idle**: No stance active
- **Transitioning**: Currently animating from one stance to another
- **Active**: Stance is fully engaged and active

Transitions triggered by:
- Player input (InputEvents.ChangeStanceInput)
- Game state changes (health, aiming state, reload)
- Animation completion

## Known Limitations & TODOs
- Still work in progress
- Placeholder implementations for some stances
- Bracing detection may need refinement for complex terrain
- Stance benefits not fully balanced between stance types
- Animation curves may need tweaking for feel
- Performance impact during stance transitions not yet optimized
- Some handlers have TODO comments for future features
