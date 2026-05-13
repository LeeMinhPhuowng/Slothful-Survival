# UI Data, Events And Services

---

## 1. IDs va enums

```csharp
public enum SceneId
{
    MainMenu,
    ChoosingMap,
    Gameplay
}

public enum PanelId
{
    Settings,
    Shop,
    Profile,
    Augment,
    Pause,
    Win,
    Revive,
    Lose
}

public enum EquipmentSlot
{
    ArmorBody,
    Helmet,
    Boots,
    Gloves,
    Pants,
    Weapon
}

public enum EquipmentTab
{
    All,
    ArmorBody,
    Helmet,
    Boots,
    Gloves,
    Pants,
    Weapon
}

public enum CurrencyType
{
    Gold,
    Diamond
}

public enum PurchaseFailureReason
{
    None,
    OfferNotFound,
    ProductUnavailable,
    InsufficientGold,
    InsufficientDiamond,
    InventoryFull,
    AlreadyOwned,
    PaymentCancelled,
    PaymentFailed,
    NetworkError,
    Unknown
}

public enum ReviveFailureReason
{
    None,
    TimerExpired,
    AdNotReady,
    AdSkipped,
    InsufficientDiamond,
    AlreadyRevived,
    PlayerNotDead,
    Unknown
}

public enum SceneLoadFailureReason
{
    None,
    SceneNotFound,
    AlreadyLoading,
    MissingGameplayRequest,
    Cancelled,
    Exception
}
```

Result objects la `readonly struct`, vi day la ket qua ngan han cua mot command/transaction, khong phai runtime entity can bind lau dai.

```csharp
public readonly struct PurchaseResult
{
    public readonly bool Success;
    public readonly PurchaseFailureReason FailureReason;
    public readonly RewardPayload Reward;
    public readonly string Message;
}

public readonly struct ReviveResult
{
    public readonly bool Success;
    public readonly ReviveFailureReason FailureReason;
    public readonly string Message;
}

public readonly struct SceneLoadResult
{
    public readonly bool Success;
    public readonly SceneLoadFailureReason FailureReason;
    public readonly SceneId TargetScene;
    public readonly string Message;
}

public readonly struct AdRewardResult
{
    public readonly bool Success;
    public readonly string PlacementId;
    public readonly RewardPayload Reward;
    public readonly string Message;
}
```

Model objects nhu `CharacterModel`, `EquipmentItemModel`, `ShopOfferModel`, `RunResultModel` nen la class/sealed class vi co the duoc expose qua reactive state, cache, list binding va save/load.

### Payload structs

Payload la du lieu ngan han de gui qua EventBus/ChannelSO hoac command bridge. Payload nen la `readonly struct` neu chi mang data va khong co behavior.

```csharp
public readonly struct SceneLoadStartedPayload
{
    public readonly SceneId TargetScene;
    public readonly string SceneName;
}

public readonly struct SceneLoadProgressPayload
{
    public readonly SceneId TargetScene;
    public readonly float Progress;
}

public readonly struct SceneLoadCompletedPayload
{
    public readonly SceneId LoadedScene;
    public readonly string SceneName;
}

public readonly struct SceneLoadFailedPayload
{
    public readonly SceneId TargetScene;
    public readonly SceneLoadFailureReason Reason;
    public readonly string Message;
}

public readonly struct PanelChangedPayload
{
    public readonly PanelId PanelId;
    public readonly bool IsOpen;
}

public readonly struct CurrencyChangedPayload
{
    public readonly CurrencyType CurrencyType;
    public readonly int OldAmount;
    public readonly int NewAmount;
    public readonly int Delta;
    public readonly string Source;
}

public readonly struct EquipmentChangedPayload
{
    public readonly string CharacterId;
    public readonly EquipmentSlot Slot;
    public readonly string PreviousItemId;
    public readonly string NewItemId;
}

public readonly struct NormalAttackRequestedPayload
{
    public readonly string CharacterId;
    public readonly string WeaponId;
    public readonly int RequestFrame;
}

public readonly struct PlayerLeveledUpPayload
{
    public readonly string CharacterId;
    public readonly int NewLevel;
    public readonly IReadOnlyList<string> AugmentOptionIds;
}

public readonly struct PlayerDiedPayload
{
    public readonly string CharacterId;
    public readonly string CauseId;
    public readonly float RunTimeSeconds;
}

public readonly struct RunResultPayload
{
    public readonly string MapId;
    public readonly bool IsWin;
    public readonly int GoldEarned;
    public readonly int EnemyDefeated;
    public readonly float PlayingTimeSeconds;
}

public readonly struct ReviveRequestedPayload
{
    public readonly string CharacterId;
    public readonly RevivePaymentType PaymentType;
}

public readonly struct ReviveCompletedPayload
{
    public readonly string CharacterId;
    public readonly bool Success;
    public readonly ReviveFailureReason FailureReason;
}

public readonly struct RewardPayload
{
    public readonly CurrencyRewardPayload[] Currencies;
    public readonly string[] EquipmentItemIds;
    public readonly string[] CharacterIds;
}

public readonly struct CurrencyRewardPayload
{
    public readonly CurrencyType CurrencyType;
    public readonly int Amount;
}
```

Them enum cho revive payment:

```csharp
public enum RevivePaymentType
{
    Ads,
    Diamond
}
```

Quy uoc nullable/string rong:

- `PreviousItemId` co the rong neu slot truoc do khong co item.
- `WeaponId` co the rong neu normal attack khong gan voi weapon config.
- `CauseId` la id cua enemy/trap/projectile neu co, rong neu chua track.

---

## 2. Core services

### Scene flow

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;

public interface ISceneFlowService
{
    ReadOnlyReactiveProperty<SceneId> CurrentScene { get; }
    ReadOnlyReactiveProperty<bool> IsLoading { get; }
    ReadOnlyReactiveProperty<float> LoadingProgress { get; }

    UniTask LoadMainMenuAsync(CancellationToken cancellationToken = default);
    UniTask LoadChoosingMapAsync(CancellationToken cancellationToken = default);
    UniTask LoadGameplayAsync(GameplayLoadRequest request, CancellationToken cancellationToken = default);
    UniTask ReloadGameplayAsync(CancellationToken cancellationToken = default);
    UniTask LoadNextLevelAsync(CancellationToken cancellationToken = default);
}
```

`GameplayLoadRequest`:

```csharp
public readonly struct GameplayLoadRequest
{
    public readonly string MapId;
    public readonly string CharacterId;
    public readonly bool IsRetry;

    public GameplayLoadRequest(string mapId, string characterId, bool isRetry = false)
    {
        MapId = mapId;
        CharacterId = characterId;
        IsRetry = isRetry;
    }
}
```

Implementation notes:

- Dung `SceneManager.LoadSceneAsync(sceneName).ToUniTask(...)` hoac await truc tiep `AsyncOperation` sau khi import UniTask.
- Set `IsLoading = true` truoc khi load, reset ve false trong `finally`.
- Update `LoadingProgress` tu `AsyncOperation.progress`.
- Neu dang loading, command load scene tiep theo phai return som.
- Scene-specific bootstrap chay sau khi load thanh cong.

### Loading overlay

```csharp
public interface ILoadingOverlayService
{
    ReadOnlyReactiveProperty<bool> IsVisible { get; }
    ReadOnlyReactiveProperty<float> Progress { get; }
    void Show();
    void SetProgress(float progress);
    void Hide();
}
```

Loading overlay co the la panel global hoac prefab rieng trong tung scene. Neu game can chuyen scene co fade, `ILoadingOverlayService` se phu trach fade in/out, con `ISceneFlowService` phu trach load scene.

### Panel service

```csharp
public interface IPanelService
{
    ReadOnlyReactiveProperty<PanelId?> CurrentPanel { get; }
    void Open(PanelId panelId);
    void Close(PanelId panelId);
    void CloseTop();
    void CloseAll();
}
```

Panel service nen co option pause game khi mo mot so panel:

| Panel | Pause gameplay |
| --- | --- |
| Settings | optional |
| Shop | optional |
| Profile | optional trong gameplay |
| Augment | yes |
| Pause | yes |
| Win | yes/end run |
| Revive | yes |
| Lose | yes/end run |

### Settings

```csharp
public interface ISettingsService
{
    ReactiveProperty<float> MusicVolume { get; }
    ReactiveProperty<float> SfxVolume { get; }
    void SetMusicVolume(float value);
    void SetSfxVolume(float value);
    void Save();
}
```

### External link

```csharp
public interface IExternalLinkService
{
    void OpenFacebook();
}
```

---

## 3. Economy and shop

```csharp
public interface IWalletService
{
    ReadOnlyReactiveProperty<int> Gold { get; }
    ReadOnlyReactiveProperty<int> Diamond { get; }
    bool HasEnough(CurrencyType currencyType, int amount);
    bool TrySpend(CurrencyType currencyType, int amount);
    void Add(CurrencyType currencyType, int amount, string source);
}

public interface IShopService
{
    ReadOnlyReactiveProperty<IReadOnlyList<ShopOfferModel>> RealMoneyOffers { get; }
    ReadOnlyReactiveProperty<IReadOnlyList<ShopOfferModel>> CurrencyOffers { get; }
    Observable<PurchaseResult> BuyRealMoneyOffer(string offerId);
    PurchaseResult BuyCurrencyOffer(string offerId);
}
```

`ShopOfferModel`:

```csharp
public sealed class ShopOfferModel
{
    public string OfferId;
    public string DisplayName;
    public Sprite Icon;
    public CurrencyType? PriceCurrency;
    public int PriceAmount;
    public string RealMoneyProductId;
    public RewardPayload Reward;
}
```

ID trong `OfferId`, `ItemId`, `MapId`, `CharacterId` khong duoc sinh moi o runtime. Chung den tu ScriptableObject config, vi du `CharacterConfigSO.characterId = "char_knight"`, `EquipmentItemSO.itemId = "weapon_iron_sword_01"`, `ShopOfferSO.offerId = "shop_gold_pack_small"`.

---

## 4. Inventory and character

```csharp
public interface ICharacterRosterService
{
    ReadOnlyReactiveProperty<CharacterModel> SelectedCharacter { get; }
    IReadOnlyList<CharacterModel> Characters { get; }
    void SelectPrevious();
    void SelectNext();
    void Select(string characterId);
}

public interface IInventoryService
{
    ReadOnlyReactiveProperty<IReadOnlyDictionary<EquipmentSlot, EquipmentItemModel>> EquippedItems { get; }
    ReadOnlyReactiveProperty<IReadOnlyList<EquipmentItemModel>> OwnedItems { get; }
    ReadOnlyReactiveProperty<EquipmentTab> SelectedTab { get; }
    ReadOnlyReactiveProperty<IReadOnlyList<EquipmentItemModel>> FilteredItems { get; }
    void SelectTab(EquipmentTab tab);
    bool CanEquip(string itemId);
    void Equip(string itemId);
    void Unequip(EquipmentSlot slot);
}
```

`EquipmentItemModel`:

```csharp
public sealed class EquipmentItemModel
{
    public string ItemId;
    public string DisplayName;
    public Sprite Icon;
    public EquipmentSlot Slot;
    public int Armor;
    public int Damage;
    public int MaxHealth;
}
```

---

## 5. Gameplay HUD and combat input

```csharp
public interface IPlayerProgressService
{
    ReadOnlyReactiveProperty<float> ExpPercent { get; }
    ReadOnlyReactiveProperty<int> CurrentLevel { get; }
}

public interface ICombatInputService
{
    ReadOnlyReactiveProperty<bool> CanNormalAttack { get; }
    ReadOnlyReactiveProperty<float> NormalAttackCooldownPercent { get; }
    void RequestNormalAttack();
}

public interface IPlayerMovementInput
{
    Vector2 MoveVector { get; }
}
```

`EnergySlider` trong yeu cau UI nen bind vao `ExpPercent`. Neu muon giu ten field trong prefab la EnergySlider, domain ViewModel van nen dat la EXP de tranh nham voi currency energy.

---

## 6. Run lifecycle

```csharp
public interface IGameRunService
{
    ReadOnlyReactiveProperty<bool> IsPaused { get; }
    ReadOnlyReactiveProperty<RunResultModel> CurrentRunResult { get; }
    void Pause();
    void Resume();
    void TryAgain();
    void ReturnToChoosingMap();
}

public interface IReviveService
{
    ReadOnlyReactiveProperty<int> RemainingSeconds { get; }
    ReadOnlyReactiveProperty<float> TimerPercent { get; }
    ReadOnlyReactiveProperty<bool> CanReviveByAds { get; }
    ReadOnlyReactiveProperty<bool> CanReviveByDiamond { get; }
    Observable<ReviveResult> ReviveByAds();
    ReviveResult ReviveByDiamond();
}
```

`RunResultModel`:

```csharp
public sealed class RunResultModel
{
    public int GoldEarned;
    public int EnemyDefeated;
    public TimeSpan PlayingTime;
    public bool IsWin;
}
```

---

## 7. Ads and purchase

```csharp
public interface IAdService
{
    ReadOnlyReactiveProperty<bool> IsRewardedAdReady { get; }
    Observable<AdRewardResult> ShowRewardedAd(string placementId);
}

public interface ICashPurchaseService
{
    Observable<PurchaseResult> Purchase(string productId);
}
```

Rule: UI khong grant reward truc tiep. Reward chi di qua service sau khi ad/IAP thanh cong.

---

## 8. UI events

| Event | Producer | Consumer | Payload |
| --- | --- | --- | --- |
| `SceneChangeRequested` | Scene VM | Scene flow service | Direct service call, not event |
| `SceneLoadStarted` | Scene flow service | Loading overlay | Reactive state/EventBus optional |
| `SceneLoadProgressChanged` | Scene flow service | Loading overlay | Reactive state preferred |
| `SceneLoadCompleted` | Scene flow service | Bootstrap/logging | EventBus optional |
| `SceneLoadFailed` | Scene flow service | Toast/logging | EventBus optional |
| `PanelOpenRequested` | Any VM | Panel service | Direct service call, not event |
| `PanelClosed` | Panel service | Scene/panel VMs | Reactive state preferred |
| `SettingsChanged` | Settings service | Audio service | Reactive property/direct service |
| `CurrencyChanged` | Wallet service | ShopPanel | Reactive property preferred |
| `EquipmentChanged` | Inventory service | Inventory/Stats | Reactive property/EventBus optional |
| `NormalAttackRequested` | Gameplay HUD | Combat service | Direct service call or EventBus |
| `PlayerLeveledUp` | Gameplay | Augment panel | ChannelSO bridge or EventBus |
| `RunWon` | Gameplay | Win panel | EventBus or run service reactive state |
| `PlayerDied` | Gameplay | Revive panel | ChannelSO bridge or EventBus |
| `ReviveExpired` | Revive service | Lose panel | Direct service call/EventBus |
| `RunLost` | Gameplay | Lose panel | EventBus or run service reactive state |

---

## 9. Legacy adapters

`Legacy` nghia la code cu/prototype hien dang co san trong project, thuong la MonoBehaviour singleton hoac button handler truc tiep. No khong co nghia la code xau phai xoa ngay. Trong buoc refactor dau, cac service co the boc code hien co:

| New service | Legacy target |
| --- | --- |
| `LegacySceneFlowService` | `SceneManager`, `GameManager` |
| `LegacyMapSelectionService` | `LevelLoader`, `CoverFlow`, `Spawner` |
| `LegacyCharacterRosterService` | `PlayerSetter`, `CharacterInfoSO` |
| `LegacyCombatInputService` | `WeaponManager`, weapon behaviours |
| `LegacyPlayerProgressService` | `PlayerEXP`, `PlayerLevelManager` |
| `LegacyAugmentService` | `AugmentManager`, `Augment` |
| `LegacyRunService` | game win/lose/death flow can them |

Adapter la cau noi tam thoi. View/ViewModel khong biet singleton nao dang nam ben duoi.

Vi du: `LegacyCharacterRosterService` co the tam thoi goi `PlayerSetter`, nhung UI chi biet `ICharacterRosterService`. Khi sau nay tach `PlayerSetter` thanh service that, UI khong can sua.
