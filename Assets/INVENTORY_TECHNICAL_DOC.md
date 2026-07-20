# Inventory Feature – Technical Documentation

## 1. Architecture Overview

```
Scripts/
├── Data/                    ← ScriptableObject configs (readonly)
│   ├── ItemSO.cs            ← Item shape, rotation, prefab refs
│   └── InventoryConfigSO.cs ← Grid dimensions, item catalog
├── Model/                   ← Pure C# logic (NO Unity visuals)
│   ├── GridModel.cs         ← Generic 2D grid data structure
│   ├── GridCell.cs          ← Single cell: tracks Bag + Regular layers
│   ├── ItemModel.cs         ← Runtime item instance (origin, dir, SO ref)
│   └── InventoryModel.cs    ← Grid logic: place, remove, validate
├── Events/                  ← EventBus payloads (readonly structs)
│   ├── ItemPlacedEvent.cs
│   ├── ItemRemovedEvent.cs
│   ├── ItemDragStartEvent.cs
│   └── ItemDragEndEvent.cs
├── View/                    ← MonoBehaviour visuals
│   ├── InventoryView.cs     ← Grid background + ItemView lifecycle
│   ├── ItemView.cs          ← Single item visual wrapper
│   ├── ItemDragHandler.cs   ← Drag input → publishes events
│   └── DragDropSystem.cs    ← Orchestrator: drag events → model calls
└── Installers/
    └── InventoryInstaller.cs
```

**Pattern**: Data-Model-View + DI (Reflex) + EventBus

**Rule**: Model NEVER touches Unity visuals. View NEVER modifies grid state directly.

```
Model ──publish──→ EventBus ──notify──→ View
View  ──publish──→ EventBus ──notify──→ DragDropSystem ──call──→ Model
```

---

## 2. Data Layer

### 2.1 ItemSO (ScriptableObject)

Defines an item's **shape** and **rotation behavior**. Shared across all instances.

```csharp
public class ItemSO : ScriptableObject
{
    public ItemType itemType;   // Bag or Regular
    public string nameString;
    public Transform prefab;    // UI prefab for instantiation
    public Transform visual;    // Visual-only prefab (preview)
    public int width, height;   // Shape in grid cells
}
```

**ItemType enum**: Two layers exist in the grid:
- `Bag` — placed directly on empty grid. Acts as "unlocked area".
- `Regular` — can ONLY be placed on cells that already have a Bag.

#### Direction & Rotation System

4 directions: `Down(0°)`, `Left(90°)`, `Up(180°)`, `Right(270°)`.

**GetGridPositionList(offset, dir)** — returns all cells an item occupies:

```
Example: Item width=2, height=3, origin=(1,1)

Dir.Down / Dir.Up (normal orientation):
  iterate x=[0..width), y=[0..height)
  → (1,1)(1,2)(1,3)(2,1)(2,2)(2,3)

Dir.Left / Dir.Right (swapped W↔H):
  iterate x=[0..height), y=[0..width)
  → (1,1)(1,2)(2,1)(2,2)(3,1)(3,2)
```

**GetRotationOffset(dir)** — pivot compensation when rotating:

| Dir   | Offset          | Why                                      |
|-------|-----------------|------------------------------------------|
| Down  | `(0, 0)`        | No compensation needed                   |
| Left  | `(0, width)`    | Pivot shifts up by original width         |
| Up    | `(width,height)`| Pivot shifts to opposite corner           |
| Right | `(height, 0)`   | Pivot shifts right by original height     |

This offset is applied to the visual's `anchoredPosition` so the sprite rotates around the correct pivot.

### 2.2 InventoryConfigSO

```csharp
public class InventoryConfigSO : ScriptableObject
{
    int gridWidth = 10;      // Columns
    int gridHeight = 10;     // Rows
    float cellSize = 50f;    // Pixels per cell
    ItemSO[] allItems;       // Catalog for Save/Load lookup
    Transform gridVisualPrefab;
}
```

---

## 3. Model Layer (Pure C#)

### 3.1 GridModel\<T\>

Generic 2D array with coordinate conversion.

**Coordinate system**: Origin at bottom-left `(0,0)`. X = column (right), Y = row (up).

```
World Position = Vector3(x, y) * cellSize + originPos

Grid Position = Floor((worldPos - originPos) / cellSize)
```

Key methods:
| Method | Description |
|--------|-------------|
| `GetWorldPosition(x,y)` | Grid coords → world/anchored position |
| `GetXY(worldPos, out x, out y)` | World → grid coords (floor) |
| `IsValidGridPosition(Vector2Int)` | Bounds check: `0 ≤ x < width && 0 ≤ y < height` |
| `TriggerGridObjectChanged(x,y)` | Fires `OnGridObjectChanged` event |

### 3.2 GridCell

Each cell has **two layers**:

```csharp
public class GridCell
{
    private ItemModel _bagItem;      // Layer 0: Bag
    private ItemModel _regularItem;  // Layer 1: Regular item
}
```

**Rules**:
- `SetItem(item)`: routes to `_bagItem` or `_regularItem` based on `item.ItemSO.itemType`
- `ClearItem(type)`: clears the specific layer
- A cell can have BOTH a bag AND a regular item simultaneously

### 3.3 ItemModel

Pure data object representing a placed item instance:

```csharp
public class ItemModel
{
    public ItemSO ItemSO;       // Shape/type definition
    public Vector2Int Origin;   // Top-left grid cell of this item
    public ItemSO.Dir Dir;      // Current rotation
    
    public List<Vector2Int> GetGridPositionList()
        → delegates to ItemSO.GetGridPositionList(Origin, Dir)
}
```

### 3.4 InventoryModel

Core logic. Created by `InventoryView` (1:1 relationship).

#### TryPlaceItem Flow

```csharp
bool TryPlaceItem(ItemSO itemSO, Vector2Int origin, ItemSO.Dir dir)
```

```
Step 1: Generate cell list
   positions = itemSO.GetGridPositionList(origin, dir)

Step 2: Validate ALL cells
   foreach pos in positions:
     ├─ IsValidGridPosition(pos)?          → false = REJECT
     ├─ If Bag:   cell.HasBag()?           → true  = REJECT (no stacking bags)
     └─ If Regular:
           cell.HasBag()?                  → false = REJECT (must have bag underneath)
           cell.HasRegular()?              → true  = REJECT (no stacking items)

Step 3: Commit (all-or-nothing)
   item = new ItemModel(itemSO, origin, dir)
   foreach pos: cell.SetItem(item)
   _items.Add(item)

Step 4: Notify
   _eventBus.Publish(new ItemPlacedEvent(item, this))
   return true
```

#### RemoveItem Flow

```csharp
bool RemoveItem(ItemModel item)
```

```
Step 1: CanRemoveItem check
   ├─ item exists in _items?
   └─ If item is Bag:
        foreach cell it occupies:
          cell.HasRegular()? → true = REJECT (can't remove bag with items on it)

Step 2: Clear cells
   foreach pos in item.GetGridPositionList():
     cell.ClearItem(item.ItemSO.itemType)

Step 3: Notify
   _eventBus.Publish(new ItemRemovedEvent(item, this))
```

#### GetItemWorldPosition (for visual positioning)

```csharp
Vector3 GetItemWorldPosition(Vector2Int origin, ItemSO.Dir dir, ItemSO itemSO)
{
    Vector2Int rotOff = itemSO.GetRotationOffset(dir);
    return grid.GetWorldPosition(origin.x, origin.y)
         + Vector3(rotOff.x, rotOff.y) * cellSize;
}
```

This converts grid coordinates to pixel coordinates AND applies the rotation offset so the visual sprite aligns correctly with the grid cells it occupies.

---

## 4. Events Layer

All events are `readonly struct` (zero-allocation):

| Event | Published By | Consumed By | Data |
|-------|-------------|-------------|------|
| `ItemPlacedEvent` | `InventoryModel` | `InventoryView` | Item, Inventory |
| `ItemRemovedEvent` | `InventoryModel` | `InventoryView` | Item, Inventory |
| `ItemDragStartEvent` | `ItemDragHandler` | `DragDropSystem` | Item, SourceInv, offsets, Dir |
| `ItemDragEndEvent` | `ItemDragHandler` | `DragDropSystem` | Item, SourceInv, ScreenPos |

**Filtering**: `InventoryView` checks `evt.Inventory != _inventory` to ignore events from other inventories sharing the same EventBus.

---

## 5. View Layer

### 5.1 InventoryView — Grid Background + ItemView Manager

**Lifecycle**:
```
Awake  → Reflex calls [Inject] Construct(IEventBus) — stores ref only
Start  → Creates InventoryModel
       → Subscribes to ItemPlacedEvent / ItemRemovedEvent
       → Calls BuildBackground()
```

#### BuildBackground — How the Grid Visual is Created

```
1. _backgroundTemplate.SetActive(false)     ← hide the template cell
2. For each (x,y) in grid:
     Clone template → parent to _backgroundContainer → SetActive(true)
3. Set GridLayoutGroup.cellSize = (cellSize, cellSize)
4. Set container.sizeDelta = (width * cellSize, height * cellSize)
5. Align container position to InventoryView's position
```

**Scene hierarchy**:
```
InventoryView (RectTransform)
├── BackgroundContainer (GridLayoutGroup)
│   ├── Template (Image, initially active, becomes hidden)
│   ├── Cell(Clone) ← generated
│   ├── Cell(Clone) ← generated
│   └── ... (width × height cells)
└── ItemContainer (RectTransform)
    ├── ItemView(Clone) ← spawned on ItemPlacedEvent
    └── ...
```

#### OnItemPlaced — Spawning Item Visuals

```
1. Instantiate(itemSO.prefab, _itemContainer)
2. Calculate anchored position:
     worldPos = model.GetItemWorldPosition(origin, dir, itemSO)
     rectTransform.anchoredPosition = worldPos
3. Apply rotation:
     localRotation = Euler(0, 0, -GetRotationAngle(dir))
     (negative Z because Unity UI rotates clockwise for positive Z)
4. Get/Add ItemView component → Initialize(itemModel, eventBus, inventory, container)
5. Z-ordering:
     Bag  → SetAsFirstSibling()  (renders behind)
     Regular → SetAsLastSibling() (renders in front)
6. Track: _itemViews[itemModel] = itemView
```

#### OnItemRemoved — Destroying Item Visuals

```
1. Find ItemView in _itemViews dictionary by ItemModel reference
2. Destroy(view.gameObject)
3. Remove from dictionary
```

### 5.2 ItemView — Single Item Visual

Thin wrapper that holds references for drag operations:

```csharp
public class ItemView : MonoBehaviour
{
    public ItemModel ItemModel;      // The data this visual represents
    public IEventBus EventBus;       // For publishing drag events
    public InventoryModel Inventory; // For grid queries during drag
    public RectTransform ItemContainer; // Parent container for offset calculation
}
```

On `Initialize()`:
1. Stores all references
2. Ensures `ItemDragHandler` component exists (adds if missing)
3. Calls `dragHandler.Setup(this)`

### 5.3 ItemDragHandler — Drag Input

Implements Unity's drag interfaces: `IPointerDownHandler`, `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler`.

#### OnBeginDrag — Offset Calculation (Critical for Snap)

```
1. Guard: CanRemoveItem check (can't drag bags with items on them)

2. Save original position for potential rollback

3. Convert screen position → local position in ItemContainer:
     RectTransformUtility.ScreenPointToLocalPointInRectangle(
         container, eventData.position, camera, out localPoint)

4. Calculate GRID offset (which cell of the item was clicked):
     mouseGridPos = inventory.GetGridPosition(localPoint)
     gridPositionOffset = mouseGridPos - itemModel.Origin
     
     Example: Item at origin (2,3), click on cell (3,4)
     → gridPositionOffset = (1,1)
     → "User grabbed the item 1 cell right, 1 cell up from its origin"

5. Calculate ANCHORED offset (pixel-precise click point):
     anchoredPositionOffset = localPoint - rectTransform.anchoredPosition
     
     Then add rotation offset to compensate pivot:
     anchoredPositionOffset += rotationOffset * cellSize

6. Visual feedback: alpha=0.7, blocksRaycasts=false

7. Publish ItemDragStartEvent with all offset data
```

#### OnDrag — Visual Follow

```csharp
rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
```

Simple pixel movement. No snapping during drag — item follows cursor freely.

#### OnEndDrag — Trigger Placement

```
1. Restore alpha=1.0, blocksRaycasts=true
2. Publish ItemDragEndEvent with current screen position
   → DragDropSystem handles the actual model logic
```

### 5.4 DragDropSystem — Drag Orchestrator

Subscribes to `ItemDragStartEvent` and `ItemDragEndEvent`.

#### OnDragStart — Cache State

Stores: dragging item, source inventory, all offsets, current direction.

#### Update — Rotation During Drag

```csharp
if (Input.GetKeyDown(KeyCode.R))
    _currentDir = ItemSO.GetNextDir(_currentDir);  // Down→Left→Up→Right→Down
```

#### OnDragEnd — The Snap + Placement Algorithm

```
1. Save original position (for fallback)
     originalOrigin = item.Origin
     originalDir = item.Dir

2. Remove from source inventory:
     sourceInv.RemoveItem(item)
     → publishes ItemRemovedEvent
     → InventoryView.OnItemRemoved destroys old visual

3. Find target inventory:
     foreach InventoryView in _inventoryViews:
       a. Convert screen → local in that view's ItemContainer:
            ScreenPointToLocalPointInRectangle(container, screenPos, ..., out localPoint)
       b. Convert local → grid position:
            gridPos = model.GetGridPosition(localPoint)
       c. Apply grid offset (compensate for grab point):
            candidateOrigin = gridPos - _gridPositionOffset
            
            Example: Dropped at grid (5,6), gridOffset was (1,1)
            → candidateOrigin = (4,5)
            → "Item origin should be at (4,5) so the grabbed cell lands at (5,6)"
       d. Bounds check:
            model.IsValidGridPosition(candidateOrigin)?
            → yes: this is the target inventory, break

4. Try place on target:
     targetInv.TryPlaceItem(item.ItemSO, targetOrigin, _currentDir)
     → validates Bag/Regular rules
     → publishes ItemPlacedEvent
     → InventoryView.OnItemPlaced spawns new visual at snapped position

5. If placement failed (occupied, out of bounds, no bag):
     sourceInv.TryPlaceItem(item.ItemSO, originalOrigin, originalDir)
     → "Return to sender" — item snaps back to where it was
```

**The "Snap" effect**: There is NO smooth interpolation. When `TryPlaceItem` succeeds, the old visual is destroyed and a new one is instantiated at the exact grid-aligned position (`GetItemWorldPosition`). This creates an instant snap.

---

## 6. Grid Snap Math — Detailed

### World ↔ Grid Conversion

```
cellSize = 50px

Grid (2,3) → World:
  x = 2 * 50 + 0 = 100px
  y = 3 * 50 + 0 = 150px
  → anchoredPosition = (100, 150)

World (137, 220) → Grid:
  x = Floor(137 / 50) = Floor(2.74) = 2
  y = Floor(220 / 50) = Floor(4.40) = 4
  → grid = (2, 4)
```

### Visual Positioning with Rotation

```
Item: width=1, height=3, origin=(2,1), dir=Left

1. Base position = GetWorldPosition(2,1) = (100, 50)

2. Rotation offset = GetRotationOffset(Left) = (0, width) = (0, 1)
   Pixel offset = (0, 1) * 50 = (0, 50)

3. Final position = (100, 50) + (0, 50) = (100, 100)

4. Rotation = Euler(0, 0, -90°)
   → The sprite rotates 90° clockwise, pivot adjusted by offset
```

### Drag Offset — Why It Matters

Without offset compensation, dropping an item would always place its origin at the cursor cell. But the user might grab a 3×1 item at its right end:

```
Item: width=3, height=1, origin=(0,0)
Cells occupied: (0,0)(1,0)(2,0)

User grabs at cell (2,0):
  gridPositionOffset = (2,0) - (0,0) = (2,0)

User drops at cursor cell (7,0):
  candidateOrigin = (7,0) - (2,0) = (5,0)
  → Item placed at origin (5,0), occupying (5,0)(6,0)(7,0)
  → The grabbed cell (7,0) stays under the cursor ✓
```

---

## 7. Bag/Regular Layer System

```
Grid visualization (single cell):

  ┌─────────────┐
  │  Regular     │  ← Layer 1 (rendered on top, SetAsLastSibling)
  │  ┌─────────┐ │
  │  │   Bag   │ │  ← Layer 0 (rendered behind, SetAsFirstSibling)
  │  └─────────┘ │
  └─────────────┘

Placement rules:
  Bag     → cell must NOT have a bag already
  Regular → cell MUST have a bag AND must NOT have a regular item

Removal rules:
  Bag     → ALL cells it covers must have NO regular items on top
  Regular → always removable
```

---

## 8. DI & Scene Setup

### Required Scene Hierarchy

```
SCN_Inventory
├── [SceneScope]                    ← Reflex SceneScope component
│   └── [CoreInstaller]             ← Registers IEventBus singleton
│
├── InventoryCanvas (Canvas)
│   ├── InventoryView               ← InventoryView component
│   │   │  Config: InventoryConfigSO (drag asset)
│   │   │  ItemContainer: → ref to ItemContainer below
│   │   │  BackgroundContainer: → ref to BG below
│   │   │  BackgroundTemplate: → ref to Template below
│   │   │
│   │   ├── BackgroundContainer     ← GridLayoutGroup + RectTransform
│   │   │   └── Template            ← Image (the cell background sprite)
│   │   └── ItemContainer           ← RectTransform (items spawn here)
│   │
│   └── DragDropSystem              ← DragDropSystem component
│       InventoryViews: [InventoryView]  ← drag ref
│
└── InventoryTest (optional)
      InventoryView: → ref to InventoryView above
      InitialItems: [list of ItemSO + positions]
```

### Injection Flow

```
1. Reflex SceneScope builds Container
2. CoreInstaller registers: IEventBus → new EventBus() (singleton)
3. Reflex scans scene for [Inject] methods:
   ├─ InventoryView.Construct(IEventBus)  ← receives shared EventBus
   └─ DragDropSystem.Construct(IEventBus) ← receives same EventBus
4. Unity Start() runs:
   ├─ InventoryView.Start() → new InventoryModel → Subscribe → BuildBackground
   └─ DragDropSystem.Start() → Subscribe drag events
5. Next frame: InventoryTest.SpawnItems() → TryPlaceItem calls
```

---

## 9. Save / Load

```csharp
// Save
string json = inventoryModel.Save();
// → {"items":[{"itemSOName":"Sword","origin":{"x":2,"y":3},"dir":0}, ...]}

// Load
inventoryModel.Load(json);
// → For each entry: config.GetItemSOByName(name) → TryPlaceItem(so, origin, dir)
// → Each TryPlaceItem publishes ItemPlacedEvent → View creates visuals
```

---

## 10. Complete Event Sequence — Place Item

```
External call: inventoryModel.TryPlaceItem(swordSO, (2,3), Down)
  │
  ├─ Validate: cells (2,3)(2,4)(2,5) — all have bags, no regular items ✓
  ├─ Create: ItemModel(swordSO, (2,3), Down)
  ├─ Occupy: cell(2,3).SetItem(item), cell(2,4).SetItem(item), ...
  ├─ Track: _items.Add(item)
  └─ Publish: ItemPlacedEvent { Item=item, Inventory=this }
       │
       └─ EventBus dispatches to all subscribers
            │
            └─ InventoryView.OnItemPlaced(evt)
                 ├─ Filter: evt.Inventory == _inventory? ✓
                 ├─ Instantiate(swordSO.prefab, _itemContainer)
                 ├─ Position: anchoredPosition = GetItemWorldPosition(...)
                 ├─ Rotate: localRotation = Euler(0,0,-0°) = identity
                 ├─ ItemView.Initialize(item, eventBus, inventory, container)
                 │    └─ ItemDragHandler.Setup(itemView)
                 ├─ Z-order: SetAsLastSibling() (Regular)
                 └─ Track: _itemViews[item] = itemView
```

## 11. Complete Event Sequence — Drag & Drop

```
User clicks item at screen (400, 300):

ItemDragHandler.OnBeginDrag
  ├─ CanRemoveItem(item)? ✓
  ├─ ScreenToLocal → localPoint = (150, 100)
  ├─ GridPos = (3, 2), Item.Origin = (2, 1)
  ├─ gridPositionOffset = (1, 1)
  ├─ anchoredPositionOffset = localPoint - anchoredPos + rotOff
  ├─ Alpha = 0.7, blocksRaycasts = false
  └─ Publish: ItemDragStartEvent { offsets, dir }
       └─ DragDropSystem.OnDragStart → caches state

User drags (multiple OnDrag calls):
  └─ anchoredPosition += delta / scaleFactor  (free movement)

User presses R during drag:
  └─ DragDropSystem.Update: _currentDir = Left (was Down)

User releases at screen (600, 500):

ItemDragHandler.OnEndDrag
  ├─ Alpha = 1, blocksRaycasts = true
  └─ Publish: ItemDragEndEvent { screenPos = (600,500) }
       │
       └─ DragDropSystem.OnDragEnd
            ├─ sourceInv.RemoveItem(item)
            │    ├─ Clear cells, publish ItemRemovedEvent
            │    └─ InventoryView destroys old visual
            │
            ├─ Find target: foreach view:
            │    ScreenToLocal(600,500) → localPoint
            │    gridPos = (6, 5)
            │    candidate = (6,5) - (1,1) = (5, 4)
            │    IsValid(5,4)? ✓ → target found
            │
            ├─ targetInv.TryPlaceItem(itemSO, (5,4), Left)
            │    ├─ Validate rotated cells ✓
            │    ├─ Publish ItemPlacedEvent
            │    └─ InventoryView spawns new visual at snapped position
            │
            └─ (If failed: sourceInv.TryPlaceItem back at original pos)
```
