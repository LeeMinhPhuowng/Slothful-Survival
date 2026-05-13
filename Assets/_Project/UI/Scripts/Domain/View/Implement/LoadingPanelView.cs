using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.View
{
    public sealed class LoadingPanelView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text progressText;

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);

            if (canvasGroup == null) 
            {
                return;
            }
           
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);

            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = "Loading... " + $"{Mathf.RoundToInt(progress * 100f)}%";
            }
        }
    }
}