using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Inventory
{
    /// <summary>
    /// Handles drag-drop input on an inventory item.
    /// Communicates with DragDropSystem directly instead of EventBus.
    /// Visual movement during drag is handled here (view-only).
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ItemDragHandler : MonoBehaviour,
        IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static event System.Action<ItemModel, InventoryModel> OnItemSelected;

        private ItemView _itemView;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;

        private Vector2 _anchoredPositionOffset;
        private Vector2Int _gridPositionOffset;
        private Vector2 _originalAnchoredPosition;
        
        private Camera _eventCamera;
        private Vector2 _localGrabPoint;
        
        private bool _isDragging;
        private ItemSO.Dir _currentDir;

        public void Setup(ItemView itemView)
        {
            _itemView = itemView;
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Reserved for future use (e.g. item tooltip)
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            if (_itemView?.ItemModel == null || _itemView.Inventory == null) return;
            OnItemSelected?.Invoke(_itemView.ItemModel, _itemView.Inventory);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_itemView?.ItemModel == null || _canvas == null) return;
            if (!_itemView.Inventory.CanRemoveItem(_itemView.ItemModel)) return;

            OnItemSelected?.Invoke(_itemView.ItemModel, _itemView.Inventory);

            _originalAnchoredPosition = _rectTransform.anchoredPosition;
            _eventCamera = eventData.pressEventCamera;

            // 1. Tính toán lệch ô lưới (click vào ô nào của vật phẩm) TRƯỚC KHI chuyển parent
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _itemView.ItemContainer, eventData.position, eventData.pressEventCamera,
                out Vector2 localPointInContainer);

            Vector2Int mouseGridPos = _itemView.Inventory.GetGridPosition(localPointInContainer);
            _gridPositionOffset = mouseGridPos - _itemView.ItemModel.Origin;

            // 2. Chuyển parent tạm thời sang Canvas root để đảm bảo vật phẩm luôn hiển thị nổi trên tất cả các Panel khác
            _rectTransform.SetParent(_canvas.transform, true);
            _rectTransform.SetAsLastSibling();

            // 3. Sau khi reparent, chuẩn hoá anchor về (0.5, 0.5) để anchoredPosition == localPosition.
            //    Nếu không làm bước này, anchoredPosition đo từ anchor corner (không phải canvas center),
            //    còn mouseCanvasLocal đo từ canvas pivot (center) → hai hệ toạ độ khác nhau → item trôi xa.
            //    Dùng localPosition (luôn đo từ parent pivot = canvas center) để tính offset cho nhất quán.
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            // Gán lại pivot về (0,0) để khớp với hệ toạ độ lưới (góc dưới-trái)
            // nhưng giữ world position bằng cách điều chỉnh localPosition thủ công.
            // Thực ra giữ pivot gốc là đơn giản nhất — chỉ cần anchor = center là đủ.

            var parentRect = _canvas.transform as RectTransform;
            if (parentRect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect, eventData.position, eventData.pressEventCamera,
                    out Vector2 mouseCanvasLocal);
                // localPosition đo từ canvas pivot (center) — CÙNG gốc với mouseCanvasLocal → offset chính xác
                _anchoredPositionOffset = mouseCanvasLocal - (Vector2)_rectTransform.localPosition;
            }

            // Tính localGrabPoint bằng cách khử xoay hiện tại để offset không bị ảnh hưởng bởi góc xoay
            _localGrabPoint = Quaternion.Inverse(_rectTransform.localRotation) * _anchoredPositionOffset;

            // Thiết lập trạng thái bắt đầu kéo
            _isDragging = true;
            _currentDir = _itemView.ItemModel.Dir;

            // Visual feedback
            _canvasGroup.alpha = 0.7f;
            _canvasGroup.blocksRaycasts = false;

            // Communicate with DragDropSystem
            if (DragDropSystem.Instance != null)
            {
                DragDropSystem.Instance.HandleDragStart(
                    _itemView.ItemModel,
                    _itemView.Inventory,
                    _anchoredPositionOffset,
                    _gridPositionOffset,
                    _itemView.ItemModel.Dir,
                    this
                );
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Vị trí và góc xoay được cập nhật mượt mà trong Update() nhằm hỗ trợ snap và hiệu ứng xoay (lerp)
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_itemView?.ItemModel == null) return;
            if (!_itemView.Inventory.CanRemoveItem(_itemView.ItemModel)) return;

            // Kết thúc trạng thái kéo
            _isDragging = false;

            // Restore visual feedback
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            // Communicate with DragDropSystem
            if (DragDropSystem.Instance != null)
            {
                DragDropSystem.Instance.HandleDragEnd(
                    _itemView.ItemModel,
                    _itemView.Inventory,
                    eventData.position,
                    eventData.pressEventCamera,
                    _rectTransform // Truyền RectTransform của vật phẩm để tính toán snap theo diện tích bao phủ
                );
            }
        }

        /// <summary>
        /// Visually rotates the RectTransform and animates the transition smoothly, keeping the grabbed point under the mouse.
        /// </summary>
        public void RotateTo(ItemSO.Dir newDir)
        {
            _currentDir = newDir;
        }

        private void Update()
        {
            if (!_isDragging) return;

            // 1. Lấy vị trí chuột trong toạ độ local của parent RectTransform (Canvas)
            var parentRect = _rectTransform.parent as RectTransform;
            if (parentRect == null || _itemView?.ItemModel == null || _canvas == null) return;

            Vector2 mouseScreenPos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, mouseScreenPos, _eventCamera, out Vector2 mouseLocalPoint);

            // 2. Tính toán lại lệch điểm neo pixel dựa theo góc xoay mục tiêu hiện tại (để giữ điểm click nằm dưới con trỏ chuột khi xoay tự do)
            float targetAngle = -_itemView.ItemModel.ItemSO.GetRotationAngle(_currentDir);
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            _anchoredPositionOffset = targetRotation * _localGrabPoint;

            // 3. Quét tìm xem chuột đang hover trên grid của InventoryView nào
            InventoryView hoverView = null;
            Vector2Int candidateOrigin = Vector2Int.zero;

            if (DragDropSystem.Instance != null && DragDropSystem.Instance.InventoryViews != null)
            {
                foreach (var view in DragDropSystem.Instance.InventoryViews)
                {
                    if (view == null || view.Model == null) continue;

                    var container = view.ItemContainer;
                    Camera cam = null;
                    Canvas canvas = container.GetComponentInParent<Canvas>();
                    if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    {
                        cam = Camera.main;
                    }

                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        container, mouseScreenPos, cam, out Vector2 localPoint))
                    {
                        Vector2Int gridPos = view.Model.GetGridPosition(localPoint);
                        if (view.Model.IsValidGridPosition(gridPos))
                        {
                            hoverView = view;
                            candidateOrigin = gridPos - _gridPositionOffset;
                            break;
                        }
                    }
                }
            }

            Vector2 targetAnchoredPosition;

            if (hoverView != null)
            {
                // SNAP TO GRID: Tính toán vị trí tâm vật phẩm khớp hoàn hảo 100% với grid visual preview
                Vector3 itemLocalSnapPos = hoverView.Model.GetItemWorldPosition(
                    candidateOrigin, _currentDir, _itemView.ItemModel.ItemSO);

                // Chuyển đổi vị trí snap từ hệ toạ độ local của ItemContainer sang hệ toạ độ local của parentRect (Canvas)
                Vector3 worldSnapPos = hoverView.ItemContainer.TransformPoint(itemLocalSnapPos);
                targetAnchoredPosition = parentRect.InverseTransformPoint(worldSnapPos);
            }
            else
            {
                // FOLLOW MOUSE: Trôi tự do theo con trỏ chuột
                targetAnchoredPosition = mouseLocalPoint - _anchoredPositionOffset;
            }

            // 4. Nội suy mượt mà (lerp) vị trí và góc xoay độc lập với tốc độ khung hình (unscaled delta time)
            _rectTransform.anchoredPosition = Vector2.Lerp(
                _rectTransform.anchoredPosition,
                targetAnchoredPosition,
                1f - Mathf.Exp(-25f * Time.unscaledDeltaTime)
            );

            _rectTransform.localRotation = Quaternion.Slerp(
                _rectTransform.localRotation,
                targetRotation,
                1f - Mathf.Exp(-20f * Time.unscaledDeltaTime)
            );
        }
    }
}
