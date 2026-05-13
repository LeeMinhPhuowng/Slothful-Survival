# UI MVVM Implementation Roadmap

File nay la huong dan code theo flow. Moi class duoc nhac den deu co mo ta ngan gon: no lam gi, nhan input nao, output nao, va no khong nen lam gi.

---

## Phase 0 - Scene va prefab target

Muc tieu: tao skeleton trong Unity truoc khi code logic.

### Can tao trong Unity

| Asset/Object | Nhiem vu |
| --- | --- |
| `SCN_MainMenu` | Scene main menu, chi chua UI/background main menu. |
| `SCN_ChoosingMap` | Scene chon map, chua UI map selection. |
| `SCN_Gameplay` | Scene gameplay runtime, chua HUD va gameplay systems. |
| `UIRoot_MainMenu` | Root object gan binder cho main menu UI. |
| `UIRoot_ChoosingMap` | Root object gan binder cho choosing map UI. |
| `UIRoot_Gameplay` | Root object gan binder cho gameplay HUD/panels. |
| `PanelHost` | Transform/container de spawn hoac show/hide overlay panel. |
| `SettingsPanel` prefab | Popup setting dung chung. |
| `ShopPanel` prefab | Popup shop dung chung. |
| `ProfilePanel` prefab | Popup inventory dung chung. |
| `AugmentPanel` prefab | Popup chon augment trong gameplay. |
| `PausePanel` prefab | Popup pause trong gameplay. |
| `WinPanel` prefab | Popup result khi win. |
| `RevivePanel` prefab | Popup revive khi chet. |
| `LosePanel` prefab | Popup result khi lose. |

Acceptance:

- Scene flow target da ro rang.
- Moi scene co `PanelHost`.
- Moi panel co prefab placeholder rieng, co Back/Close button neu can.

---

## Phase 1 - UI Core Flow

Muc tieu: tao core de moi scene/panel deu chay theo cung mot pattern.

### Data da co

| Class | Nhiem vu |
| --- | --- |
| `SceneId` | Enum dinh danh scene logic: MainMenu, ChoosingMap, Gameplay. |
| `PanelId` | Enum dinh danh overlay panel: Settings, Shop, Profile, Pause, Win, Revive, Lose, Augment. |
| `GameplayLoadRequest` | Struct request load gameplay gom `MapId`, `CharacterId`, `IsRetry`. |
| `SceneLoadResult` | Struct ket qua load scene: success/fail + reason/message. |

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `SceneFlowService` | Implement `ISceneFlowService`, load scene bat dong bo bang UniTask va `SceneManager.LoadSceneAsync`. |
| `PanelService` | Implement `IPanelService`, quan ly panel hien tai/stack panel va open/close overlay. |
| `LoadingOverlayService` | Implement `ILoadingOverlayService`, expose `IsVisible`, `Progress`, dieu khien loading overlay. |
| `PanelRegistry` | Map `PanelId` -> prefab/view instance. Co the la MonoBehaviour serialize list trong scene. |
| `PanelHostView` | MonoBehaviour nam tren `PanelHost`, co nhiem vu show/hide hoac instantiate panel prefab. |
| `UIInstaller` | Reflex root installer: dang ky `SceneNameRegistry`, `GameCatalogRegistry`, global loading va global UI services. |
| Scene installer | `MainMenuInstaller`, `ChoosingMapInstaller`, ... dang ky `PanelHostView`, `PanelService` va ViewModel cua scene hien tai. |
| `GlobalUIRootBinder` | MonoBehaviour nhan service qua `[Inject]`, reset state UI luc scene bat dau va dung de debug dependency da resolve. |

### Service behavior

`SceneFlowService`:

- Input: method `LoadMainMenuAsync`, `LoadChoosingMapAsync`, `LoadGameplayAsync(request)`.
- Output: `CurrentScene`, `IsLoading`, `LoadingProgress`.
- Khong lam: khong bind UI button, khong spawn player/map truc tiep.
- Ghi chu: gameplay bootstrap se doc `GameplayLoadRequest` tu session service sau khi scene load.

`PanelService`:

- Input: `Open(PanelId)`, `Close(PanelId)`, `CloseTop()`, `CloseAll()`.
- Output: `CurrentPanel`.
- Khong lam: khong chua logic cua Settings/Shop/Inventory.
- Ghi chu: neu panel can pause game, service goi `IGameRunService.Pause()` thong qua rule table.

`LoadingOverlayService`:

- Input: `Show()`, `SetProgress(float)`, `Hide()`.
- Output: `IsVisible`, `Progress`.
- Khong lam: khong tu load scene.

### Flow

```text
ViewModel command
  -> SceneFlowService.LoadXAsync()
  -> LoadingOverlayService.Show()
  -> LoadingProgress update
  -> Scene loaded
  -> LoadingOverlayService.Hide()
```

Acceptance:

- Button chuyen scene khong goi `SceneManager` truc tiep.
- Dang loading thi button load scene khong duoc trigger lan 2.
- Loading overlay nhan duoc progress.

---

## Phase 2 - Catalog va ID backend

Muc tieu: co noi resolve ID sang config/model de UI khong phu thuoc index list.

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `GameCatalogSO` | ScriptableObject chua list character/map/equipment/shop config. |
| `GameCatalog` | Implement `IGameCatalog`, lookup config/model theo stable ID. |
| `CharacterModelFactory` | Tao `CharacterModel` tu `CharacterInfoSO` + save/unlock state. |
| `MapModelFactory` | Tao `MapModel` tu `LevelSO` + progress/unlock state. |
| `EquipmentItemModelFactory` | Tao `EquipmentItemModel` tu equipment config/save state. |

### Config can bo sung dan

| Config | Field can co |
| --- | --- |
| `CharacterInfoSO` | `characterId`, `displayName`, `portrait/preview`. |
| `LevelSO` | `mapId`, `displayName`, `previewImage`, `isUnlockedDefault`. |
| `EquipmentItemSO` | `itemId`, `displayName`, `icon`, `slot`, stat modifiers. |
| `ShopOfferSO` | `offerId`, `displayName`, price, reward. |

### Why ID

- Save data chi luu ID.
- UI command gui ID, service resolve model.
- Khong dung index vi doi thu tu asset se hong save.

Acceptance:

- `GameCatalog.GetCharacter("char_knight")` tra ve config/model dung.
- ID duplicate duoc detect trong editor/log.

---

## Phase 3 - SCN_MainMenu flow

Muc tieu: main menu dung MVVM va mo shared panels.

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `MainMenuSceneView` | MonoBehaviour bind `SettingsButton`, `ShopButton`, `ProfileButton`, `ChooseMapButton`. |
| `MainMenuViewModel` | Chua commands: `OpenSettings`, `OpenShop`, `OpenProfile`, `ChooseMapAsync`. |
| `MainMenuInstaller` | Reflex installer dang ky `MainMenuViewModel` vao scene container. |
| `MainMenuBinder` | Nhan `MainMenuViewModel` bang `[Inject]`, bind vao `MainMenuSceneView` trong `Start`. |
| `DynamicBackgroundPanelView` | Quan ly visual background dong: co cay, lua, anh sang. |

### Class responsibilities

`MainMenuSceneView`:

- Serialize Unity UI references.
- Subscribe button click va forward sang ViewModel.
- Khong load scene, khong open panel truc tiep neu da co ViewModel.

`MainMenuViewModel`:

- Goi `IPanelService.Open(PanelId.Settings/Shop/Profile)`.
- Goi `ISceneFlowService.LoadChoosingMapAsync()`.
- Expose `IsLoading` neu can disable `ChooseMapButton`.

`DynamicBackgroundPanelView`:

- Chay animation/particle/VFX.
- Khong can ViewModel neu chi la visual lap.

### Flow

```text
ChooseMapButton
  -> MainMenuViewModel.ChooseMapAsync()
  -> SceneFlowService.LoadChoosingMapAsync()
```

```text
SettingsButton
  -> MainMenuViewModel.OpenSettings()
  -> PanelService.Open(Settings)
```

Acceptance:

- Main menu khong con goi `GameManager.OnMainMenuPlayButtonClicked`.
- Settings/Shop/Inventory mo overlay tren main menu.
- ChooseMap load sang `SCN_ChoosingMap` async.

---

## Phase 4 - Shared Panel: SettingsPanel

Muc tieu: settings panel dung chung trong MainMenu, ChoosingMap, Gameplay/Pause.

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `SettingsPanelView` | Bind `MusicVolumeSlider`, `SfxVolumeSlider`, `FacebookButton`, `BackButton`. |
| `SettingsPanelViewModel` | Expose volume state va commands set volume/open facebook/back. |
| `SettingsService` | Implement `ISettingsService`, luu volume runtime va save/load. |
| `AudioService` | Apply music/sfx volume vao AudioMixer/AudioSource. |
| `ExternalLinkService` | Mo Facebook URL bang `Application.OpenURL`. |

### Responsibilities

`SettingsPanelViewModel`:

- Input: slider value tu View.
- Output: `MusicVolume`, `SfxVolume`.
- Goi `ISettingsService.SetMusicVolume`, `SetSfxVolume`.
- Goi `IExternalLinkService.OpenFacebook`.
- Goi `IPanelService.Close(PanelId.Settings)` khi Back.

`SettingsService`:

- Giu `ReactiveProperty<float>` cho music/sfx.
- Clamp value 0..1.
- Save/load vao PlayerPrefs hoac save service.
- Khong biet UI slider nao.

Acceptance:

- Slider doi value thi nhac/sfx thay doi.
- Back chi dong panel.
- SettingsPanel dung lai duoc o MainMenu va ChoosingMap.

---

## Phase 5 - Shared Panel: ShopPanel

Muc tieu: shop hien currency va mua offer bang real money/gold/diamond.

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `ShopPanelView` | Bind gold/diamond text, list real-money offer, list currency offer, Back button. |
| `ShopPanelViewModel` | Format balance, expose offer list, command buy offer. |
| `WalletService` | Implement `IWalletService`, quan ly Gold/Diamond reactive. |
| `ShopService` | Implement `IShopService`, validate offer, spend currency, grant reward. |
| `CashPurchaseService` | Implement `ICashPurchaseService`, fake/editor purchase truoc khi gan SDK. |
| `ShopOfferItemView` | View cho tung item trong list shop. |
| `ShopOfferItemViewModel` | State/command cua mot offer item. |

### Responsibilities

`ShopPanelViewModel`:

- Bind `GoldText`, `DiamondText`.
- Goi `IShopService.BuyCurrencyOffer(offerId)` hoac `BuyRealMoneyOffer(offerId)`.
- Xu ly `PurchaseResult` de hien fail/success feedback.
- Khong tu tru tien, khong tu add item.

`ShopService`:

- Resolve `ShopOfferModel` theo `offerId`.
- Neu offer bang gold/diamond: goi `WalletService.TrySpend`.
- Neu thanh cong: grant reward qua inventory/wallet.
- Tra `PurchaseResult`.

Acceptance:

- Gold/Diamond text auto update.
- Mua bang gold/diamond validate du tien.
- Mua real-money dung fake service trong editor.

---

## Phase 6 - Shared Panel: ProfilePanel

Muc tieu: chon nhan vat, xem/equip 6 slot trang bi va filter inventory.

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `ProfilePanelView` | Bind model preview, left/right buttons, 6 equipment slots, tab bar, item grid, Back. |
| `ProfilePanelViewModel` | Quan ly selected character, selected tab, filtered items, equip/unequip commands. |
| `CharacterRosterService` | Implement `ICharacterRosterService`, quan ly list character va selected character. |
| `InventoryService` | Implement `IInventoryService`, quan ly owned items, equipped items, filter tab. |
| `EquipmentSlotView` | View cua mot slot trang bi. |
| `EquipmentSlotViewModel` | Hien item dang equip trong slot va command unequip/select. |
| `InventoryItemView` | View cua mot item trong kho. |
| `InventoryItemViewModel` | Hien icon/name/state cua item va command select/equip. |
| `CharacterPreviewView` | Hien model/animator preview nhan vat. |

### Responsibilities

`ProfilePanelViewModel`:

- Goi `CharacterRosterService.SelectPrevious/SelectNext`.
- Goi `InventoryService.SelectTab`, `Equip`, `Unequip`.
- Expose `FilteredItems`, `EquippedItems`, `SelectedCharacter`.
- Khong spawn player gameplay.

`InventoryService`:

- Luu danh sach owned item.
- Luu dictionary `EquipmentSlot -> EquipmentItemModel`.
- Tinh filter theo `EquipmentTab`.
- Validate slot dung loai item.

`CharacterRosterService`:

- Doc character tu `IGameCatalog`.
- Quan ly selected character cho UI/session.
- Khong instantiate prefab gameplay.

Acceptance:

- Nhan nut trai/phai doi preview nhan vat.
- 6 slot hien dung equipment dang equip.
- Tab filter item dung loai.
- Back chi dong panel.

---

## Phase 7 - SCN_ChoosingMap flow

Muc tieu: chon map va load gameplay voi selected map.

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `ChoosingMapSceneView` | Bind Return, Settings, Shop, Play va map list/preview. |
| `ChoosingMapViewModel` | Quan ly selected map, open panels, return main menu, play selected map. |
| `MapSelectionService` | Implement `IMapSelectionService`, expose maps va selected map. |
| `MapItemView` | View tung map trong list/carousel. |
| `MapItemViewModel` | Hien map name/preview/lock state va command select. |
| `GameSessionService` | Luu selected map/character cho lan load gameplay tiep theo. |

### Responsibilities

`ChoosingMapViewModel`:

- `ReturnToMainMenuAsync()` -> `ISceneFlowService.LoadMainMenuAsync()`.
- `OpenSettings()` -> `IPanelService.Open(Settings)`.
- `OpenShop()` -> `IPanelService.Open(Shop)`.
- `PlayAsync()` -> tao `GameplayLoadRequest` va goi `LoadGameplayAsync`.

`MapSelectionService`:

- Doc maps tu `IGameCatalog`.
- Expose `SelectedMap`.
- Validate map unlocked.

`GameSessionService`:

- Luu `SelectedMapId`, `SelectedCharacterId`.
- Cung cap request cho gameplay bootstrap.

Acceptance:

- Return ve MainMenu async.
- Play khong chay neu chua co selected/unlocked map.
- Settings/Shop la overlay tren ChoosingMap.

---

## Phase 8 - SCN_Gameplay HUD

Muc tieu: HUD gameplay dung reactive state va command.

### Class nen tao

| Class | Nhiem vu |
| --- | --- |
| `GameplaySceneView` | Bind Pause, Energy/EXP slider, Inventory, Attack button, joystick. |
| `GameplayHudViewModel` | Expose `ExpPercent`, `CanAttack`, attack cooldown, commands pause/inventory/attack. |
| `PlayerProgressService` | Implement `IPlayerProgressService`, bridge PlayerEXP/level state sang reactive. |
| `CombatInputService` | Implement `ICombatInputService`, request normal attack va cooldown state. |
| `JoystickMovementInput` | Implement `IPlayerMovementInput`, expose move vector tu `FloatingJoystick`. |
| `GameplayBootstrap` | Sau khi scene load, spawn map/player/waves dua tren `GameSessionService`. |

### Responsibilities

`GameplayHudViewModel`:

- `OpenPause()` -> `IPanelService.Open(Pause)`.
- `OpenProfile()` -> `IPanelService.Open(Profile)`.
- `Attack()` -> `ICombatInputService.RequestNormalAttack()`.
- Bind `ExpPercent` tu `IPlayerProgressService`.

`GameplayBootstrap`:

- Doc selected map/character.
- Goi legacy adapter/player spawn/map spawn trong giai doan dau.
- Khong chua UI button logic.

Acceptance:

- EXP slider update khi PlayerEXP doi.
- Attack button trigger normal attack service.
- Pause/profile mo overlay.
- Joystick van dieu khien player.

---

## Phase 9 - Gameplay Panels

### PausePanel

| Class | Nhiem vu |
| --- | --- |
| `PausePanelView` | Bind ChoosingMap, Resume, TryAgain, music/sfx sliders. |
| `PausePanelViewModel` | Command resume/retry/choosing map va volume settings. |

Flow:

```text
PauseButton -> PanelService.Open(Pause) -> GameRunService.Pause()
Resume -> GameRunService.Resume() -> PanelService.Close(Pause)
TryAgain -> SceneFlowService.ReloadGameplayAsync()
ChoosingMap -> SceneFlowService.LoadChoosingMapAsync()
```

### AugmentPanel

| Class | Nhiem vu |
| --- | --- |
| `AugmentPanelView` | Bind 3 augment options: frame, image, description. |
| `AugmentPanelViewModel` | Expose options va command select augment. |
| `AugmentRollService` | Roll 3 augment options. |
| `AugmentApplyService` | Apply selected augment vao player/weapon/stat. |
| `AugmentOptionView` | View tung option. |
| `AugmentOptionViewModel` | Hien data cua mot option va command select. |

Flow:

```text
PlayerLeveledUpEvent/ChannelSO
  -> AugmentRollService.Roll()
  -> PanelService.Open(Augment)
  -> Select option
  -> AugmentApplyService.Apply()
  -> PanelService.Close(Augment)
```

### RevivePanel

| Class | Nhiem vu |
| --- | --- |
| `RevivePanelView` | Bind timer slider/text, Ads button, Diamond button. |
| `RevivePanelViewModel` | Expose timer/can revive va commands revive. |
| `ReviveService` | Implement `IReviveService`, quan ly timer va revive result. |

Flow:

```text
PlayerDiedEvent/ChannelSO
  -> GameRunService.Pause()
  -> ReviveService.StartTimer()
  -> PanelService.Open(Revive)
  -> Ads/Diamond revive
  -> Success: close panel, resume
  -> Fail/timeout: open LosePanel
```

### WinPanel / LosePanel

| Class | Nhiem vu |
| --- | --- |
| `WinPanelView` | Bind gold, enemy defeat, playing time, ChoosingMap, NextLevel. |
| `WinPanelViewModel` | Expose run result va commands choosing map/next level. |
| `LosePanelView` | Bind gold, enemy defeat, playing time, ChoosingMap, TryAgain. |
| `LosePanelViewModel` | Expose run result va commands choosing map/retry. |
| `GameRunService` | Luu `RunResultModel`, pause/end run, retry/return map. |

Acceptance:

- Pause/Win/Lose/Revive khong xu ly logic truc tiep trong View.
- Win/Lose hien dung `RunResultModel`.
- Revive timeout di toi LosePanel.

---

## Phase 10 - Legacy adapters

Muc tieu: boc code gameplay hien tai de UI moi chay duoc ma chua rewrite gameplay.

| Adapter | Boc code hien tai | Nhiem vu |
| --- | --- | --- |
| `LegacySceneFlowService` | `SceneManager`, old `GameManager` flow neu can | Load scene trong giai doan dau. |
| `LegacyCharacterRosterService` | `PlayerSetter`, `CharacterInfoSO` list | Chon character va feed selected character. |
| `LegacyMapSelectionService` | `LevelLoader`, `CoverFlow`, `LevelSO` | Chon map va feed selected map. |
| `LegacyPlayerProgressService` | `PlayerEXP`, `PlayerLevelManager` | Expose EXP percent/current level reactive. |
| `LegacyCombatInputService` | `WeaponManager`, weapon behaviours | Request normal attack/cooldown. |
| `LegacyAugmentService` | `AugmentManager`, `Augment` | Bridge level-up augment UI. |
| `LegacyRunService` | death/win/lose prototype logic | Pause/resume/retry/result. |

Rule:

- Adapter duoc phep biet singleton/prototype.
- ViewModel khong duoc biet singleton/prototype.

Acceptance:

- UI moi chay tren gameplay cu.
- Sau nay thay adapter bang service that ma ViewModel khong can sua.

---

## Suggested implementation order

1. Tao scenes, roots, panel prefabs.
2. Implement `SceneFlowService`, `PanelService`, `LoadingOverlayService`, `PanelHostView`.
3. Implement `GameCatalogSO` + `GameCatalog`.
4. Implement `SCN_MainMenu`.
5. Implement `SettingsPanel`.
6. Implement `SCN_ChoosingMap`.
7. Implement `SCN_Gameplay` HUD.
8. Implement `PausePanel`.
9. Implement `ProfilePanel`.
10. Implement `ShopPanel`.
11. Implement `AugmentPanel`.
12. Implement `RevivePanel`, `WinPanel`, `LosePanel`.
13. Thay dan old `GameManager` UI flow bang scene/panel service flow.

Thu tu nay cho phep code UI theo tung lop, moi buoc co the test duoc rieng va khong can rewrite gameplay mot lan.
