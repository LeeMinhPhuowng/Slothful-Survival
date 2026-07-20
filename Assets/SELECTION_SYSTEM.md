# Selection System – Tài liệu kỹ thuật

---

## 1. Tổng quan

Khi user click vào item trên inventory grid, item đó hiện **4 góc vuông** (corner brackets) ở 4 góc.
Chỉ **1 item** được chọn tại 1 thời điểm trên mỗi inventory. Click item khác → bỏ chọn cũ, chọn mới.

```
  Chưa chọn:              Đã chọn:

  ┌────────────┐           ┌─            ─┐
  │            │           │              │
  │   ITEM     │    →      │    ITEM      │
  │            │           │              │
  └────────────┘           └─            ─┘
                           
                           4 góc L-shape, màu gold
                           len=12px, thick=3px, pad=2px
```

---

## 2. Files liên quan

| File | Vai trò |
|------|---------|
| [ItemSelectedEvent.cs](file:///d:/Fantasy-Survivors/Assets/_Project/Features/Inventory/Scripts/Events/ItemSelectedEvent.cs) | Event payload: `{Item, Inventory}` |
| [ItemDragHandler.cs](file:///d:/Fantasy-Survivors/Assets/_Project/Features/Inventory/Scripts/View/ItemDragHandler.cs) | **Publisher** – click/drag → publish event |
| [InventoryView.cs](file:///d:/Fantasy-Survivors/Assets/_Project/Features/Inventory/Scripts/View/InventoryView.cs) | **Manager** – track `_selectedItemView`, deselect cũ / select mới |
| [ItemView.cs](file:///d:/Fantasy-Survivors/Assets/_Project/Features/Inventory/Scripts/View/ItemView.cs) | **Visual** – tạo + hiện/ẩn 4 góc vuông |

---

## 3. Event Flow

```
User click item
      │
      ▼
ItemDragHandler.OnPointerClick()
      │
      ▼
EventBus.Publish( ItemSelectedEvent { Item, Inventory } )
      │
      ├──────────────────────────────────┐
      ▼                                  ▼
InventoryView A                    InventoryView B
  evt.Inventory == _inventory?       evt.Inventory == _inventory?
  ✓ YES → xử lý                     ✗ NO → bỏ qua
      │
      ▼
OnItemSelected()
  ├─ _selectedItemView?.Deselect()     ← ẩn corners cũ
  ├─ _selectedItemView = null
  ├─ _itemViews.TryGetValue(evt.Item) → view
  ├─ view.Select()                     ← hiện corners mới
  └─ _selectedItemView = view
```

### Khi drag bắt đầu

```
ItemDragHandler.OnBeginDrag()
      │
      ├─ Publish ItemSelectedEvent   ← select item đang drag
      └─ Publish ItemDragStartEvent  ← bắt đầu drag logic
```

### Khi item bị xoá (drag đi nơi khác / remove)

```
InventoryView.OnItemRemoved()
      │
      ├─ if _selectedItemView == view đang xoá:
      │     _selectedItemView = null     ← tự bỏ chọn
      └─ Destroy(view.gameObject)        ← corners bị destroy cùng
```

---

## 4. Cấu trúc GameObject của Corner Brackets

Tạo hoàn toàn bằng code trong `ItemView.CreateSelectionFrame()`, không cần prefab.

```
ItemView (RectTransform)
└── SelectionFrame (RectTransform + CanvasGroup)
    │   anchorMin=(0,0) anchorMax=(1,1)    ← stretch fill parent
    │   blocksRaycasts=false               ← không chặn click
    │
    ├── Corner_TL (RectTransform)
    │   │   anchor=(0,1) pivot=(0,1)       ← neo góc trái-trên
    │   │   anchoredPos=(+pad, -pad)       ← dịch vào trong
    │   │   sizeDelta=(len, len)           ← bounding box
    │   ├── Bar_H (Image)                  ── thanh ngang
    │   │   anchor/pivot=(0,1)
    │   │   sizeDelta=(len, thick)         ← 12×3 px
    │   └── Bar_V (Image)                  │ thanh dọc
    │       anchor/pivot=(0,1)
    │       sizeDelta=(thick, len)         ← 3×12 px
    │
    ├── Corner_TR (RectTransform)
    │   │   anchor=(1,1) pivot=(1,1)       ← neo góc phải-trên
    │   │   anchoredPos=(-pad, -pad)
    │   ├── Bar_H ──
    │   └── Bar_V  │
    │
    ├── Corner_BL (RectTransform)
    │   │   anchor=(0,0) pivot=(0,0)       ← neo góc trái-dưới
    │   │   anchoredPos=(+pad, +pad)
    │   ├── Bar_H ──
    │   └── Bar_V  │
    │
    └── Corner_BR (RectTransform)
        │   anchor=(1,0) pivot=(1,0)       ← neo góc phải-dưới
        │   anchoredPos=(-pad, +pad)
        ├── Bar_H ──
        └── Bar_V  │
```

### Chi tiết cách neo mỗi góc

Mỗi corner dùng anchor+pivot cùng giá trị → neo chính xác vào góc parent.
`anchoredPosition` dịch vào trong theo `_cornerPadding`.

```
               pad                        pad
          ◄───────►                  ◄───────►
     ┌─── ═══════════                ═══════════ ───┐  ▲
     │    ║ TL corner                TR corner ║    │  │ pad
     ║    ║                                    ║    ║  ▼
     ║                                              ║
     ║                   ITEM                       ║
     ║                                              ║
     ║    ║                                    ║    ║  ▲
     │    ║ BL corner                BR corner ║    │  │ pad
     └─── ═══════════                ═══════════ ───┘  ▼

     ═ = Bar (Image), dày 3px, dài 12px
```

### Tại sao mỗi corner cần 2 bars?

Một hình chữ L không thể tạo bằng 1 Image đơn (trừ khi dùng sprite custom).
Dùng 2 Image thanh mỏng ghép vuông góc:

```
  Corner TL:          Corner TR:          Corner BL:          Corner BR:
  
  ┌──── Bar_H         Bar_H ────┐                             
  │                           │         │                           │
  Bar_V                   Bar_V         Bar_V                   Bar_V
                                        │                           │
                                        └──── Bar_H         Bar_H ────┘
```

---

## 5. Tuỳ chỉnh Visual

Cấu hình qua `[SerializeField]` trên `ItemView`:

```csharp
[Header("Selection Visual Settings")]
Color _cornerColor     = (1, 0.85, 0.25, 1)  // Gold
float _cornerLength    = 12f                   // Chiều dài mỗi cánh (px)
float _cornerThickness = 3f                    // Độ dày thanh (px)
float _cornerPadding   = 2f                    // Khoảng cách từ mép item (px)
```

| Param | Hiệu ứng khi thay đổi |
|-------|----------------------|
| `_cornerColor` | Đổi màu tất cả 8 bars (4 corners × 2 bars) |
| `_cornerLength` | Cánh dài hơn/ngắn hơn – giá trị lớn = khung rõ hơn |
| `_cornerThickness` | Thanh dày hơn/mỏng hơn |
| `_cornerPadding` | Dịch corners ra ngoài (âm) hoặc vào trong (dương) |

### Ví dụ tuỳ chỉnh

```
Mặc định (len=12, thick=3):        Lớn hơn (len=20, thick=4):

  ┌─       ─┐                      ┌───         ───┐
  │  ITEM   │                      │               │
  └─       ─┘                      │     ITEM      │
                                   │               │
                                   └───         ───┘
```

---

## 6. Lifecycle

```
ItemView.Initialize()
    │
    ├─ Setup ItemDragHandler
    └─ CreateSelectionFrame()      ← tạo 4 corners (ẩn)
         │
         └─ _selectionFrame.SetActive(false)

         ┌─── User click ───────────────────────────────┐
         │                                              │
         ▼                                              │
    ItemView.Select()                                   │
         │                                              │
         └─ _selectionFrame.SetActive(true)  ← hiện    │
              _isSelected = true                        │
                                                        │
         ┌─── User click item khác ─────────────────────┤
         │                                              │
         ▼                                              │
    ItemView.Deselect()                                 │
         │                                              │
         └─ _selectionFrame.SetActive(false) ← ẩn      │
              _isSelected = false                       │
                                                        │
         ┌─── Item bị xoá (RemoveItem) ────────────────┘
         │
         ▼
    InventoryView.OnItemRemoved()
         │
         ├─ _selectedItemView = null
         └─ Destroy(view.gameObject)
              └─ SelectionFrame bị destroy cùng parent
```

---

## 7. Tương tác với Rotation

Corners là con của `ItemView` (RectTransform). Khi item xoay, corners **xoay theo** vì nằm trong transform hierarchy.

```
Item Dir.Down (0°):              Item Dir.Left (90°):

  ┌─       ─┐                     ┌─           ─┐
  │         │                     │              │
  │  ITEM   │          →          │    ITEM      │
  │         │                     └─           ─┘
  └─       ─┘                     
  2×3 cells                       3×2 cells (rotated)
```

`SelectionFrame` dùng `anchorMin=(0,0)` `anchorMax=(1,1)` → **tự co giãn** theo kích thước parent RectTransform. Khi item đổi shape do rotation, corners tự điều chỉnh vị trí.

---

## 8. Edge Cases

| Case | Behaviour |
|------|-----------|
| Click item đang chọn | `Select()` return sớm (`if _isSelected return`) → không thay đổi |
| Click vùng trống | Không trigger (chỉ item có `ItemDragHandler`) |
| Drag item đang chọn đi | Corners bị destroy cùng visual cũ. Visual mới ở vị trí drop sẽ không có corners (chưa chọn lại) |
| Drag item A, click item B | `OnPointerClick` không fire khi đang drag (Unity ưu tiên drag) |
| Item bị xoá lúc đang chọn | `OnItemRemoved` set `_selectedItemView = null` trước destroy |
| Nhiều InventoryView | Mỗi view track `_selectedItemView` riêng. Select trên view A không ảnh hưởng view B (filter bằng `evt.Inventory`) |
