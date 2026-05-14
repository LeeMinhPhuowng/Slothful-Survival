# Game Design Overview: Apocaloop (Working Title)

## 1. Tổng quan Dự án (Project Concept)
*   **Thể loại**: Roguelike / Survivors-like Action.
*   **Nền tảng**: PC / Mobile (Unity Engine).
*   **Mục tiêu**: Người chơi điều khiển nhân vật sống sót qua các đợt tấn công của kẻ thù, thu thập kinh nghiệm để thăng cấp và nâng cấp kho vũ khí đa dạng.

---

## 2. Vòng lặp Gameplay Cốt lõi (Core Gameplay Loop)
1.  **Chiến đấu**: Tiêu diệt kẻ địch bằng hệ thống vũ khí tự động hoặc định hướng.
2.  **Thu thập**: Nhặt các hạt EXP rơi ra từ quái vật.
3.  **Thăng cấp**: Khi đủ EXP, người chơi lên cấp, nhận hiệu ứng hình ảnh (Aura, Floating Text) và game tạm dừng để chọn nâng cấp.
4.  **Tiến hóa**: Lựa chọn vũ khí mới hoặc nâng cấp chỉ số (Augments) để đối đầu với các đợt quái mạnh hơn.

---

## 3. Hệ thống Vũ khí (Weapon System)
Đây là trái tim của trò chơi, được thiết kế theo kiến trúc **Behavior-Driven**:
*   **Cấu trúc kỹ thuật**:
    *   `Weapon`: Class quản lý hồi chiêu và cấp độ.
    *   `AWeaponBehaviour`: Class trừu tượng cho phép mở rộng vô số loại vũ khí khác nhau.
*   **Các loại vũ khí tiêu biểu**:
    *   **Melee (Cận chiến)**: `BaseSword`, `Bloodmoon` (Gây sát thương vùng xung quanh).
    *   **Ranged (Tầm xa)**: `BaseBow`, `BaseStaff`, `Devastator` (Bắn loạt 5 viên ngẫu nhiên), `PB28A` (Bắn Laser đa hướng).
    *   **Special (Đặc biệt)**:
        *   `Boomerang`: Bay đến mục tiêu rồi quay về Player.
        *   `Guardian`: Các thanh kiếm xoay quanh bảo vệ người chơi.
        *   `Shield`: Hào quang gây sát thương định kỳ và bảo vệ.

---

## 4. Hệ thống Kẻ thù (Enemy System)
*   **Trí tuệ nhân tạo (AI)**:
    *   Tự động đuổi theo người chơi.
    *   **Separation Logic**: Cơ chế tự động giữ khoảng cách (0.6 đơn vị) giữa các quái vật để tránh chồng lấn (Clumping).
*   **Cơ chế sát thương**:
    *   Gây sát thương va chạm (Contact Damage) cho Player.
    *   Sử dụng **I-Frames (Thời gian bất tử)** để khống chế tốc độ mất máu của Player.

---

## 5. Kiến trúc Kỹ thuật (Technical Architecture)
*   **Object Pooling**: Tối ưu hóa hiệu năng bằng cách tái sử dụng đạn (Projectile) và kẻ địch (Enemy), đảm bảo game chạy mượt dù có hàng trăm đối tượng trên màn hình.
*   **ScriptableObjects (SO)**: Quản lý toàn bộ dữ liệu cân bằng (Sát thương, Tốc độ, Hồi chiêu) thông qua `WeaponInfoSO` và `EnemyInfoSO`, cho phép chỉnh sửa nhanh mà không cần động vào code.
*   **DOTween Integration**:
    *   Sử dụng để tạo hiệu ứng mượt mà cho UI (Upgrade Panel phóng to dần).
    *   Hiệu ứng chữ Level Up bay lên và mờ dần.

---

## 6. Trải nghiệm người dùng & Hình ảnh (UX/VFX)
*   **Feedback Hệ thống**:
    *   Kẻ địch và Player chớp trắng (Flash) khi nhận sát thương.
    *   Thanh máu (Healthbar) tự động cập nhật mượt mà.
*   **Hiệu ứng Thăng cấp**:
    *   Hào quang (Aura) rực rỡ dưới chân.
    *   Dòng chữ "Level Up" bay từ dưới lên trong 1 giây trước khi mở bảng chọn.

---

## 7. Định hướng Phát triển (Future Roadmap)
*   Thêm hệ thống Boss ở các mốc thời gian cố định.
*   Phát triển thêm các loại đạn đuổi (Homing Projectiles) phức tạp hơn.
*   Xây dựng hệ thống bản đồ đa dạng (Maps/Biomes).
