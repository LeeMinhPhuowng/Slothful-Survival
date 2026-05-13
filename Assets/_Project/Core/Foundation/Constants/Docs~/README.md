# Global Game Constants
**File:** `GameConstants.cs`
**Usage:** Static class containing canonical values used for system validation and logic.

### 1. Lane & Grid System
Used by **WaveSystem** (to assign lanes) and **Combat** (to calculate world position).

| Constant Name    | Value | Description & Source                                                                                     |
|:-----------------| :--- |:---------------------------------------------------------------------------------------------------------|
| **`LaneCount`**  | `5` | Total number of horizontal lanes (Indices 0-4). Defined in Wave System Design.                           |
| **`RandomLaneIndex`** | `-1` | Magic number used in `SpawnGroup` config to indicate "Pick a random valid lane". Defined in Test SG-005. |

### 2. Economy Defaults
Used by **Economy** (regeneration logic) and **UI** (display timers).

| Constant Name | Value | Description & Source                                                                                                          |
| :--- | :--- |:------------------------------------------------------------------------------------------------------------------------------|
| **`DefaultEnergyRegenSeconds`** | `60f` | **Canonical Value:** Energy regenerates 1 point every 60 seconds. All systems must reference this. Defined in Economy Design. |
| **`DefaultEnergySoftCap`** | `20` | Natural regeneration stops at this limit. Defined in Economy Design.                                                          |
| **`DefaultEnergyHardCap`** | `999` | Absolute maximum limit for Energy only.                                                        |

### 3. Spawn Limits & Performance
Used by **WaveSystem** (validation) and **Combat** (pooling) to ensure stability.

| Constant Name | Value | Description & Source                                                                       |
| :--- | :--- |:-------------------------------------------------------------------------------------------|
| **`MaxSpawnCountPerGroup`** | `50` | Maximum monsters in a single spawn group to prevent lag. Validation rule from Test SG-004. |
| **`MaxGroupsPerWave`** | `20` | Safety limit for wave configuration sizing.                                                |
| **`MaxWavesPerLevel`** | `100` | Safety limit for level configuration sizing.                                               |
| **`MaxEntitiesPerScene`** | `200` | Performance cap to maintain 60FPS on mobile devices.                                       |

### Integration Rules

1.  **Zero Logic:** These files must remain pure data definitions.
2.  **No Dependencies:** Do not reference `Game.Features.*` namespaces.
3.  **PascalCase:** All constants must follow PascalCase (e.g., `LaneCount`, not `LANE_COUNT`) per `CONVENTION_RULES.md`
