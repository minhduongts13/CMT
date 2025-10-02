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
        // Cleanup previous instance nếu có
        CleanupDraggingInstance();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        // NULL CHECK cho myPlantCardSO
        if (myPlantCardSO?.PlantPrefab == null)
        {
            Debug.LogError("[CARD] PlantPrefab is null!");
            return;
        }

        draggingPlantInstance = Instantiate(myPlantCardSO.PlantPrefab, worldPos, Quaternion.identity);

        // NULL CHECK sau khi instantiate
        if (draggingPlantInstance == null)
        {
            Debug.LogError("[CARD] Failed to instantiate plant!");
            return;
        }

        SetupDraggingPlant();
    }

    private void SetupDraggingPlant()
    {
        if (draggingPlantInstance == null) return;

        // Find Body sprite renderer để scale
        Transform bodyTransform = draggingPlantInstance.transform.Find("Body");
        if (bodyTransform != null)
        {
            SpriteRenderer sr = bodyTransform.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                // Chuẩn hóa kích thước cây
                float targetHeight = 3.5f;
                float spriteHeight = sr.sprite.bounds.size.y;
                float scale = targetHeight / spriteHeight;
                draggingPlantInstance.transform.localScale = new Vector3(scale, scale, 1);
            }
        }

        // SETUP CHỈ PLANT COMPONENT
        Plant plantComponent = draggingPlantInstance.GetComponent<Plant>();
        if (plantComponent != null && myPlantCardSO != null)
        {
            plantComponent.SetPlantCardSO(myPlantCardSO);
            plantComponent.SetDragging(true);
            Debug.Log($"[CARD] Plant component setup with SO: {myPlantCardSO.name}");

            // Validate bullet assignment
            if (plantComponent.Bullet != null)
            {
                Debug.Log($"[CARD] Bullet assigned: {plantComponent.Bullet.name}");
            }
            else
            {
                Debug.LogWarning("[CARD] No bullet assigned to Plant component!");
            }
        }
        else
        {
            Debug.LogError("[CARD] Plant component or myPlantCardSO is null!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingPlantInstance == null) return;

        try
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            if (draggingPlantInstance != null && draggingPlantInstance.transform != null)
            {
                draggingPlantInstance.transform.position = worldPos;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CARD] Error during drag: {e.Message}");
            CleanupDraggingInstance();
        }
    }

    private SlotManagerCollider FindNearestSlot()
    {
        if (draggingPlantInstance == null || draggingPlantInstance.transform == null)
        {
            Debug.LogWarning("[CARD] draggingPlantInstance is null in FindNearestSlot");
            return null;
        }

        SlotManagerCollider[] allSlots = FindObjectsByType<SlotManagerCollider>(FindObjectsSortMode.None);
        SlotManagerCollider nearestSlot = null;
        float nearestDistance = float.MaxValue;
        float maxAllowedDistance = 2.0f;

        Vector3 plantPosition = draggingPlantInstance.transform.position;

        foreach (SlotManagerCollider slot in allSlots)
        {
            if (slot == null || slot.transform == null) continue;
            if (slot.slotType != SlotType.PlantSlot) continue;
            if (!slot.IsEmpty()) continue;

            float distance = Vector2.Distance(plantPosition, slot.transform.position);

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

        if (draggingPlantInstance == null)
        {
            Debug.LogWarning("[CARD] draggingPlantInstance is null in OnEndDrag");
            return;
        }

        try
        {
            SlotManagerCollider nearestSlot = FindNearestSlot();

            if (nearestSlot != null &&
                nearestSlot.CanPlaceObject(draggingPlantInstance) &&
                nearestSlot.IsEmpty())
            {
                float finalDistance = Vector2.Distance(
                    draggingPlantInstance.transform.position,
                    nearestSlot.transform.position
                );

                if (finalDistance <= 2.0f)
                {
                    if (TryPlacePlant(nearestSlot))
                    {
                        Debug.Log("[CARD] Plant placed successfully!");
                        return; // Success
                    }
                }
            }

            Debug.Log("[CARD] Cannot place plant - destroying instance");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CARD] Error in OnEndDrag: {e.Message}");
        }

        CleanupDraggingInstance();
    }

    private bool TryPlacePlant(SlotManagerCollider slot)
    {
        if (slot == null || draggingPlantInstance == null) return false;

        if (slot.PlacePlant(draggingPlantInstance))
        {
            // Make plant visible
            SpriteRenderer sr = draggingPlantInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color plantColor = sr.color;
                plantColor.a = 1f;
                sr.color = plantColor;
            }

            // CHỈ SETUP PLANT COMPONENT
            Plant plantComponent = draggingPlantInstance.GetComponent<Plant>();

            if (plantComponent != null)
            {
                // ENSURE PlantCardSO is set
                if (plantComponent.PlantCardSO == null && myPlantCardSO != null)
                {
                    Debug.Log("[CARD] Setting PlantCardSO before spawn");
                    plantComponent.SetPlantCardSO(myPlantCardSO);
                }

                // SET ROW
                plantComponent.SetPlantRow(slot.slotRow);
                Debug.Log($"[CARD] Plant row set to: {slot.slotRow}");

                // STOP DRAGGING
                plantComponent.SetDragging(false);

                // VALIDATION
                Debug.Log($"[CARD] Plant validation:");
                Debug.Log($"  - Health: {plantComponent.Health}");
                Debug.Log($"  - Row: {plantComponent.CurrentRow}");
                Debug.Log($"  - CanShoot: {plantComponent.CanShoot}");
                Debug.Log($"  - Bullet: {plantComponent.Bullet?.name ?? "NULL"}");
                Debug.Log($"  - AttackRange: {plantComponent.AttackRange}");
                Debug.Log($"  - AttackRate: {plantComponent.AttackRate}");

                if (plantComponent.Health <= 0)
                {
                    Debug.LogError("[CARD] Plant has 0 health!");
                    return false;
                }

                if (plantComponent.Bullet == null)
                {
                    Debug.LogError("[CARD] Plant has no bullet! Cannot attack!");
                }

                // SPAWN PLANT - triggers OnPlantInitialized
                plantComponent.Spawn();
                Debug.Log("[CARD] Plant.Spawn() called - should trigger initialization");
            }
            else
            {
                Debug.LogError("[CARD] No Plant component found on dragging instance!");
                return false;
            }

            // CLEAR REFERENCE
            draggingPlantInstance = null;
            return true;
        }

        Debug.LogError("[CARD] slot.PlacePlant() returned false!");
        return false;
    }

    private void CleanupDraggingInstance()
    {
        if (draggingPlantInstance != null)
        {
            try
            {
                Debug.Log("[CARD] Cleaning up dragging instance");
                Destroy(draggingPlantInstance);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CARD] Error destroying dragging instance: {e.Message}");
            }
            finally
            {
                draggingPlantInstance = null;
            }
        }
    }

    // Safety cleanup
    private void OnDestroy()
    {
        CleanupDraggingInstance();
    }

    private void OnDisable()
    {
        CleanupDraggingInstance();
    }

    // Debug validation
    private void Update()
    {
        // Check nếu dragging instance bị destroy externally
        if (draggingPlantInstance != null && draggingPlantInstance.Equals(null))
        {
            Debug.LogWarning("[CARD] Dragging instance was destroyed externally");
            draggingPlantInstance = null;
        }
    }
}