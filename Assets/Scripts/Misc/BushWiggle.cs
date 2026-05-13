using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static DG.Tweening.DOTween;

public class BushWiggle : MonoBehaviour
{
    private List<Tween> _tweens = new();

    private void Start()
    {
        foreach (Transform t in transform)
        {
            Vector3 startPos = t.position;
            float duration = Random.Range(1f, 2f);

            Tween wiggle = t.DOMoveX(startPos.x + 0.1f, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            _tweens.Add(wiggle);
        }

    }

    private void OnDestroy()
    {
        foreach (Tween tween in _tweens)
        {
            tween?.Kill();
        }

        _tweens.Clear();
    }
}
