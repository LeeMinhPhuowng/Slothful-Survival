# UI MVVM System Overview

**Project:** Slothful Survival  
**Scope:** Refactor toan bo UI scene va overlay panel  
**Pattern:** MVVM + R3 Reactive + service/facade + DI-ready composition  
**Async scene loading:** UniTask (`com.cysharp.unitask`) + Unity `SceneManager.LoadSceneAsync`  
**Source of truth:** Yeu cau UI ngay 2026-05-08

---

## 1. Muc tieu

He thong UI can duoc to chuc lai quanh 3 scene chinh va cac overlay panel:

```text
SCN_MainMenu --ChooseMap Button--> SCN_ChoosingMap --Play Button--> SCN_Gameplay
```

Button Settings, Shop, Profile, Pause, Win/Lose/Revive/Augment khong chuyen scene truc tiep. Chung mo panel overlay tren scene hien tai.

Muc tieu ky thuat:

- Tach UI khoi `GameManager`, `PlayerSetter`, `LevelLoader`, `AugmentManager`.
- Moi scene co ViewModel rieng quan ly state va command cua scene.
- Moi panel co ViewModel rieng, co the mo tren nhieu scene.
- Shared panels nhu Settings/Shop/Profile khong bi duplicate logic.
- Gameplay state nhu EXP, pause, win/lose, revive, reward duoc expose reactive de UI tu cap nhat.
- Scene loading duoc thuc hien bat dong bo, co loading progress va khong cho bam load nhieu lan.

---

## 2. Scene va panel topology

```text
SCN_MainMenu
|-- MainMenuCanvas
|   |-- DynamicBackgroundPanel
|   |-- MainMenuHud
|   |-- PanelHost
|       |-- SettingsPanel
|       |-- ShopPanel
|       |-- ProfilePanel

SCN_ChoosingMap
|-- ChoosingMapCanvas
|   |-- ChoosingMapHud
|   |-- PanelHost
|       |-- SettingsPanel
|       |-- ShopPanel

SCN_Gameplay
|-- GameplayCanvas
|   |-- GameplayHud
|   |-- MovementJoystick
|   |-- AttackButton
|   |-- PanelHost
|       |-- ProfilePanel
|       |-- PausePanel
|       |-- AugmentPanel
|       |-- WinPanel
|       |-- RevivePanel
|       |-- LosePanel
```

PanelHost la noi quan ly overlay/pop up. Mot scene co the co nhieu panel nhung chi nen co mot service dieu phoi mo/dong.

---

## 3. Layer architecture

```text
Unity Scene Canvas / Prefab
        |
        v
View MonoBehaviours
        |
        v
ViewModels
        |
        v
UI Flow + Panel + Feature Services
        |
        v
Legacy Adapters / Domain Services
        |
        v
Current Gameplay Managers + ScriptableObject Data
```

| Layer | Trach nhiem | Vi du |
| --- | --- | --- |
| Scene View | Bind cac UI component cua scene | `MainMenuSceneView`, `GameplaySceneView` |
| Panel View | Bind UI component cua popup | `SettingsPanelView`, `ShopPanelView` |
| ViewModel | Reactive state + command | `GameplayHudViewModel`, `PausePanelViewModel` |
| UI Service | Scene flow, panel stack | `ISceneFlowService`, `IPanelService` |
| Feature Service | Economy, inventory, settings, combat input | `IWalletService`, `IInventoryService` |
| Adapter | Goi code prototype hien tai | `LegacyLevelService`, `LegacyPlayerService` |

---

## 4. Naming convention de xuat

### Scenes

- `SCN_MainMenu`
- `SCN_ChoosingMap`
- `SCN_Gameplay`

### Root objects

- `UIRoot_MainMenu`
- `UIRoot_ChoosingMap`
- `UIRoot_Gameplay`
- `PanelHost`

### Views

- `MainMenuSceneView`
- `ChoosingMapSceneView`
- `GameplaySceneView`
- `SettingsPanelView`
- `ShopPanelView`
- `ProfilePanelView`
- `AugmentPanelView`
- `PausePanelView`
- `WinPanelView`
- `RevivePanelView`
- `LosePanelView`

### ViewModels

- `MainMenuViewModel`
- `ChoosingMapViewModel`
- `GameplayHudViewModel`
- `SettingsPanelViewModel`
- `ShopPanelViewModel`
- `ProfilePanelViewModel`
- `AugmentPanelViewModel`
- `PausePanelViewModel`
- `WinPanelViewModel`
- `RevivePanelViewModel`
- `LosePanelViewModel`

---

## 5. UI flow

### Scene transition flow

```text
SCN_MainMenu
  ChooseMap Button
    -> ISceneFlowService.LoadChoosingMap()
    -> SCN_ChoosingMap

SCN_ChoosingMap
  Return Button
    -> ISceneFlowService.LoadMainMenu()
    -> SCN_MainMenu

SCN_ChoosingMap
  Play Button
    -> ISceneFlowService.LoadGameplay(selectedMap)
    -> SCN_Gameplay
```

### Overlay panel flow

```text
Settings Button -> IPanelService.Open(SettingsPanel)
Shop Button -> IPanelService.Open(ShopPanel)
Profile Button -> IPanelService.Open(ProfilePanel)
Pause Button -> IPanelService.Open(PausePanel)
Back Button -> IPanelService.CloseTop() or Close(panel)
```

Panel overlay khong unload scene goc. Scene gameplay khi mo Pause/Win/Lose/Revive co the thay doi `Time.timeScale` hoac pause service.

---

## 6. Async scene loading

Tat ca scene transition phai di qua `ISceneFlowService` va tra ve `UniTask`.

```text
Button click
  -> ViewModel command
  -> ISceneFlowService.LoadXAsync()
  -> LoadingOverlay shows progress
  -> SceneManager.LoadSceneAsync(...)
  -> Scene bootstrap binds scene UI
  -> LoadingOverlay closes
```

Rules:

- Khong goi `SceneManager.LoadScene` truc tiep trong View/Button.
- Khong goi load scene neu `IsLoading == true`.
- `LoadingProgress` expose reactive float 0..1 de bind slider/fill.
- Scene gameplay load xong moi bootstrap selected map/player/waves.
- Moi async load nhan `CancellationToken` de cancel khi root object bi destroy.

---

## 7. View rules

View duoc lam:

- Serialize `Button`, `Slider`, `Image`, `TMP_Text`, `Animator`.
- Subscribe vao ViewModel reactive properties.
- Forward button click/input vao ViewModel command.
- Play visual effect cuc bo neu ViewModel da bao state.

View khong duoc lam:

- Goi truc tiep `SceneManager.LoadScene`.
- Goi truc tiep `GameManager`, `PlayerInfo.instance`, `WeaponManager.Instance`.
- Tru tien, cong reward, equip item, revive player truc tiep.
- Chua validation mua ban/equipment.

---

## 8. ViewModel rules

ViewModel duoc lam:

- Expose `ReadOnlyReactiveProperty<T>` cho UI.
- Chua command: `OpenSettings`, `Play`, `Resume`, `TryAgain`, `ReviveByAds`.
- Goi service interface.
- Combine state thanh UI display string/percent/enabled.

ViewModel khong nen:

- Reference Unity UI component.
- Instantiate prefab.
- Query scene object truc tiep.

---

## 9. Migration strategy

Project hien da co gameplay prototype. Nen migration theo huong adapter:

```text
New UI ViewModel -> Interface Service -> Legacy Adapter -> Existing MonoBehaviour
```

Vi du:

- `ChoosingMapViewModel.Play()` goi `ILevelSelectionService.PlaySelectedMap()`.
- `LegacyLevelSelectionService` tam thoi goi code tu `LevelLoader`/`Spawner`.
- Sau nay tach `LevelLoader` thanh service that ma UI khong can doi.
