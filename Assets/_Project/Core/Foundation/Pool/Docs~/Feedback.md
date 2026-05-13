# Feedback Pool

## 1. Các Vấn Đề Triển Khai
- **PoolManager Generic Casting**: Trong `PoolManager.cs`, `RegisterPool<T>` cố gắng ép kiểu `IPool<T>` thành `IPool<object>`.
    - **Vấn đề**: `IPool<T>` không phải là covariant (và không thể là covariant vì `Release(T)`). Việc ép kiểu này sẽ luôn trả về `null` cho bất kỳ `T` nào không phải chính xác là `object`.
    - **Kết quả**: Các Pool không được lưu trữ chính xác. `Get<T>` và `Release<T>` sẽ thất bại hoặc ném ra NullReferenceException.
    - **Khắc phục**: Thay đổi `_pools` thành `Dictionary<string, object>` và ép kiểu về `IPool<T>` khi lấy ra trong `Get<T>` và `Release<T>`.

- **Thống Kê PoolManager (Stats)**:
    - `TotalCreated` trong `PoolStats` được định nghĩa nhưng không bao giờ được tăng lên. Interface `IPool` hiện tại không hỗ trợ báo cáo sự kiện tạo mới cho manager.

## 2. Conventions
- **Naming**:
    - **Parameters**: Constructor trong `MonoPool` và `ObjectPoolWrapper` sử dụng PascalCase (`DefaultCapacity`, `MaxSize`).
        - *Quy tắc*: Phải là **camelCase** (ví dụ: `defaultCapacity`, `maxSize`).
        - *Tệp*: `MonoPool.cs`, `ObjectPoolWrapper.cs`.

- **Format**:
    - `MonoPool.cs`: Dòng 7 `class MonoPool<T> : IPool<T> where T : MonoBehaviour , IPoolable`. Khoảng trắng thừa trước dấu phẩy.
    - `MonoPool.cs`: Dòng 18 quá dài. Nên ngắt các đối số thành nhiều dòng.

- **Redundant Code**:
    - `ObjectPoolWrapper.cs`: Constructor chấp nhận `Action<T> actionOnDestroy` nhưng bỏ qua nó trong quá trình khởi tạo `ObjectPool` (sử dụng lambda gọi `OnDestroyItem` thay thế).

## 3. Logic Chung
- **MonoPool/ObjectPoolWrapper**:
    - `ObjectPool` được khởi tạo với `collectionCheck: false`. Mặc dù tốt cho performance, nhưng có lẽ nên được cấu hình hoặc để mặc định là `true` cho các bản Editor/Debug để bắt lỗi double-release (giải phóng 2 lần).

## 4. File hierarchy.
Tổ chức lại file và thư mục theo phần 4 - `Assets/CONVENTION_RULES.md`
