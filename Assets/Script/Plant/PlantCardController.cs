using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlantCardController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    [Header("Card Data")]
    public PlantCardScriptableObject myPlantCardSO;
    [Header("Plant Prefab")]
    // public GameObject plantPrefab;

    private GameObject draggingPlantInstance;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        draggingPlantInstance = Instantiate(myPlantCardSO.plantPrefab, worldPos, Quaternion.identity);

        Image iconImage = transform.Find("Plant Icon").GetComponent<Image>();
        SpriteRenderer sr = draggingPlantInstance.GetComponent<SpriteRenderer>();
        if (iconImage != null && sr != null)
        {
            sr.sprite = iconImage.sprite;

            // Chuẩn hóa kích thước cây
            float targetHeight = 3.5f;
            float spriteHeight = sr.sprite.bounds.size.y;
            float scale = targetHeight / spriteHeight;
            draggingPlantInstance.transform.localScale = new Vector3(scale, scale, 1);
            PlantManager pm = draggingPlantInstance.GetComponent<PlantManager>();
            pm.plantCardSO = myPlantCardSO;
            pm.isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingPlantInstance != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            draggingPlantInstance.transform.position = worldPos;
        }
    }

    // Sửa hàm FindNearestSlot trong PlantCardController
    private SlotManagerCollider FindNearestSlot()
    {
        SlotManagerCollider[] allSlots = FindObjectsByType<SlotManagerCollider>(FindObjectsSortMode.None);
        SlotManagerCollider nearestSlot = null;
        float nearestDistance = float.MaxValue;
        float maxAllowedDistance = 20.0f; // Khoảng cách tối đa cho phép đặt

        foreach (SlotManagerCollider slot in allSlots)
        {
            // CHỈ tìm plant slots
            if (slot.slotType != SlotType.PlantSlot) continue;

            // Bỏ qua slot đã có object
            if (!slot.IsEmpty()) continue;

            var dragPlantRect = draggingPlantInstance.GetComponent<RectTransform>();
            var slotRect = slot.GetComponent<RectTransform>();
            // float xDistance = dragPlantRect.position.x - slotRect.position.x;
            // float yDistance = dragPlantRect.position.y - slotRect.position.y;
            // Debug.Log($"X Distance to slot {slot.name}: {xDistance}, Y Distance: {yDistance}");
            // Debug.Log($"Slot Width: {slotRect.rect.width}, Slot Height: {slotRect.rect.height}");
            float distance = Vector2.Distance(dragPlantRect.position, slotRect.position);

            // CHỈ xét các slot trong phạm vi cho phép
            if (distance <= maxAllowedDistance && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSlot = slot;
            }
            // if (Mathf.Abs(xDistance) <= slotRect.rect.width / 2 && Mathf.Abs(yDistance) <= slotRect.rect.height / 2)
            // {
            //     nearestSlot = slot;
            //     return nearestSlot;
            // }

        }
        Debug.Log($"Distance to slot {nearestSlot.name}: {nearestDistance}");

        return nearestSlot;
    }

    // Sửa OnEndDrag để kiểm tra chặt chẽ hơn
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = originalPosition;

        SlotManagerCollider nearestSlot = FindNearestSlot();

        // CHỈ đặt nếu tìm được slot hợp lệ VÀ có thể đặt object
        if (nearestSlot != null &&
            nearestSlot.CanPlaceObject(draggingPlantInstance) &&
            nearestSlot.IsEmpty())
        {
            // Kiểm tra khoảng cách một lần nữa để chắc chắn
            float finalDistance = Vector2.Distance(draggingPlantInstance.transform.position, nearestSlot.transform.position);
            if (finalDistance <= 20.0f) // Cùng giá trị maxAllowedDistance

            {
                if (nearestSlot.PlacePlant(draggingPlantInstance))
                {
                    SpriteRenderer sr = draggingPlantInstance.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color plantColor = sr.color;
                        plantColor.a = 1f;
                        sr.color = plantColor;
                    }

                    draggingPlantInstance.GetComponent<PlantManager>().isDragging = false;
                    draggingPlantInstance = null;
                    Debug.Log("Plant placed successfully!");
                    return;
                }
            }
        }

        // Nếu KHÔNG thỏa mãn bất kỳ điều kiện nào → XÓA plant
        if (draggingPlantInstance != null)
        {
            Debug.Log("Cannot place plant - destroying instance");
            Destroy(draggingPlantInstance);
            draggingPlantInstance = null;
        }
    }
}