# Slothful Survival

Slothful Survival la game 2D pixel bullet-hell survival tren Unity. Nguoi choi di qua flow chinh:

```text
SCN_MainMenu --ChooseMap--> SCN_ChoosingMap --Play--> SCN_Gameplay
```

UI moi can refactor theo mo hinh scene + overlay panel, dung MVVM voi R3/Reactive va service/facade de tach UI khoi gameplay manager hien tai.
Scene transition nen load bat dong bo bang UniTask (`com.cysharp.unitask`) de co loading progress, loading overlay va khong freeze frame khi chuyen scene.

## UI source of truth

Tai lieu UI moi nam tai:

- `docs/SystemDesign/UI/00_UI_MVVM_OVERVIEW.md`
- `docs/SystemDesign/UI/01_SCENE_AND_PANEL_SPECS.md`
- `docs/SystemDesign/UI/02_DATA_EVENTS_AND_SERVICES.md`
- `docs/SystemDesign/UI/03_IMPLEMENTATION_ROADMAP.md`
- `docs/SystemDesign/UI/04_UI_BACKEND_MODEL_AND_EVENT_STRATEGY.md`

Nhung file nay la tai lieu can doc dau tien khi lam UI. Khong can duyet ca project moi nam duoc flow.

## Scene UI chinh

- `SCN_MainMenu`: background panel dong, Settings, Shop, Profile, ChooseMap.
- `SCN_ChoosingMap`: Return, Settings, Shop, Play.
- `SCN_Gameplay`: Pause, Energy/EXP slider, Profile, Movement Joystick, Player Attack Button.

## Overlay panels

Overlay/popup panel co the mo tren scene goc:

- `SettingsPanel`
- `ShopPanel`
- `ProfilePanel`
- `ItemDetailPanel`
- `ConfirmationPanel`
- `AugmentPanel`
- `PausePanel`
- `WinPanel`
- `RevivePanel`
- `LosePanel`

## Profile / Player Inventory Flow

`ProfilePanel` la PlayerInventory. Data item khong hard-code trong service. Moi item trang bi duoc tao bang `EquipmentItemSO`:

- `ItemId`, `DisplayName`, `Description`, `RarityItem`, `Icon`
- `Slot`: `Armor`, `Helmet`, `Boots`, `Gloves`, `Pants`, `Weapon`
- `Level`, `BuyPrice`, `SellPrice`
- `IsUnlocked`
- combat stats: `Armor`, `Damage`, `MaxHealth`, `MoveSpeed`

Gan cac `EquipmentItemSO` vao `EquipmentCatalogRegistry`, sau do assign `EquipmentCatalogRegistry` vao `UIInstaller`. `InventoryService` nhan catalog qua DI va giu runtime state:

- `CatalogItems`: tat ca item, gom ca locked va unlocked.
- `FilteredItems`: item theo tab hien tai, van gom item locked.
- `EquippedItems`: item dang lap theo slot.
- `SelectedTab`: tab hien tai, mac dinh `All` khi mo ProfilePanel.

Khi mo `ProfilePanel`:

```text
ProfilePanelView.Show()
-> ProfilePanelViewModel.PrepareOpen()
-> SelectedSlot = null
-> SelectedTab = All
-> FilteredItems = tat ca item, gom item locked
-> khong co item inventory nao duoc selected
```

Item trong inventory la prefab chung co `InventoryItemView`. Prefab nay nen co:

- `Button`
- `Icon Image`
- optional: name, level, rarity text
- `EquippedState`
- `LockedState` overlay co icon khoa
- `RarityBackgroundImage` nam duoi `Icon Image`
- `RarityBackgrounds`: map `RarityItem -> Sprite background`

Inventory item khong co selected state. Khi click item:

```text
InventoryItemView.Button
-> ProfilePanelView.OnInventoryItemClicked(itemId)
-> ProfilePanelViewModel.OpenItemDetail(itemId)
-> selected slot = item.Slot
-> PanelService.Open(PanelId.ItemDetail)
```

`ItemDetailPanel` hien detail cua item dang duoc chon. Neu item unlocked:

```text
buttons: Equip/Unequip + Sell
```

Neu item locked:

```text
button: Buy
```

Buy/Sell khong tru/cong vang truc tiep. Luong transaction:

```text
Buy/Sell clicked
-> ConfirmationDialogService.Request(...)
-> PanelService.Open(PanelId.Confirmation)
-> Confirm: thuc thi transaction
-> Cancel/Close: huy, khong doi wallet/item state
```

Sau transaction thanh cong:

- Buy: `WalletService.TrySpend(Gold, BuyPrice)` -> `InventoryService.Unlock(itemId)`.
- Sell: `InventoryService.Lock(itemId)` -> unequip neu dang lap -> `WalletService.Add(Gold, SellPrice, "equipment_sell")`.
- Detail panel refresh lai theo state moi.

Compare stat chi hien khi:

- item detail dang co item,
- item do da unlocked,
- item do khong phai item dang equip trong slot,
- slot do dang co item khac de so sanh.

Ca `InventoryItemView` va `ItemDetailPanel` render icon theo 2 lop:

```text
Rarity background image
-> Item icon image nam de len tren
```

Background duoc resolve tu enum `RarityItem`, khong dung string rarity.

## Test MainMenu DI

1. Open `SCN_MainMenu`.
2. Create a global/root UI object and add Reflex `ContainerScope`.
3. Add `UIInstaller` on that root object, then register this object in `Assets/Resources/ReflexSettings.asset` -> `RootScopes`.
4. Assign `UIInstaller` references: `SceneNameRegistry`, `GameCatalogRegistry`, `LoadingPanelView`.
5. In `SCN_MainMenu`, create `SceneScope` and add Reflex `ContainerScope`.
6. Add `MainMenuInstaller` on the `SceneScope` or a child object, then assign its scene `PanelHostView`.
7. Add `MainMenuSceneView` to `UIRoot_MainMenu` or `MainMenuHud`, then assign `SettingsButton`, `ShopButton`, `ProfileButton`, `ChooseMapButton`.
8. Add `MainMenuBinder` to `UIRoot_MainMenu` and assign the `MainMenuSceneView`.
9. Add `GlobalUIRootBinder` to the shared UI root object.
10. In `SceneNameRegistry`, map `MainMenu -> SCN_MainMenu`, `ChoosingMap -> SCN_ChoosingMap`, `Gameplay -> SCN_Gameplay`.
11. In `EquipmentCatalogRegistry`, add all `EquipmentItemSO` assets.
12. In `PanelRegistry`, map temporary panels using `Game.UI.View.Demo.PlaceholderPanelView` or real panel views: `Settings`, `Shop`, `Profile`, `ItemDetail`, `Confirmation`.

## ChoosingMap -> Gameplay ID Flow

- Add `GameCatalogRegistry` to the scene UI root and assign it in `UIInstaller`.
- In `GameCatalogRegistry.Characters`, define stable `CharacterId` values such as `knight`, `archer`, `mage`, then assign each `CharacterInfoSO`.
- In `GameCatalogRegistry.Maps`, define stable `MapId` values such as `level_1`, `level_2`, then assign each `LevelSO`.
- In `ChoosingMapSceneView.mapButtons`, bind every map button to its matching `MapId`.
- `ChoosingMapViewModel` builds `GameplayLoadRequest(selectedMapId, selectedCharacterId)`.
- `SceneFlowService` validates IDs against `GameCatalogRegistry`, stores the selected `LevelSO` and `CharacterInfoSO` in `GameplayLaunchContext`, then loads `SCN_Gameplay`.
- Legacy `LevelLoader` and `PlayerSetter` read `GameplayLaunchContext` first, falling back to old `CoverFlow` / player index behavior when no UI request exists.

## Global Runtime + Save Progress

Global state khong nen song trong tung scene. Dat cac data/service dung xuyen scene vao mot global scope:

```text
GO_Global / Reflex RootScope / DontDestroyOnLoad
-> SceneNameRegistry
-> GameCatalogRegistry
-> EquipmentCatalogRegistry
-> LoadingPanelView
-> UIInstaller
-> GlobalRuntimeRoot neu object nay nam truc tiep trong scene
```

Scene scope chi nen giu UI cua scene hien tai:

```text
SceneScope
-> MainMenuInstaller / ChoosingMapInstaller / GameplayInstaller
-> PanelHostView
-> PanelService
-> ViewModel scene
```

`PlayerProgressService` la source of truth cho progress runtime va save file. No load/save file JSON bang `JsonProgressSaveService` tai `Application.persistentDataPath/player_progress.json`.

Progress dang save:

- Gold, Diamond
- equipment da unlock
- equipment dang equip theo slot
- map da unlock
- character da unlock
- selected map
- selected character
- level/exp placeholder

`WalletService`, `InventoryService`, `MapSelectionService`, `CharacterRosterService` da doc/ghi qua `IPlayerProgressService`, nen khi doi scene service co bi tao lai thi data van duoc restore tu save.

Trong `UIInstaller`/`GlobalInstaller` co 2 flag debug:

```text
resetProgressOnStart = true
-> xoa save cu va tao data moi tu catalog/default gold/diamond moi lan Play

saveOnEveryChange = true
-> moi lan mua/ban/equip/chon map/chon character/doi wallet se save ngay
```

Khi da chuyen sang global setup on dinh, chi giu `UIInstaller` o global root. Khong nen lap lai `UIInstaller` trong tung gameplay/menu scene, vi nhu vay cac singleton global se bi khai bao lai theo scene.

## Gameplay code hien tai can luu y

Core gameplay hien nam trong `Assets/Scripts`:

- `Manager/GameManager.cs`: flow prototype hien tai.
- `Manager/PlayerSetter.cs`: chon va spawn player.
- `Manager/LevelLoader.cs`: chon/spawn map va waves.
- `Manager/WeaponManager.cs`: quan ly weapon slots.
- `Manager/AugmentManager.cs`: hien augment khi level up.
- `Player/PlayerInfo.cs`: mau, stat, damage/death.
- `Player/PlayerMovement.cs`: joystick movement.
- `Augment/Augment.cs`: apply buff/add/upgrade.
- `SO/`: `CharacterInfoSO`, `WeaponInfoSO`, `LevelSO`, `EnemyWaveSO`, augment SO.

Hien project dung nhieu singleton/static nhu `PlayerInfo.instance`, `WeaponManager.Instance`, `AugmentManager.Instance`, `Spawner.Instance`. Khi refactor UI, View va ViewModel khong nen goi truc tiep cac singleton nay; hay di qua service/adapter de migration an toan.

## Nguyen tac UI refactor

- Scene canvas chi quan ly UI cua scene do va panel host.
- Panel la overlay, mo/dong qua `IPanelService`.
- View la MonoBehaviour passive: bind UI, cap nhat visual, forward click/input.
- ViewModel chua reactive state va command, khong reference UI component.
- Service/facade boc quanh gameplay hien tai, economy, inventory, settings, ads.
- Scene transition di qua `ISceneFlowService`, khong hard-code `SceneManager.LoadScene` trong button.
- Scene transition dung `LoadSceneAsync` + UniTask, co `IsLoading` de chan double click.
- UI backend model gom static config SO, runtime model, save data, service state va result structs.
- ID la stable id duoc khai bao trong ScriptableObject config, khong sinh moi moi lan runtime.
