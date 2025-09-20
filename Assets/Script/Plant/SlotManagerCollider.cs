using UnityEngine;

// Trong SlotManagerCollider, thêm enum
public enum SlotType
{
    PlantSlot,
    ZombieSlot
}

public class SlotManagerCollider : MonoBehaviour
{
    [Header("Slot Settings")]
    public SlotType slotType;
    public bool isEmpty = true;

    public bool CanPlaceObject(GameObject obj)
    {
        // SỬA: Chỉ check isEmpty cho PlantSlot
        if (slotType == SlotType.PlantSlot && !isEmpty) return false;

        // Kiểm tra object có phù hợp với slot type không
        if (slotType == SlotType.PlantSlot)
        {
            return obj.GetComponent<PlantManager>() != null;
        }
        else if (slotType == SlotType.ZombieSlot)
        {
            // Zombie slot LUÔN cho phép đặt (không check isEmpty)
            return obj.GetComponent<ZombieController>() != null;
        }

        return false;
    }

    [Header("Row Settings")]
    public int slotRow = 0;

    public bool PlacePlant(GameObject plant)
    {
        if (slotType != SlotType.PlantSlot || !isEmpty) return false;

        plant.transform.position = transform.position;
        plant.transform.SetParent(transform);
        isEmpty = false;

        // SET ROW cho plant
        PlantManager plantManager = plant.GetComponent<PlantManager>();
        if (plantManager != null)
        {
            plantManager.SetPlantRow(slotRow);
        }

        return true;
    }

    public bool PlaceZombie(GameObject zombie)
    {
        // SỬA: Bỏ check isEmpty cho zombie slot
        if (slotType != SlotType.ZombieSlot) return false;

        zombie.transform.position = transform.position;
        zombie.transform.SetParent(transform);

        // SET ROW cho zombie
        ZombieController zombieController = zombie.GetComponent<ZombieController>();
        if (zombieController != null)
        {
            zombieController.SetZombieRow(slotRow);
        }

        return true;
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }
}