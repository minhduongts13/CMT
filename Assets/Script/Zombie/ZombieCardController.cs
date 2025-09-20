using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZombieCardController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Zombie Card Settings")]
    public ZombieCardScriptableObject myZombieCardSO;

    [Header("Drag Settings")]
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private GameObject draggingZombieInstance;

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

        draggingZombieInstance = Instantiate(myZombieCardSO.zombiePrefab, worldPos, Quaternion.identity);

        // Setup zombie
        Image iconImage = transform.Find("Zombie Icon").GetComponent<Image>();
        SpriteRenderer sr = draggingZombieInstance.GetComponent<SpriteRenderer>();
        if (iconImage != null && sr != null)
        {
            sr.sprite = iconImage.sprite;

            // TĂNG SIZE cho zombie (từ 3.5f lên 6.5f)
            float targetHeight = 6.5f;
            float spriteHeight = sr.sprite.bounds.size.y;
            float scale = targetHeight / spriteHeight;
            draggingZombieInstance.transform.localScale = new Vector3(scale, scale, 1);

            // TẮT HOẠT ĐỘNG khi đang kéo
            ZombieController zm = draggingZombieInstance.GetComponent<ZombieController>();
            if (zm != null)
            {
                zm.zombieCardSO = myZombieCardSO;
                zm.isDragging = true;
                zm.enabled = false; // TẮT script để zombie không hoạt động
            }

            // TẮT movement component nếu có
            Rigidbody2D rb = draggingZombieInstance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic; // Không bị physics ảnh hưởng
            }

            // Làm mờ zombie khi đang kéo
            Color zombieColor = sr.color;
            zombieColor.a = 0.6f;
            sr.color = zombieColor;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingZombieInstance != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            draggingZombieInstance.transform.position = worldPos;
        }
    }

    private SlotManagerCollider FindNearestZombieSlot()
    {
        SlotManagerCollider[] allSlots = FindObjectsByType<SlotManagerCollider>(FindObjectsSortMode.None);
        SlotManagerCollider nearestSlot = null;
        float nearestDistance = float.MaxValue;
        float maxAllowedDistance = 3.0f;

        foreach (SlotManagerCollider slot in allSlots)
        {
            // CHỈ tìm zombie slots
            if (slot.slotType != SlotType.ZombieSlot) continue;

            // BỎ CHECK IsEmpty() - zombie slot luôn cho phép đặt thêm
            // if (!slot.IsEmpty()) continue; // XÓA DÒNG NÀY

            float distance = Vector2.Distance(draggingZombieInstance.transform.position, slot.transform.position);

            if (distance <= maxAllowedDistance && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSlot = slot;
            }
        }

        return nearestSlot;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = originalPosition;

        SlotManagerCollider nearestSlot = FindNearestZombieSlot();

        // SỬA: Bỏ check IsEmpty() cho zombie slot
        if (nearestSlot != null && nearestSlot.CanPlaceObject(draggingZombieInstance))
        {
            float finalDistance = Vector2.Distance(draggingZombieInstance.transform.position, nearestSlot.transform.position);
            Debug.Log($"Final distance check: {finalDistance}");

            if (finalDistance <= 3.0f)
            {
                if (nearestSlot.PlaceZombie(draggingZombieInstance))
                {
                    // KHÔI PHỤC zombie khi đặt thành công
                    SpriteRenderer sr = draggingZombieInstance.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color zombieColor = sr.color;
                        zombieColor.a = 1f;
                        sr.color = zombieColor;
                    }

                    ZombieController zm = draggingZombieInstance.GetComponent<ZombieController>();
                    if (zm != null)
                    {
                        zm.isDragging = false;
                        zm.enabled = true;
                    }

                    draggingZombieInstance = null;
                    Debug.Log("Zombie placed successfully!");
                    return;
                }
            }
        }

        // Xóa zombie nếu không đặt được
        if (draggingZombieInstance != null)
        {
            Debug.Log("Cannot place zombie - destroying instance");
            Destroy(draggingZombieInstance);
            draggingZombieInstance = null;
        }
    }
}