using System;
using Game.UI.Presentation.Profile;
using Game.UI.Presentation.Settings;
using Game.UI.Service;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using ReflexResolution = Reflex.Enums.Resolution;

namespace Game.UI.Presentation.MainMenu
{
    public sealed class MainMenuInstaller : MonoBehaviour, IInstaller
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
                _ => new EquipmentItemDetailState(),
                new[] { typeof(EquipmentItemDetailState) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new ConfirmationDialogService(container.Resolve<IPanelService>()),
                new[] { typeof(ConfirmationDialogService), typeof(IConfirmationDialogService) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new MainMenuViewModel(
                    container.Resolve<IPanelService>(),
                    container.Resolve<ISceneFlowService>()),
                new[] { typeof(MainMenuViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new ProfilePanelViewModel(
                    container.Resolve<IPanelService>(),
                    container.Resolve<ICharacterRosterService>(),
                    container.Resolve<IInventoryService>(),
                    container.Resolve<IWalletService>(),
                    container.Resolve<EquipmentItemDetailState>()),
                new[] { typeof(ProfilePanelViewModel) },
                Lifetime.Scoped,
                ReflexResolution.Lazy);

            containerBuilder.RegisterFactory(
                container => new EquipmentItemDetailPanelViewModel(
                    container.Resolve<IPanelService>(),
                    container.Resolve<IInventoryService>(),
                    container.Resolve<IWalletService>(),
                    container.Resolve<IConfirmationDialogService>(),
                    container.Resolve<EquipmentItemDetailState>()),
                new[] { typeof(EquipmentItemDetailPanelViewModel) },
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
                throw new InvalidOperationException("[MainMenuInstaller] PanelHostView is not assigned.");
            }
        }
    }
}
