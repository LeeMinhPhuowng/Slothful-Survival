using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.ChoosingMap
{
    public sealed class ChoosingMapBinder : MonoBehaviour
    {
        [SerializeField] private ChoosingMapSceneView sceneView;

        private ChoosingMapViewModel _viewModel;

        [Inject]
        private void Construct(ChoosingMapViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (sceneView == null)
            {
                sceneView = GetComponentInChildren<ChoosingMapSceneView>(true);
            }

            if (sceneView == null)
            {
                Debug.LogError("[ChoosingMapBinder] ChoosingMapSceneView is not assigned.");
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
