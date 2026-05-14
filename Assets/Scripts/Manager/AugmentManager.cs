using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Collections; // Add DOTween

public class AugmentManager : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Transform mainPanel; // The panel containing the options
    [SerializeField] List<GameObject> options;

    public static AugmentManager Instance{get; private set;}

    List<AugmentInfoSO> augmentInfos = new List<AugmentInfoSO>();
    public List<BuffInfoSO> buffInfos;
    public List<AddInfoSO> addInfos;

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
        foreach (var weapon in WeaponManager.Instance.ActiveWeapons)
        {
            WeaponInfoSO weaponInfo = weapon.Info;
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
        StartCoroutine(LevelUpSequence());
    }

    private IEnumerator LevelUpSequence()
    {
        // 1. Wait a bit for the Level Up VFX/Text to pop up
        yield return new WaitForSecondsRealtime(1.0f);

        // 2. Pause the game
        Time.timeScale = 0f;

        // 3. Show and Animate the UI
        ShowAugments();
    }

    private void ShowAugments()
    {
        SetUpAugments();
        GenerateAugments();
        
        canvas.gameObject.SetActive(true);
        
        // 1. Animate the Main Panel background (if any)
        if (mainPanel != null)
        {
            mainPanel.localScale = Vector3.zero;
            mainPanel.DOScale(Vector3.one, 1f).SetEase(Ease.OutSine).SetUpdate(true);
        }

        // 2. Animate each option card sequentially
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] == null) continue;

            Transform card = options[i].transform;
            
            // Record original position (assuming they are set correctly in Layout Group or manually)
            // If using Layout Groups, you might need to disable them temporarily or use a wrapper.
            // For now, let's assume we can move them locally.
            
            Vector3 targetPos = card.localPosition;
            Vector3 startPos = targetPos + new Vector3(0, -300f, 0); // Start 300 units below

            card.localPosition = startPos;
            card.localScale = Vector3.zero;

            float delay = i * 0.25f; // Staggered delay (0s, 0.15s, 0.3s)

            card.DOLocalMove(targetPos, 0.5f).SetDelay(delay).SetEase(Ease.OutSine).SetUpdate(true);
            card.DOScale(Vector3.one, 0.5f).SetDelay(delay).SetEase(Ease.OutSine).SetUpdate(true);
        }
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
        Time.timeScale = 1f;
        canvas.SetActive(false);
    }
}
