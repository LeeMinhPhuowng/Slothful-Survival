using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class CoverFlow : MonoBehaviour
{
    [Header("Scroll Setup")]
    public ScrollRect scrollRect;          // ScrollRect chứa content
    public float spacing = 300f;           // Khoảng cách giữa các item

    [Header("CoverFlow Settings")]
    public float scaleFactor = 0.5f;       // Scale nhỏ nhất của item xa center
    public float rotationFactor = 25f;     // Góc xoay max của item xa center
    public float fadeFactor = 0.5f;        // Alpha nhỏ nhất của item xa center
    public float snapDuration = 0.5f;      // Thời gian tween khi snap

    [Header("LevelSO")]
    [SerializeField] List<LevelSO> levels = new List<LevelSO>();

    private List<RectTransform> items = new List<RectTransform>();
    private RectTransform content;
    private int currentItemIndex = 0;
    private bool isDragging = false;

    public static CoverFlow instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        content = scrollRect.content;

        // Lấy tất cả child làm item
        foreach (Transform child in content)
            items.Add(child as RectTransform);

        // Sắp xếp item theo spacing
        for (int i = 0; i < items.Count; i++)
            items[i].anchoredPosition = new Vector2(i * spacing, 0);

        // Đảm bảo content đủ rộng để snap item cuối
        float contentWidth = (items.Count - 1) * spacing + scrollRect.viewport.rect.width;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);

        scrollRect.horizontal = true;
        scrollRect.vertical = false;
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

        // Cập nhật realtime khi kéo
        scrollRect.onValueChanged.AddListener(_ => UpdateItemsRealtime());

        // Căn giữa item đầu tiên
        CenterOnItem(items[0], true);
    }

    public void OnBeginDrag()
    {
        isDragging = true;
        content.DOKill();
    }

    public void OnEndDrag()
    {
        isDragging = false;
        scrollRect.StopMovement();
        SnapToNearestItem();
    }

    private void UpdateItemsRealtime()
    {
        float viewportCenter = scrollRect.viewport.rect.width / 2f;

        foreach (RectTransform item in items)
        {
            float itemCenter = item.localPosition.x + content.localPosition.x;
            float distance = itemCenter - viewportCenter;
            float normalized = Mathf.Clamp01(Mathf.Abs(distance) / spacing);

            float scale = Mathf.Lerp(scaleFactor, 1f, 1f - normalized);
            float angle = Mathf.Clamp(rotationFactor * (distance / spacing), -rotationFactor, rotationFactor);
            float alpha = Mathf.Lerp(fadeFactor, 1f, 1f - normalized);

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

        if (nearest != null)
            CenterOnItem(nearest, false);
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

    public LevelSO GetLevelSO()
    {
        return levels[currentItemIndex];
    }   
}
