using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.MainMenu
{
    public sealed class MainMenuBinder : MonoBehaviour
    {
        [SerializeField] private MainMenuSceneView sceneView;

        private MainMenuViewModel _viewModel;

        [Inject]
        private void Construct(MainMenuViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (sceneView == null)
            {
                sceneView = GetComponentInChildren<MainMenuSceneView>(true);
            }

            if (sceneView == null)
            {
                Debug.LogError("[MainMenuBinder] MainMenuSceneView is not assigned.");
                return;
            }

            sceneView.Bind(_viewModel);
        }

        private void OnDestroy()
        {
            if (sceneView != null)
            {
                sceneView.Unbind();
            }
        }
    }
}
