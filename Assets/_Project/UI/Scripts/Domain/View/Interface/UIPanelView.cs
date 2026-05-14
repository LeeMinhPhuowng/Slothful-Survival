using Game.UI.Data;
using DG.Tweening;
using UnityEngine;

namespace Game.UI.View
{
    public abstract class UIPanelView : MonoBehaviour
    {
        [Header("Show Animation")]
        [SerializeField] private Transform animatedRoot;
        [SerializeField] private bool animateOnShow = true;
        [SerializeField, Min(0f)] private float showAnimationDuration = 1f;
        [SerializeField, Range(0.1f, 1f)] private float showStartScale = 0.8f;

        private Tween _showTween;
        private Vector3 _baseScale;
        private bool _hasBaseScale;

        public abstract PanelId PanelId { get; }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            PlayShowAnimation();
        }

        public virtual void Hide()
        {
            KillShowAnimation();
            gameObject.SetActive(false);
        }

        protected virtual void OnDisable()
        {
            KillShowAnimation();
        }

        private void PlayShowAnimation()
        {
            if (!animateOnShow)
            {
                return;
            }

            EnsureBaseScale();
            KillShowAnimation();

            Transform target = GetAnimatedTarget();
            target.localScale = _baseScale * showStartScale;
            _showTween = target
                .DOScale(_baseScale, showAnimationDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }

        private void EnsureBaseScale()
        {
            if (_hasBaseScale)
            {
                return;
            }

            _baseScale = GetAnimatedTarget().localScale;
            _hasBaseScale = true;
        }

        private void KillShowAnimation()
        {
            _showTween?.Kill();
            _showTween = null;
        }

        private Transform GetAnimatedTarget()
        {
            return animatedRoot != null ? animatedRoot : transform;
        }
    }
}
