using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Game.UI.Service;
using Reflex.Attributes;
using TMPro;
using Game.UI.Model;

public class LevelCoverFlow : MonoBehaviour
{
    [Header("Scroll Setup")]
    [SerializeField] private ScrollRect scrollRect;          // ScrollRect chứa content
    [SerializeField] private float spacing = 300f;           // Khoảng cách giữa các item

    [Header("CoverFlow Settings")]
    [SerializeField] private float scaleFactor = 0.5f;       // Scale nhỏ nhất của item xa center
    [SerializeField] private float rotationFactor = 25f;     // Góc xoay max của item xa center
    [SerializeField] private float fadeFactor = 0.5f;        // Alpha nhỏ nhất của item xa center
    [SerializeField] private float snapDuration = 0.5f;      // Thời gian tween khi snap

    [Header("Level Name")]
    [SerializeField] private TextMeshProUGUI levelName;

    [Header("Actions")]
    [SerializeField] private Button playButton;

    private readonly List<RectTransform> items = new();
    private RectTransform content;
    private int currentItemIndex = 0;

    private IMapSelectionService _mapSelectionService;

    [Inject]
    private void Construct(IMapSelectionService mapSelectionService)
    {
        _mapSelectionService = mapSelectionService;
    }

    private void Start()
    {
        if (scrollRect == null || scrollRect.content == null || scrollRect.viewport == null)
        {
            Debug.LogError("[LevelCoverFlow] ScrollRect, content, or viewport is not assigned.");
            return;
        }

        if (_mapSelectionService == null)
        {
            Debug.LogError("[LevelCoverFlow] IMapSelectionService is not injected.");
            return;
        }

        content = scrollRect.content;

        // Lấy tất cả child làm item
        foreach (Transform child in content)
        {
            items.Add(child as RectTransform);
        }

        // Sắp xếp item theo spacing
        for (int i = 0; i < items.Count; i++)
        {
            items[i].anchoredPosition = new Vector2(i * spacing, 0);
        }

        // Đảm bảo content đủ rộng để snap item cuối
        float contentWidth = (items.Count - 1) * spacing + scrollRect.viewport.rect.width;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);

        scrollRect.horizontal = true;
        scrollRect.vertical = false;
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

        // Cập nhật realtime khi kéo
        scrollRect.onValueChanged.AddListener(_ => UpdateItemsRealtime());

        // Căn giữa item đầu tiên
        if (items.Count > 0)
        {
            CenterOnItem(items[0], true);
            SelectMap(0);
        }
    }

    public void OnBeginDrag()
    {
        content.DOKill();
    }

    public void OnEndDrag()
    {
        scrollRect.StopMovement();
        SnapToNearestItem();
    }

    private void UpdateItemsRealtime()
    {
        float viewportCenter = scrollRect.viewport.rect.width / 2f;

        for (int i = 0; i < items.Count; i++)
        {
            RectTransform item = items[i];
            float itemCenter = item.localPosition.x + content.localPosition.x;
            float distance = itemCenter - viewportCenter;
            float normalized = Mathf.Clamp01(Mathf.Abs(distance) / spacing);

            float scale = Mathf.Lerp(scaleFactor, 1f, 1f - normalized);
            float angle = Mathf.Clamp(rotationFactor * (distance / spacing), -rotationFactor, rotationFactor);
            float alpha = Mathf.Lerp(fadeFactor, 1f, 1f - normalized);
            MapModel itemMap = GetMapByIndex(i);
            if (itemMap != null && !itemMap.IsUnlocked)
            {
                alpha *= 0.5f;
            }

            item.localScale = new Vector3(scale, scale, 1f);
            item.localRotation = Quaternion.Euler(0f, -angle, 0f);

            Image img = item.GetComponentInChildren<Image>();
            if (img != null)
                img.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    // Snap item gần viewport center
    private void SnapToNearestItem()
    {
        float viewportCenter = scrollRect.viewport.rect.width / 2f;
        RectTransform nearest = null;
        float minDistance = float.MaxValue;

        int index = 0;
        foreach (RectTransform item in items)
        {
            float itemCenter = item.localPosition.x + content.localPosition.x;
            float distance = Mathf.Abs(itemCenter - viewportCenter);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = item;
                currentItemIndex = index;
            }
            index++;
        }
        
        SelectMap(currentItemIndex);

        if (nearest != null)
            CenterOnItem(nearest, false);
    }

    private void SelectMap(int currentIndex)
    {
        MapModel map = GetMapByIndex(currentIndex);
        if (map == null)
        {
            Debug.LogWarning($"[LevelCoverFlow] Item index {currentIndex} is out of Maps range.");
            return;
        }

        if (!map.IsUnlocked)
        {
            SetLevelName("Locked Level");
            SetPlayButtonActive(false);
            return;
        }

        _mapSelectionService.Select(map.MapId);
        SetLevelName(map.DisplayName);
        SetPlayButtonActive(true);
    }

    private MapModel GetMapByIndex(int index)
    {
        if (_mapSelectionService == null || index < 0 || index >= _mapSelectionService.Maps.Count)
        {
            return null;
        }

        return _mapSelectionService.Maps[index];
    }

    private void SetLevelName(string text)
    {
        if (levelName != null)
        {
            levelName.text = text;
        }
    }

    private void SetPlayButtonActive(bool isActive)
    {
        if (playButton != null)
        {
            playButton.interactable = isActive;
        }
    }

    // Căn giữa một item bất kỳ
    private void CenterOnItem(RectTransform target, bool instant = false)
    {
        float viewportCenter = scrollRect.viewport.rect.width / 2f;

        // Center item trong content
        float itemCenter = target.localPosition.x;
        float targetX = -itemCenter + viewportCenter;

        // Tính clamp để item đầu và cuối có thể center
        float maxX = -items[0].localPosition.x + viewportCenter; // item cuối cực trái
        float minX = -items[items.Count - 1].localPosition.x + viewportCenter;                             // item đầu cực phải
        targetX = Mathf.Clamp(targetX, minX, maxX);

        if (instant)
        {
            content.localPosition = new Vector3(targetX, content.localPosition.y, 0f);
            UpdateItemsRealtime();
        }
        else
        {
            content.DOKill();
            content.DOLocalMoveX(targetX, snapDuration)
                   .SetEase(Ease.OutQuart)
                   .OnUpdate(UpdateItemsRealtime);
        }
    }

    private void OnDestroy()
    {
        scrollRect?.onValueChanged.RemoveAllListeners();
        content?.DOKill();
    }
}
