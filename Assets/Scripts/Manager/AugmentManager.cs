using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] List<GameObject> options;
    [SerializeField, Min(0f)] private float showAnimationDuration = 1f;
    [SerializeField, Range(0.1f, 1f)] private float showStartScale = 0.8f;

    public static AugmentManager Instance{get; private set;}

    List<AugmentInfoSO> augmentInfos = new List<AugmentInfoSO>();
    public List<BuffInfoSO> buffInfos;
    public List<AddInfoSO> addInfos;
    private Tween showTween;
    private Vector3 baseCanvasScale;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {    
        baseCanvasScale = canvas.transform.localScale;
        canvas.gameObject.SetActive(false);
    }

    private void SetUpAugments()
    {
        augmentInfos.Clear();
        augmentInfos.AddRange(buffInfos);
        if(WeaponManager.Instance.HasFreeSlot())
        {
            augmentInfos.AddRange(addInfos);
        }
        foreach (var weapon in WeaponManager.Instance.weapons)
        {
            WeaponInfoSO weaponInfo = weapon.GetComponent<Weapon>().Info;
            if (weaponInfo.nextAugmentInfo.Count == 0) continue;
            foreach (var nextAugment in weaponInfo.nextAugmentInfo)
            {
                augmentInfos.Add(nextAugment);
            }
        }
    }

    public void OnPlayerLevelUp()
    {
        Debug.Log("Leveled!");
        Time.timeScale = 0f;
        ShowAugments();
    }

    private void ShowAugments()
    {
        SetUpAugments();
        GenerateAugments();
        canvas.gameObject.SetActive(true);
        PlayShowAnimation();
    }

    private void GenerateAugments()
    {
        //Get augment1 info
        int index1 = Random.Range(0, augmentInfos.Count);
        Augment augment = options[0].GetComponent<Augment>();
        augment.Init(augmentInfos[index1]);

        //Get augment2 info
        int index2;
        Augment augment2 = options[1].GetComponent<Augment>();
        do
        {
            index2 = Random.Range(0, augmentInfos.Count);
        } while (index2 == index1);
        augment2.Init(augmentInfos[index2]);

        //Get augment3 info
        int index3;
        Augment augment3 = options[2].GetComponent<Augment>();
        do
        {
            index3 = Random.Range(0, augmentInfos.Count);
        } while (index3 == index1 || index3 == index2);
        augment3.Init(augmentInfos[index3]);
    }

    public void OnAugmentChosen()
    {
        showTween?.Kill();
        Time.timeScale = 1f;
        canvas.SetActive(false);
    }

    private void PlayShowAnimation()
    {
        showTween?.Kill();
        canvas.transform.localScale = baseCanvasScale * showStartScale;
        showTween = canvas.transform
            .DOScale(baseCanvasScale, showAnimationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void OnDestroy()
    {
        showTween?.Kill();
    }
}
