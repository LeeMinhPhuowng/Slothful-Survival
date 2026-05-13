using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GrassWiggle : MonoBehaviour
{
    private readonly List<Tween> _tweens = new();

    void Start()
    {
        _tweens.Add(
            transform.DOMoveX(transform.position.x + 0.1f, 1.2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject)
        );

        foreach (Transform t in transform)
        {
            _tweens.Add(
                t.DOLocalRotate(
                        new Vector3(0, 0, Random.Range(2.5f, 5f)),
                        Random.Range(1f, 2.5f)
                    )
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(gameObject)
            );
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