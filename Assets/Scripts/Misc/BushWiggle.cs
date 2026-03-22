using DG.Tweening;
using UnityEngine;
using static DG.Tweening.DOTween;
public class BushWiggle : MonoBehaviour
{
    void Start()
    {
        foreach (Transform t in transform)
        {
            Vector3 startPos = t.position;
            float duration = Random.Range(1f, 2f);

            t.DOMoveX(startPos.x + 0.1f, duration)
             .SetEase(Ease.InOutSine)
             .SetLoops(-1, LoopType.Yoyo);
        }

    }

    void Update()
    {
        
    }
}
