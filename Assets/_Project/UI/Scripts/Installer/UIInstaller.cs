using System;
using Game.UI.Core;
using Game.UI.Service;
using Game.UI.View;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using ReflexResolution = Reflex.Enums.Resolution;

namespace Game.UI.Installer
{
    public class UIInstaller : MonoBehaviour, IInstaller
    {
        [Header("Global References")]
        [SerializeField] private SceneNameRegistry sceneNameRegistry;
        [SerializeField] private GameCatalogRegistry gameCatalogRegistry;
        [SerializeField] private EquipmentCatalogRegistry equipmentCatalogRegistry;
        [SerializeField] private LoadingPanelView loadingPanelView;

        [Header("Loading")]
        [SerializeField, Min(0f)] private float minimumLoadingDurationSeconds = 0.75f;

        [Header("Wallet")]
        [SerializeField, Min(0)] private int initialGold = 500;
        [SerializeField, Min(0)] private int initialDiamond;

        [Header("Progress Debug")]
        [SerializeField] private bool resetProgressOnStart;
        [SerializeField] private bool saveOnEveryChange = true;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            ValidateReferences();

            containerBuilder.RegisterValue(sceneNameRegistry, new[]
            {
                typeof(SceneNameRegistry),
                typeof(ISceneNameRegistry)
            });

            containerBuilder.RegisterValue(gameCatalogRegistry, new[]
            {
                typeof(GameCatalogRegistry),
                typeof(IGameCatalog)
            });

            containerBuilder.RegisterValue(equipmentCatalogRegistry, new[]
            {
                typeof(EquipmentCatalogRegistry),
                typeof(IEquipmentCatalog)
            });

            containerBuilder.RegisterValue(loadingPanelView, new[]
            {
                typeof(LoadingPanelView)
            });

            containerBuilder.RegisterFactory(
                container => new LoadingOverlayService(container.Resolve<LoadingPanelView>()),
                new[] { typeof(LoadingOverlayService), typeof(ILoadingOverlayService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                _ => new JsonProgressSaveService(),
                new[] { typeof(JsonProgressSaveService), typeof(IProgressSaveService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new PlayerProgressService(
                    container.Resolve<IProgressSaveService>(),
                    container.Resolve<IGameCatalog>(),
                    container.Resolve<IEquipmentCatalog>(),
                    initialGold,
                    initialDiamond,
                    resetProgressOnStart,
                    saveOnEveryChange),
                new[] { typeof(PlayerProgressService), typeof(IPlayerProgressService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new MapSelectionService(
                    container.Resolve<IGameCatalog>(),
                    container.Resolve<IPlayerProgressService>()),
                new[] { typeof(MapSelectionService), typeof(IMapSelectionService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new CharacterRosterService(
                    container.Resolve<IGameCatalog>(),
                    container.Resolve<IPlayerProgressService>()),
                new[] { typeof(CharacterRosterService), typeof(ICharacterRosterService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new InventoryService(
                    container.Resolve<IEquipmentCatalog>(),
                    container.Resolve<IPlayerProgressService>()),
                new[] { typeof(InventoryService), typeof(IInventoryService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new WalletService(container.Resolve<IPlayerProgressService>()),
                new[] { typeof(WalletService), typeof(IWalletService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new SceneFlowService(
                    container.Resolve<ISceneNameRegistry>(),
                    container.Resolve<IGameCatalog>(),
                    container.Resolve<ILoadingOverlayService>(),
                    minimumLoadingDurationSeconds),
                new[] { typeof(SceneFlowService), typeof(ISceneFlowService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);
        }

        private void ValidateReferences()
        {
            if (sceneNameRegistry == null)
            {
                throw new InvalidOperationException("[UIInstaller] SceneNameRegistry is not assigned.");
            }

            if (gameCatalogRegistry == null)
            {
                throw new InvalidOperationException("[UIInstaller] GameCatalogRegistry is not assigned.");
            }

            if (equipmentCatalogRegistry == null)
            {
                throw new InvalidOperationException("[UIInstaller] EquipmentCatalogRegistry is not assigned.");
            }

            if (loadingPanelView == null)
            {
                throw new InvalidOperationException("[UIInstaller] LoadingPanelView is not assigned.");
            }
        }
    }
}
