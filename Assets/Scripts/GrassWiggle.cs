using DG.Tweening;
using UnityEngine;
using static DG.Tweening.DOTween;
public class GrassWiggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Transform t in transform)
        {
            t.DOLocalRotate(new Vector3(0, 0, Random.Range(2.5f, 5f)), Random.Range(1f, 2.5f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            transform.DOMoveX(transform.position.x + 0.1f, 1.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
