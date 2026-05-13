using UnityEngine;

namespace Game.UI.View
{
    public abstract class UIView<TViewModel> : MonoBehaviour
    {
        protected TViewModel ViewModel;

        public virtual void Bind(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public virtual void Unbind()
        {
            ViewModel = default;
        }
    }
}