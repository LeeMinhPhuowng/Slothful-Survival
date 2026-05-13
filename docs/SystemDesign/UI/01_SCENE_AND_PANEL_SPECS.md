# Scene And Panel Specifications

---

## 1. SCN_MainMenu

### UI components

- `BackgroundPanel`: dynamic background co co cay, lua, anh sang.
- `SettingsButton`: mo `SettingsPanel`.
- `ShopButton`: mo `ShopPanel`.
- `ProfileButton`: mo `ProfilePanel`.
- `ChooseMapButton`: chuyen sang `SCN_ChoosingMap`.

### View

`MainMenuSceneView`

- Bind 4 button chinh.
- Bind dynamic background view neu can start/stop animation.
- Khong xu ly scene loading truc tiep.

### ViewModel

`MainMenuViewModel`

| Command | Service call |
| --- | --- |
| `OpenSettings()` | `IPanelService.Open(PanelId.Settings)` |
| `OpenShop()` | `IPanelService.Open(PanelId.Shop)` |
| `OpenProfile()` | `IPanelService.Open(PanelId.Profile)` |
| `ChooseMap()` | `ISceneFlowService.LoadChoosingMap()` |

### Background Panel dong

`DynamicBackgroundPanelView`

- Co the dung particle/VFX/Animator cho co cay, lua, anh sang.
- Nen la visual-only view, khong co ViewModel rieng neu chi animation lap.
- Neu co setting reduce motion, bind `SettingsService.IsBackgroundMotionEnabled`.

---

## 2. SCN_ChoosingMap

### UI components

- `ReturnButton`: tro ve `SCN_MainMenu`.
- `SettingsButton`: mo `SettingsPanel`.
- `ShopButton`: mo `ShopPanel`.
- `PlayButton`: chuyen sang `SCN_Gameplay`.
- Map list/preview: duoc them vao spec vi scene nay can chon map truoc khi Play.

### ViewModel

`ChoosingMapViewModel`

| State/Command | Type | Ghi chu |
| --- | --- | --- |
| `Maps` | reactive list | Danh sach `LevelSO`/map runtime |
| `SelectedMap` | reactive | Map dang chon |
| `CanPlay` | reactive bool | True khi co selected map va unlock |
| `ReturnToMainMenu()` | command | Load `SCN_MainMenu` |
| `OpenSettings()` | command | Mo `SettingsPanel` |
| `OpenShop()` | command | Mo `ShopPanel` |
| `SelectMap(mapId)` | command | Chon map |
| `Play()` | command | Load `SCN_Gameplay` voi selected map |

### Integration

`Play()` can:

- Luu selected `LevelSO` vao `IGameSessionService`.
- Chuyen scene sang `SCN_Gameplay`.
- Khi `SCN_Gameplay` load, gameplay bootstrap spawn map/waves theo selected map.

---

## 3. SCN_Gameplay

### UI components

- `PauseButton`: mo `PausePanel`.
- `EnergySlider`: cap nhat thanh kinh nghiem cua Player.
- `ProfileButton`: mo `ProfilePanel`.
- `MovementJoystick`: dieu khien nhan vat di chuyen.
- `PlayerAttackButton`: tan cong danh thuong.

### ViewModel

`GameplayHudViewModel`

| State/Command | Type | Ghi chu |
| --- | --- | --- |
| `ExpPercent` | reactive float | Bind vao `EnergySlider`; ten UI co the giu la EnergySlider nhung domain nen la EXP |
| `CanOpenInventory` | reactive bool | False khi dang revive/win/lose |
| `CanAttack` | reactive bool | False khi cooldown/pause/dead |
| `AttackCooldownPercent` | reactive float | Bind radial fill neu co |
| `OpenPause()` | command | Mo `PausePanel` va pause game |
| `OpenProfile()` | command | Mo `ProfilePanel` |
| `Attack()` | command | Goi `ICombatInputService.RequestNormalAttack()` |

### Movement Joystick

Movement joystick la input view dac biet:

- Co the tiep tuc dung `FloatingJoystick` hien tai trong giai doan dau.
- Sau refactor nen co `IPlayerMovementInput` de gameplay doc vector input.
- ViewModel khong can xu ly moi frame movement neu joystick da la component input.

### Player Attack Button

`AttackButtonView`

- Button click -> `GameplayHudViewModel.Attack()`.
- Disabled state bind `CanAttack`.
- Optional cooldown fill bind `AttackCooldownPercent`.

---

## 4. SettingsPanel

### UI components

- `MusicVolumeImage`.
- `MusicVolumeSlider`: tang/giam nhac.
- `SfxVolumeImage`.
- `SfxVolumeSlider`: tang/giam am thanh hieu ung.
- `FacebookButton`: mo trang Facebook.
- `BackButton`: tat panel.

### ViewModel

`SettingsPanelViewModel`

| State/Command | Type |
| --- | --- |
| `MusicVolume` | reactive float 0..1 |
| `SfxVolume` | reactive float 0..1 |
| `SetMusicVolume(value)` | command |
| `SetSfxVolume(value)` | command |
| `OpenFacebook()` | command |
| `Back()` | command |

### Service

- `ISettingsService`: save/load volume.
- `IAudioService`: apply music/sfx volume.
- `IExternalLinkService`: open Facebook URL.

---

## 5. ShopPanel

### UI components

- `GoldImage` + `GoldText`: hien thi vang hien co.
- `DiamondImage` + `DiamondText`: hien thi kim cuong hien co.
- Real-money transaction list: mua gold, mua diamond.
- In-game currency transaction list: mua trang bi nhan vat bang gold/diamond.
- `BackButton`: tat panel.

### ViewModel

`ShopPanelViewModel`

| State/Command | Type |
| --- | --- |
| `GoldText` | reactive string |
| `DiamondText` | reactive string |
| `RealMoneyOffers` | reactive list |
| `CurrencyOffers` | reactive list |
| `IsBusy` | reactive bool |
| `BuyRealMoneyOffer(offerId)` | command |
| `BuyCurrencyOffer(offerId)` | command |
| `Back()` | command |

### Rules

- Real-money purchase chi grant reward sau khi `ICashShopService` confirm success.
- In-game purchase phai qua `IWalletService.TrySpend`.
- Trang bi mua duoc grant vao `IInventoryService`.

---

## 6. ProfilePanel

### UI components

- Character model/preview.
- Left character button.
- Right character button.
- 6 equipment slots:
  - Armor body / giap.
  - Helmet / giap mu.
  - Boots / giap chan.
  - Gloves / giap tay.
  - Pants / quan.
  - Weapon / vu khi.
- Inventory tab bar:
  - All.
  - Armor body.
  - Helmet.
  - Boots.
  - Gloves.
  - Pants.
  - Weapon.
- Equipment item grid/list.
- `BackButton`: tat panel.

### ViewModel

`ProfilePanelViewModel`

| State/Command | Type |
| --- | --- |
| `SelectedCharacter` | reactive character |
| `CharacterPreviewState` | reactive preview data |
| `EquipmentSlots` | reactive 6 slot view models |
| `SelectedTab` | reactive equipment tab |
| `FilteredItems` | reactive list |
| `SelectedItem` | reactive item |
| `CanEquipSelected` | reactive bool |
| `PreviousCharacter()` | command |
| `NextCharacter()` | command |
| `SelectTab(tab)` | command |
| `SelectItem(itemId)` | command |
| `EquipSelected()` | command |
| `Unequip(slot)` | command |
| `Back()` | command |

### Equipment slots

```text
Left column:  ArmorBody, Helmet, Gloves
Right column: Boots, Pants, Weapon
```

Cot trai/phai co the doi trong layout, nhung data slot phai on dinh.

---

## 7. AugmentPanel

### UI components

Moi augment option gom:

- Frame: bac/vang/tim.
- Image: tang chi so hoac them vu khi.
- Description: mo ta nang cap.

### ViewModel

`AugmentPanelViewModel`

| State/Command | Type |
| --- | --- |
| `Options` | reactive list 3 augment option |
| `SelectAugment(optionId)` | command |

### Integration

`AugmentManager` hien tai dang random va apply augment truc tiep. Refactor nen tach:

- `IAugmentRollService`: roll options.
- `IAugmentApplyService`: apply selected augment.
- Panel chi hien options va forward select.

---

## 8. PausePanel

### UI components

- `ChoosingMapButton`: quay ve `SCN_ChoosingMap`.
- `ResumeButton`: tiep tuc choi.
- `TryAgainButton`: load lai man choi.
- `MusicVolumeImage` + `MusicVolumeSlider`.
- `SfxVolumeImage` + `SfxVolumeSlider`.

### ViewModel

`PausePanelViewModel`

| Command/State | Ghi chu |
| --- | --- |
| `MusicVolume`, `SfxVolume` | Dung chung settings service |
| `ReturnToChoosingMap()` | Load `SCN_ChoosingMap` |
| `Resume()` | Close panel + unpause |
| `TryAgain()` | Reload current gameplay |
| `SetMusicVolume(value)` | Apply + save |
| `SetSfxVolume(value)` | Apply + save |

---

## 9. WinPanel

### UI components

- `ChoosingMapButton`: quay ve `SCN_ChoosingMap`.
- `NextLevelButton`: choi man tiep theo.
- `GoldImage` + `GoldText`: vang kiem duoc trong tran.
- `EnemyDefeatImage` + `EnemyDefeatText`: tong so quai chet.
- `PlayingTimeImage` + `PlayingTimeText`: thoi gian hoan thanh.

### ViewModel

`WinPanelViewModel`

| State/Command | Type |
| --- | --- |
| `GoldEarnedText` | reactive string |
| `EnemyDefeatedText` | reactive string |
| `PlayingTimeText` | reactive string |
| `ReturnToChoosingMap()` | command |
| `NextLevel()` | command |

### Integration

Win result lay tu `IRunResultService` hoac `IGameSessionService`.

---

## 10. RevivePanel

### UI components

- `TimerSlider` + `TimerText`: so giay con lai.
- `AdsButton`: xem video de revive.
- `DiamondButton`: tieu kim cuong de revive.

### ViewModel

`RevivePanelViewModel`

| State/Command | Type |
| --- | --- |
| `RemainingSeconds` | reactive int |
| `TimerPercent` | reactive float |
| `CanReviveByAds` | reactive bool |
| `CanReviveByDiamond` | reactive bool |
| `ReviveByAds()` | command |
| `ReviveByDiamond()` | command |

### Rules

- Het timer ma khong revive -> mo `LosePanel`.
- Ads revive chi thanh cong khi ad service confirm rewarded success.
- Diamond revive phai spend diamond thanh cong.

---

## 11. LosePanel

### UI components

- `GoldImage` + `GoldText`: vang kiem duoc trong tran.
- `EnemyDefeatImage` + `EnemyDefeatText`: tong so quai chet.
- `PlayingTimeImage` + `PlayingTimeText`: thoi gian thuc hien man choi.
- `ChoosingMapButton`: quay ve `SCN_ChoosingMap`.
- `TryAgainButton`: load lai man choi.

### ViewModel

`LosePanelViewModel`

| State/Command | Type |
| --- | --- |
| `GoldEarnedText` | reactive string |
| `EnemyDefeatedText` | reactive string |
| `PlayingTimeText` | reactive string |
| `ReturnToChoosingMap()` | command |
| `TryAgain()` | command |

