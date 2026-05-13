using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class GameplayBinder : MonoBehaviour
    {
        [SerializeField] private GameplaySceneView sceneView;

        private GameplayViewModel _viewModel;

        [Inject]
        private void Construct(GameplayViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (sceneView == null)
            {
                sceneView = GetComponentInChildren<GameplaySceneView>(true);
            }

            if (sceneView == null)
            {
                Debug.LogError("[GameplayBinder] GameplaySceneView is not assigned.");
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
