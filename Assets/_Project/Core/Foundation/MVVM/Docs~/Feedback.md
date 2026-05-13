# Feedback

## 1. **[Convention]** Đặt tên Assembly Definition của Demo

**Vị trí:** `Demo/MVVM.Demo.asmdef`

**Hiện tại:**
```json
{
    "name": "MVVM.Demo",
    ...
}
```

**Kỳ vọng:**
```json
{
    "name": "Core.Foundation.MVVM.Demo",
    ...
}
```

---

## 2. **[Convention]** Tài liệu mô tả

**Vị trí:** `Docs/`
Đổi tên `BaseViewModel.md` → `README.md` và tái cấu trúc nội dung theo định dạng 5 phần:
1. Tiêu đề & Tóm tắt
2. Các phụ thuộc (Dependencies)
3. Các Class chính
4. Ví dụ sử dụng
5. Ghi chú kiến trúc

---

## 3. **[Convention]** Đặt tên Scene thiếu tiền tố

**Vị trí:** `Demo/Scenes/Demo.unity`

Các tệp Scene phải sử dụng tiền tố `SCN_`:
```
Demo.unity → SCN_MVVMDemo.unity
```

---

## 4. **[Code Quality]** Đặt tên trường trong HealthView không nhất quán

**Vị trí:** `Demo/Scripts/View/HealthView.cs:17`

```csharp
private Reactive.DisposableBag _subscriptions;
```

Nên sử dụng kiểu đầy đủ hoặc chỉ thị `using`:
```csharp
using Core.Foundation.Reactive;
// ...
private DisposableBag _subscriptions;
```

Triển khai hiện tại đã có `using Core.Foundation.Reactive;` ở dòng 5, vì vậy tiền tố `Reactive.` là dư thừa.

---

## 5. **[Code Quality]** DemoBootstrap thiếu Null Safety Pattern

**Vị trí:** `Demo/DemoBootstrap.cs:50-51`

Phương thức `DisposeViewModels()` không kiểm tra null trước khi gọi Dispose:

```csharp
private void DisposeViewModels()
{
    _counterVM.Dispose();  // Có thể gây lỗi nếu CreateViewModels thất bại
    _healthVM.Dispose();
}
```

**Đề xuất:**
```csharp
private void DisposeViewModels()
{
    _counterVM?.Dispose();
    _healthVM?.Dispose();
}
```

---

## 6. **[Convention]** File Hierarchy

Tổ chức lại file và thư mục theo mục 4 - `Assets/CONVENTION_RULES.md`.

