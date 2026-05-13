# UI Backend Model And Event Strategy

---

## 1. Muc tieu

File nay bo sung phan con thieu cua UI design:

- Backend model nao can co de UI hoat dong.
- ID lay tu dau va tai sao dung ID.
- Result nen la enum, struct hay class.
- Event nao dung direct service call, reactive state, EventBus, ChannelSO hay C# event/action.
- `Legacy` nghia la gi trong giai doan refactor.

---

## 2. UI backend model la gi

Trong UI MVVM, backend model khong phai server backend. No la lop data/runtime nam sau ViewModel:

```text
ScriptableObject Config
        |
        v
Runtime Model
        |
        v
Service State
        |
        v
ViewModel
        |
        v
View
```

### 2.1. Static config

Static config la data edit trong Unity Inspector, thuong la ScriptableObject:

- `CharacterConfigSO`
- `MapConfigSO` hoac mo rong `LevelSO`
- `EquipmentItemSO`
- `ShopOfferSO`
- `CurrencyConfigSO`
- `AugmentConfigSO` hoac tiep tuc dung `AugmentInfoSO`

Config phai co stable id:

```csharp
public abstract class IdentifiedConfigSO : ScriptableObject
{
    [SerializeField] private string id;
    public string Id => id;
}
```

Vi project hien da co `CharacterInfoSO`, `WeaponInfoSO`, `LevelSO`, co the bo sung field id truc tiep thay vi tao base class ngay.

### 2.2. Runtime model

Runtime model la object thuong duoc tao tu config + save data:

```csharp
public sealed class CharacterModel
{
    public string CharacterId;
    public CharacterInfoSO Config;
    public bool IsUnlocked;
    public int Level;
}

public sealed class EquipmentItemModel
{
    public string ItemId;
    public EquipmentItemSO Config;
    public bool IsNew;
    public int UpgradeLevel;
}

public sealed class ShopOfferModel
{
    public string OfferId;
    public ShopOfferSO Config;
    public int RemainingStock;
}
```

Model dung class vi UI can cache, bind list, compare selected item, va cap nhat state.

### 2.3. Save data

Save data chi luu ID va value nho, khong luu reference ScriptableObject:

```csharp
public sealed class PlayerSaveData
{
    public string SelectedCharacterId;
    public List<string> OwnedEquipmentItemIds;
    public Dictionary<EquipmentSlot, string> EquippedItemIds;
    public int Gold;
    public int Diamond;
    public float MusicVolume;
    public float SfxVolume;
    public List<string> UnlockedMapIds;
}
```

Khi load game:

```text
Save IDs -> Repository lookup config by ID -> Runtime models -> Services -> ViewModels
```

---

## 3. ID lay tu dau

ID nen duoc khai bao trong config asset va on dinh qua toan bo vong doi game.

Vi du:

| Asset | ID |
| --- | --- |
| Knight character | `char_knight` |
| Archer character | `char_archer` |
| Mage character | `char_mage` |
| Map 1 | `map_forest_01` |
| Iron sword | `weapon_iron_sword_01` |
| Body armor | `armor_body_leather_01` |
| Gold pack small | `shop_gold_pack_small` |

Khong nen dung:

- Index list, vi doi thu tu asset se sai save.
- `GetInstanceID()`, vi thay doi moi session.
- Ten file asset lam source duy nhat, vi rename file lam hong save.

Nen co editor validation:

- ID khong rong.
- ID unique trong cung catalog.
- ID chi dung lowercase, number, underscore.

---

## 4. Repository/catalog layer

Can co catalog de lookup config theo ID:

```csharp
public interface IGameCatalog
{
    CharacterInfoSO GetCharacter(string characterId);
    LevelSO GetMap(string mapId);
    EquipmentItemSO GetEquipment(string itemId);
    ShopOfferSO GetShopOffer(string offerId);
    IReadOnlyList<CharacterInfoSO> GetAllCharacters();
    IReadOnlyList<LevelSO> GetAllMaps();
    IReadOnlyList<EquipmentItemSO> GetAllEquipment();
}
```

Ban dau co the implement bang ScriptableObject:

```text
GameCatalogSO
|-- Characters: List<CharacterInfoSO>
|-- Maps: List<LevelSO>
|-- EquipmentItems: List<EquipmentItemSO>
|-- ShopOffers: List<ShopOfferSO>
```

Khi game lon hon, catalog co the chuyen sang Addressables, nhung UI/service contract khong can doi.

---

## 5. Result design

Result la ket qua cua mot action, nen dung `readonly struct` + enum reason:

```csharp
public readonly struct PurchaseResult
{
    public readonly bool Success;
    public readonly PurchaseFailureReason FailureReason;
    public readonly RewardPayload Reward;
    public readonly string Message;
}
```

Dung enum cho reason vi UI can switch de hien message/icon:

```csharp
public enum PurchaseFailureReason
{
    None,
    OfferNotFound,
    InsufficientGold,
    InsufficientDiamond,
    PaymentCancelled,
    NetworkError,
    Unknown
}
```

Quy uoc:

- `Result` la struct khi chi la response ngan han.
- `Model` la class khi can ton tai lau, bind, cache, selected.
- `Payload` la readonly struct khi chi gui qua event.
- `Config` la ScriptableObject khi edit trong Inspector.
- `SaveData` la class/DTO serialize JSON.

---

## 6. Payload design

Payload la goi du lieu di qua event boundary. No khong nen chua logic, khong nen giu reference UI object, va nen uu tien ID/stable value.

Payload can co cho UI system:

| Payload | Khi nao dung | Transport |
| --- | --- | --- |
| `SceneLoadStartedPayload` | Bat dau async load scene | EventBus optional |
| `SceneLoadProgressPayload` | Progress load scene | Reactive preferred, EventBus optional |
| `SceneLoadCompletedPayload` | Load scene xong | EventBus optional |
| `SceneLoadFailedPayload` | Load scene loi | EventBus |
| `PanelChangedPayload` | Panel open/close | Reactive preferred |
| `CurrencyChangedPayload` | Gold/Diamond thay doi | Reactive preferred, EventBus optional |
| `EquipmentChangedPayload` | Equip/unequip item | Reactive preferred, EventBus optional |
| `NormalAttackRequestedPayload` | UI request danh thuong | Direct call or EventBus |
| `PlayerLeveledUpPayload` | Gameplay bao level up | ChannelSO bridge or EventBus |
| `PlayerDiedPayload` | Gameplay bao player chet | ChannelSO bridge or EventBus |
| `RunResultPayload` | Win/Lose summary | EventBus or run service state |
| `ReviveRequestedPayload` | Yeu cau revive | Direct call |
| `ReviveCompletedPayload` | Ket qua revive | Result return/EventBus optional |
| `RewardPayload` | Reward cua shop/ads/run | Result payload |

Example:

```csharp
public readonly struct RunResultPayload
{
    public readonly string MapId;
    public readonly bool IsWin;
    public readonly int GoldEarned;
    public readonly int EnemyDefeated;
    public readonly float PlayingTimeSeconds;
}
```

Rule:

- Payload di qua event nen la `readonly struct`.
- Payload reference asset chi khi event nam trong cung Unity boundary va khong can save/replay. Mac dinh van nen gui ID.
- Payload khong thay the model. UI list/detail van bind model tu service.

---

## 7. Co nen tat ca tuong tac bang ID khong

Khong phai tat ca. Dung ID o boundary, dung reference/model o ben trong.

Dung ID khi:

- Save/load.
- Command tu ViewModel: `SelectItem(itemId)`, `BuyOffer(offerId)`.
- Event payload.
- Lookup trong catalog.

Dung model/reference khi:

- Service da resolve xong va can tinh stat.
- ViewModel bind list item.
- Gameplay spawn prefab tu config.

Workflow:

```text
View click item card -> itemId
ViewModel.SelectItem(itemId)
InventoryService lookup model by id
SelectedItem reactive property = EquipmentItemModel
View bind selected item detail
```

---

## 8. Event strategy

Khong nen bien tat ca thanh EventBus. UI MVVM nen uu tien direct service call va reactive state.

### 8.1. Direct service call

Dung cho command co chu dich ro tu UI:

- Open settings.
- Open shop.
- Load scene.
- Buy offer.
- Equip item.
- Resume game.
- Try again.

Vi du:

```text
Button -> ViewModel -> IPanelService.Open(PanelId.Shop)
```

Khong can event.

### 8.2. Reactive state

Dung cho state UI can cap nhat lien tuc:

- Gold/Diamond.
- Music/SFX volume.
- EXP percent.
- Loading progress.
- Current panel.
- Selected character.
- Inventory filtered items.

Vi du:

```text
WalletService.Gold -> ShopPanelViewModel.GoldText -> ShopPanelView
```

Khong can EventBus.

### 8.3. EventBus

Dung cho domain event runtime ma producer khong can biet consumer:

- `RunWon`
- `RunLost`
- `SceneLoadFailed`
- `PurchaseCompleted`
- `RewardGranted`
- `NormalAttackRequested` neu combat input tach doc lap

EventBus nen gom it event that su cross-system. Neu event chi de UI refresh text thi sai, nen dung reactive property.

### 8.4. ChannelSO

Dung o ranh gioi Unity scene/prefab/legacy, khi can keo-tha asset trong Inspector:

- Gameplay MonoBehaviour cu raise `PlayerDiedChannelSO`.
- `PlayerLevelManager` raise `PlayerLeveledUpChannelSO`.
- Prototype scene chua co DI nhung can noi event.

ChannelSO la bridge tot giua code cu va system moi, nhung khong nen la duong chinh trong ViewModel moi.

### 8.5. C# event/action

Dung cuc bo trong 1 object/component, khong dung cross-feature:

- Animation callback trong mot View.
- Button wrapper noi bo.
- Local timer callback.

Khong nen dung static `Action` cho global game event moi, vi kho quan ly lifetime va unsubscribe.

---

## 9. Event decision table

| Case | Mechanism |
| --- | --- |
| Button mo panel | Direct service call |
| Button chuyen scene | Direct async service call |
| Loading progress | Reactive state |
| Currency display | Reactive state |
| Inventory list/filter | Reactive state |
| Equip item thanh cong | Direct result + reactive state update |
| Purchase fail/success | Result return, EventBus optional for analytics/toast |
| Player level up tu gameplay cu | ChannelSO bridge, then service opens AugmentPanel |
| Player death tu gameplay cu | ChannelSO bridge, then service opens RevivePanel |
| Run won/lost | EventBus or `IGameRunService.CurrentRunResult` reactive |
| Local visual animation ended | C# event/action local |

---

## 10. Legacy la gi

`Legacy` trong tai lieu nay nghia la code hien tai cua project truoc khi refactor UI MVVM:

- MonoBehaviour singleton.
- Button handler gan truc tiep trong Inspector.
- `GameManager` bat/tat object scene.
- `PlayerSetter` chon/spawn player truc tiep.
- `AugmentManager` random va hien UI truc tiep.

Legacy khong co nghia la phai xoa ngay. No la lop can boc lai bang adapter:

```text
New ViewModel -> New interface -> Legacy adapter -> Existing code
```

Vi du:

```csharp
public sealed class LegacyPlayerProgressService : IPlayerProgressService
{
    // Tam thoi doc tu PlayerEXP/PlayerLevelManager hien co.
    // Sau nay thay bang PlayerProgressService that ma UI khong can sua.
}
```

Muc tieu la UI moi khong phu thuoc vao code cu, con code cu van co the chay trong luc refactor tung phan.

---

## 11. Workflow de implement

1. Them stable id vao config SO hien co: character, level/map, weapon/equipment, shop offer.
2. Tao `GameCatalogSO` de gom lists va validate unique ID.
3. Tao save DTO chi luu id/value.
4. Tao runtime services:
   - `CharacterRosterService`
   - `InventoryService`
   - `WalletService`
   - `ShopService`
   - `SettingsService`
   - `GameRunService`
5. Service expose reactive state cho ViewModel.
6. ViewModel goi service command va bind reactive state.
7. Gameplay cu phat ChannelSO/EventBus vao service bridge.
8. Khi service that da on, giam dan legacy adapter.
