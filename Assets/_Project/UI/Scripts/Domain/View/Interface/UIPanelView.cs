using Game.UI.Data;
using UnityEngine;

namespace Game.UI.View
{
    public abstract class UIPanelView : MonoBehaviour
    {
        public abstract PanelId PanelId { get; }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}