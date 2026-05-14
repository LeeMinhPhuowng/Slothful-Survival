using System;
using Game.UI.Presentation.Profile;
using Game.UI.Presentation.Settings;
using Game.UI.Service;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using ReflexResolution = Reflex.Enums.Resolution;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class GameplayInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private PanelHostView panelHostView;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            ValidateReferences();

            containerBuilder.RegisterValue(panelHostView, new[]
            {
                typeof(PanelHostView),
                typeof(IPanelHost)
            });

            containerBuilder.RegisterFactory(
                container => new PanelService(container.Resolve<IPanelHost>()),
                new[] { typeof(PanelService), typeof(IPanelService) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new ConfirmationDialogService(container.Resolve<IPanelService>()),
                new[] { typeof(ConfirmationDialogService), typeof(IConfirmationDialogService) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new GameRunService(
                    container.Resolve<IPanelService>(),
                    container.Resolve<ISceneFlowService>(),
                    container.Resolve<IWalletService>(),
                    container.Resolve<IMapSelectionService>(),
                    container.Resolve<IPlayerProgressService>()),
                new[] { typeof(GameRunService), typeof(IGameRunService) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new GameplayViewModel(container.Resolve<IGameRunService>()),
                new[] { typeof(GameplayViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new PausePanelViewModel(
                    container.Resolve<IGameRunService>(),
                    container.Resolve<IPanelService>()),
                new[] { typeof(PausePanelViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new RevivePanelViewModel(container.Resolve<IGameRunService>()),
                new[] { typeof(RevivePanelViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new RunResultPanelViewModel(container.Resolve<IGameRunService>()),
                new[] { typeof(RunResultPanelViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new ConfirmationDialogViewModel(container.Resolve<IConfirmationDialogService>()),
                new[] { typeof(ConfirmationDialogViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new SettingsPanelViewModel(
                    container.Resolve<IPanelService>(),
                    container.Resolve<ISettingsService>()),
                new[] { typeof(SettingsPanelViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);
        }

        private void ValidateReferences()
        {
            if (panelHostView == null)
            {
                throw new InvalidOperationException("[GameplayInstaller] PanelHostView is not assigned.");
            }
        }
    }
}
