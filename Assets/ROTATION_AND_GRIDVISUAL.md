# Rotation & Grid Visual – Tài liệu kỹ thuật

---

## Phần A: Rotation System (Đã implement)

### A1. Dữ liệu cốt lõi

Mỗi `ItemSO` định nghĩa shape (width × height). Rotation không thay đổi SO — chỉ thay đổi **cách đọc** width/height lúc runtime.

```
enum Dir { Down=0, Left=1, Up=2, Right=3 }

Cycle: Down → Left → Up → Right → Down  (nhấn R)
```

### A2. Ba hàm tạo thành bộ ba rotation

Ví dụ xuyên suốt: **Item width=2, height=3, origin=(1,1), cellSize=50px**

#### Hàm 1: `GetGridPositionList(origin, dir)` — Cells bị chiếm

Quy tắc: Down/Up giữ nguyên W×H. Left/Right **hoán đổi** W↔H.

```
DIR.DOWN (2×3 giữ nguyên)        DIR.LEFT (hoán W↔H → 3×2)

  y                                y
  4│                               3│
  3│ ■ ■                           2│ ■ ■ ■
  2│ ■ ■                           1│ ■ ■ ■ ← origin
  1│ ■ ■ ← origin                  └─────────
   └──────                           1 2 3  x
     1 2  x
                                   Cells: (1,1)(1,2)
  Cells: (1,1)(1,2)(1,3)                  (2,1)(2,2)
         (2,1)(2,2)(2,3)                  (3,1)(3,2)
```

```
DIR.UP (2×3 giữ nguyên)          DIR.RIGHT (hoán W↔H → 3×2)

  y                                y
  4│                               3│
  3│ ■ ■                           2│ ■ ■ ■
  2│ ■ ■                           1│ ■ ■ ■ ← origin
  1│ ■ ■ ← origin                  └─────────
   └──────                           1 2 3  x
     1 2  x
                                   Cells giống LEFT
  Cells giống DOWN
```

> **Lưu ý**: DOWN và UP cho ra cells giống nhau. LEFT và RIGHT cũng giống nhau.
> Sự khác biệt nằm ở **sprite rotation** và **offset bù pivot**.

#### Hàm 2: `GetRotationAngle(dir)` — Góc xoay sprite

```
Down  →   0°   (sprite gốc)
Left  →  90°   (xoay ngược chiều kim đồng hồ)
Up    → 180°   (lật ngược)
Right → 270°   (xoay thuận chiều kim đồng hồ)
```

Unity UI áp dụng: `localRotation = Euler(0, 0, -angle)`

Dấu trừ vì `Euler Z dương = ngược chiều kim đồng hồ` trong Unity,
nhưng giá trị angle tăng theo chiều thuận → cần negate.

#### Hàm 3: `GetRotationOffset(dir)` — Bù pivot (quan trọng nhất)

Sprite pivot ở góc trái-dưới. Khi xoay, sprite bay khỏi cells nó chiếm.
Offset bù lại để sprite khớp cells.

```
Down  → (0, 0)              Không bù
Left  → (0, width)          Dịch lên width cells
Up    → (width, height)     Dịch phải width + lên height cells  
Right → (height, 0)         Dịch phải height cells
```

### A3. Minh họa trực quan — Tại sao cần offset

**Item 2×3, cellSize=50. Tất cả 4 hướng tại origin (1,1):**

```
DIR.DOWN                          DIR.LEFT
angle=0°, offset=(0,0)           angle=90°, offset=(0,2)

  ┌────────┐ sprite               ┌──────────────┐ sprite
  │        │ 100×150px            │              │ 150×100px
  │ sprite │                      │    sprite    │ (đã xoay 90°)
  │        │                      │              │
  └────────┘                      └──────────────┘
  pos = (50,50)                   pos = (50, 50)+(0,100) = (50,150)
  
  Grid cells: y=1,2,3 x=1,2      Grid cells: y=1,2 x=1,2,3
  ■ ■ (50,50)→(150,200)          ■ ■ ■ (50,50)→(200,150)
  Sprite khớp cells ✓            Sprite khớp cells ✓


DIR.UP                            DIR.RIGHT
angle=180°, offset=(2,3)         angle=270°, offset=(3,0)

  ┌────────┐ sprite               ┌──────────────┐ sprite
  │        │ (lật ngược)          │              │ (xoay 270°)
  │        │                      │              │
  └────────┘                      └──────────────┘
  pos = (50,50)+(100,150)         pos = (50,50)+(150,0) = (200,50)
      = (150,200)
  
  Grid cells: y=1,2,3 x=1,2      Grid cells: y=1,2 x=1,2,3
  Sprite khớp cells ✓            Sprite khớp cells ✓
```

### A4. Công thức tổng quát

```
finalPos = grid.GetWorldPosition(origin.x, origin.y)
         + Vector3(rotOffset.x, rotOffset.y) * cellSize

finalRot = Quaternion.Euler(0, 0, -GetRotationAngle(dir))
```

Hai giá trị này luôn **đồng bộ** với `GetGridPositionList` → sprite nằm chính xác
trên tập cells mà model đã chiếm.

### A5. Runtime flow khi nhấn R

```
User nhấn R lúc đang drag
        │
        ▼
DragDropSystem.Update()
  _currentDir = GetNextDir(_currentDir)
  // Down→Left→Up→Right→Down
  // Chỉ lưu dir mới, KHÔNG thay đổi gì trên model hay visual
        │
User thả chuột
        │
        ▼
DragDropSystem.OnDragEnd()
  1. sourceInv.RemoveItem(item)           // xoá item cũ khỏi model
     → ItemRemovedEvent → View destroy    // xoá visual cũ
  2. targetInv.TryPlaceItem(              
       itemSO, newOrigin, _currentDir)    // ◀ dir MỚI
     → GetGridPositionList dùng dir mới   // cells MỚI
     → validate cells mới
     → ItemPlacedEvent → View spawn       // tạo visual MỚI
        anchoredPos = offset theo dir mới
        rotation = angle theo dir mới
```

---

## Phần B: Grid Visual / Drag Preview (Chưa implement)

Hiện tại: khi drag, item di chuyển tự do (pixel). Khi thả, snap tức thì (destroy + recreate).
**Không có viền preview** cho thấy item sẽ đặt ở đâu.

### B1. Khái niệm

Grid Visual = tập hợp hình vuông viền (outline) hiển thị trên grid **trong lúc drag**,
cho thấy vị trí item sẽ snap vào nếu thả tại vị trí hiện tại.

```
Drag item 2×3 qua grid:

  ┌───┬───┬───┬───┬───┐
  │   │   │   │   │   │
  ├───┼───╔═══╦═══╗───┤      ═══ = Grid Visual (viền xanh/đỏ)
  │   │   ║   ║   ║   │      
  ├───┼───╠═══╬═══╣───┤      Xanh = valid placement
  │   │   ║   ║   ║   │      Đỏ  = invalid (bị chiếm / ngoài bounds)
  ├───┼───╠═══╬═══╣───┤
  │   │   ║   ║   ║   │
  ├───┼───╚═══╩═══╝───┤
  │   │   │   │   │   │
  └───┴───┴───┴───┴───┘
           ▲
     cursor ở đây
```

### B2. Thiết kế — GridVisualController

```
Tên file:  View/GridVisualController.cs
Vị trí:    Đặt trên cùng GameObject với DragDropSystem
           hoặc con của InventoryView
```

#### Trách nhiệm

| Thứ | Việc |
|-----|------|
| 1 | Khi drag bắt đầu → tạo pool visual cells |
| 2 | Mỗi frame (Update) → tính grid position từ cursor → hiện viền |
| 3 | Đổi màu viền tuỳ valid/invalid |
| 4 | Khi dir thay đổi (R) → cập nhật shape viền |
| 5 | Khi drag kết thúc → ẩn tất cả viền |

#### Dữ liệu cần

```csharp
// Từ DragDropSystem (hoặc subscribe cùng events):
ItemSO        _dragItemSO;        // shape
ItemSO.Dir    _currentDir;        // dir hiện tại (đã xoay)
Vector2Int    _gridPositionOffset; // offset từ origin tới điểm grab

// Từ InventoryView:
InventoryModel _hoverInventory;   // inventory đang hover
RectTransform  _hoverContainer;   // container để convert toạ độ

// Prefab:
Transform gridVisualPrefab;       // 1 ô vuông viền (từ InventoryConfigSO)
```

#### Pseudocode — Update mỗi frame

```
void UpdateGridVisual()
{
    // 1. Tắt hết visual cũ
    HideAllVisuals();
    
    if (đang không drag) return;

    // 2. Tìm inventory đang hover
    foreach view in inventoryViews:
        ScreenToLocal(mousePos, view.ItemContainer) → localPoint
        gridPos = view.Model.GetGridPosition(localPoint)
        candidateOrigin = gridPos - _gridPositionOffset
        
        if view.Model.IsValidGridPosition(candidateOrigin):
            _hoverInventory = view.Model
            _hoverContainer = view.ItemContainer
            break

    if (_hoverInventory == null) return;

    // 3. Lấy danh sách cells theo dir hiện tại
    cells = _dragItemSO.GetGridPositionList(candidateOrigin, _currentDir)

    // 4. Với mỗi cell → hiển thị 1 visual
    foreach cell in cells:
        visual = GetOrCreateVisual(index)
        visual.SetActive(true)
        
        // Vị trí = toạ độ grid → pixel
        worldPos = grid.GetWorldPosition(cell.x, cell.y)
        visual.anchoredPosition = worldPos
        visual.sizeDelta = (cellSize, cellSize)
        
        // 5. Màu tuỳ valid/invalid
        if (!grid.IsValidGridPosition(cell)):
            visual.color = RED        // ngoài bounds
        elif (cell có Regular item):
            visual.color = RED        // bị chiếm
        elif (itemType == Regular && !cell.HasBag()):
            visual.color = RED        // thiếu bag
        else:
            visual.color = GREEN      // hợp lệ
}
```

#### Object Pooling cho visual cells

```
Kích thước pool tối đa = max(itemSO.width * itemSO.height) của bất kỳ item nào.
Thực tế: pool 16 cells là đủ (4×4 item lớn nhất).

// Tạo pool lúc drag start
for (int i = 0; i < maxCells; i++):
    visual = Instantiate(gridVisualPrefab, _hoverContainer)
    visual.SetActive(false)
    _pool.Add(visual)

// Trả pool lúc drag end
foreach visual in _pool:
    Destroy(visual)
_pool.Clear()
```

### B3. Tích hợp vào hệ thống event hiện tại

```
                        ┌──────────────────────┐
                        │  ItemDragStartEvent   │
                        └──────┬───────────────┘
                               │
              ┌────────────────┼───────────────────┐
              ▼                ▼                    ▼
    DragDropSystem     GridVisualController    (future...)
    (lưu state)        (tạo pool, bật mode)
              
              
                         Mỗi frame
                            │
                            ▼
                  GridVisualController.Update()
                  - đọc mousePos
                  - tính candidateOrigin
                  - vẽ viền cells
                  - đổi màu valid/invalid
                  - nếu R → dir đổi → cells đổi shape
              
              
                        ┌──────────────────────┐
                        │  ItemDragEndEvent     │
                        └──────┬───────────────┘
                               │
              ┌────────────────┼───────────────────┐
              ▼                ▼                    ▼
    DragDropSystem     GridVisualController    (future...)
    (remove + place)   (ẩn hết, destroy pool)
```

### B4. GridVisual Prefab

```
GridVisual.prefab
├── RectTransform
│   anchor: bottom-left
│   pivot: (0, 0)         ← khớp với grid coordinate system
│   sizeDelta: (50, 50)   ← = cellSize (hoặc set runtime)
├── Image
│   sprite: square outline (viền 2px)
│   color: white (đổi runtime)
│   raycastTarget: false  ← QUAN TRỌNG: không chặn drag events
└── CanvasGroup
    blocksRaycasts: false
    interactable: false
```

### B5. Ví dụ hoàn chỉnh — Drag item 2×3 đã xoay Left

```
Item: width=2, height=3, _currentDir = Left
Cursor ở screen (400, 300)
gridPositionOffset = (1, 0)  (grab ở giữa item)

1. ScreenToLocal → localPoint = (200, 150)
2. GetGridPosition(200,150) = (4, 3)  (cellSize=50)
3. candidateOrigin = (4,3) - (1,0) = (3, 3)

4. GetGridPositionList((3,3), Left):
   Left → hoán W↔H → iterate x=[0..3), y=[0..2)
   Cells: (3,3)(3,4)(4,3)(4,4)(5,3)(5,4)  ← 3 cột × 2 hàng

5. Vẽ 6 visual cells:
   Cell (3,3) → pos=(150,150) → HasBag? ✓ HasRegular? ✗ → GREEN
   Cell (3,4) → pos=(150,200) → HasBag? ✓ HasRegular? ✗ → GREEN
   Cell (4,3) → pos=(200,150) → HasBag? ✓ HasRegular? ✓ → RED
   Cell (4,4) → pos=(200,200) → HasBag? ✓ HasRegular? ✗ → GREEN
   Cell (5,3) → pos=(250,150) → HasBag? ✗             → RED
   Cell (5,4) → pos=(250,200) → HasBag? ✓ HasRegular? ✗ → GREEN

   Kết quả: 4 ô xanh, 2 ô đỏ → thả sẽ FAIL (TryPlaceItem trả false)

   ┌───┬───┬───┬───┬───┬───┐
   │   │   │   │   │   │   │
   ├───┼───┼───╔═══╦═══╦═══╗
   │   │   │   ║ ✓ ║ ✗ ║ ✗ ║ ← y=3
   ├───┼───┼───╠═══╬═══╬═══╣
   │   │   │   ║ ✓ ║ ✓ ║ ✓ ║ ← y=4
   ├───┼───┼───╚═══╩═══╩═══╝
   │   │   │   │   │   │   │
   └───┴───┴───┴───┴───┴───┘
               x=3 x=4 x=5
```

### B6. Edge Cases

| Case | Handling |
|------|----------|
| Cursor ngoài mọi inventory | Ẩn hết visual |
| Một số cells ngoài bounds | Cells ngoài → RED, cells trong → check valid |
| Nhấn R xoay | Tính lại cells ngay frame đó (shape thay đổi) |
| Di chuyển giữa 2 inventory | Chuyển parent visual sang container mới |
| Item là Bag | Check logic Bag: cell không được có bag sẵn |
