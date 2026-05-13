## Shared Enums
**Location:** `Assets/_Project/Core/Foundation/Enums/`

### 1. CurrencyType
**File:** `CurrencyType.cs`
**Usage:** Defines resources for Wallet and Rewards.

| Value | Description                                             |
| :--- |:--------------------------------------------------------|
| **`Energy`** | Stamina resource required to perform a **Gacha Pull**. Regenerates over time, capped by Soft/Hard cap |
| **`Gold`** | Soft currency earned from **Wave Rewards** and drops.   |
| **`Gem`** | Hard currency (Premium). Used to buy Energy.            |

Note: `None` is used for uninitialized or editor-only entities. Must not be targetable.

### 2. Faction
**File:** `Faction.cs`
**Usage:** Used by **Combat TargetingSystem** to identify targets.

| Value | Description |
| :--- | :--- |
| **`Player`** | Allies: **The Wall** and **Fruit Heroes**. |
| **`Enemy`** | Hostiles: **Monsters**. |
| **`Neutral`** | Environmental objects or unassigned Projectiles. |

Note: `None` is used for uninitialized or editor-only entities. Must not be targetable.

### 3. MonsterType
**File:** `MonsterType.cs`
**Usage:** Contract between **WaveSystem** (Sender) and **Combat** (Receiver).
**Structure:** Uses specific integer ranges to categorize behavior logic (Standard vs Boss).

#### Standard Enemies (Range: 0 - 99)
These units are spawned regularly in waves.

| Value | ID | Description & Behavior                                  |
| :--- | :--- |:--------------------------------------------------------|
| **`BasicMelee`** | `0` | Standard melee unit. Balanced stats.                    |
| **`FastRunner`** | `1` | High MoveSpeed, Low HP. Rushes the Wall.                |
| **`Tank`** | `2` | High HP/Armor, Slow Speed. Soaks damage.                |
| **`Ranged`** | `3` | Attacks from distance. Logic uses **ProjectileSystem**. |
| **`Healer`** | `4` | Support unit. Priority target for Heroes.               |
| **`Bomber`** | `5` | Explodes on impact/death (AoE Damage).                  |

#### Boss Enemies (Range: 100+)
These units trigger special logic in WaveSystem (e.g., `WaitForGroupClear`).

| Value | ID | Description & Behavior |
| :--- | :--- | :--- |
| **`Boss`** | `100` | Major boss. Pauses wave timer until defeated. |
| **`MiniBoss`** | `101` | Elite unit. Stronger stats, acts as squad leader. |

Integration Rules

1.  **Zero Logic:** These files must remain pure data definitions.
2.  **No Dependencies:** Do not reference `Game.Features.*` namespaces.