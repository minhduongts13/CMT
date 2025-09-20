using System.Collections.Generic;
using UnityEngine;

public class RowOfZombie : MonoBehaviour
{
    public static RowOfZombie Instance;

    [Header("Row Settings")]
    public int totalRows = 5;

    // Dictionary lưu danh sách zombie theo từng row
    private Dictionary<int, LinkedList<ZombieController>> zombiesByRow = new Dictionary<int, LinkedList<ZombieController>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeRows();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeRows()
    {
        for (int i = 0; i < totalRows; i++)
        {
            zombiesByRow[i] = new LinkedList<ZombieController>();
        }
    }

    // Thêm zombie vào row
    public void AddZombieToRow(int row, ZombieController zombie)
    {
        if (zombiesByRow.ContainsKey(row))
        {
            // THÊM VÀO CUỐI LIST (không sắp xếp)
            zombiesByRow[row].AddLast(zombie);

            zombie.currentRow = row;
            Debug.Log($"Added zombie to row {row} at end of list. Total in row: {zombiesByRow[row].Count}");
        }
    }

    // Xóa zombie khỏi row
    public void RemoveZombieFromRow(int row, ZombieController zombie)
    {
        if (zombiesByRow.ContainsKey(row))
        {
            zombiesByRow[row].Remove(zombie);
            Debug.Log($"Removed zombie from row {row}");
        }
    }

    // Lấy zombie đầu tiên (gần plant nhất) trong row
    public ZombieController GetFirstZombieInRow(int row)
    {
        if (!zombiesByRow.ContainsKey(row) || zombiesByRow[row].Count == 0)
            return null;

        // TÌM zombie có X nhỏ nhất (gần plant nhất) trong thời gian thực
        ZombieController nearestZombie = null;
        float minX = float.MaxValue;

        foreach (var zombie in zombiesByRow[row])
        {
            if (zombie != null && !zombie.isDragging && !zombie.isDead)
            {
                float zombieX = zombie.transform.position.x;
                if (zombieX < minX)
                {
                    minX = zombieX;
                    nearestZombie = zombie;
                }
            }
        }

        return nearestZombie;
    }

    public bool HasZombieInRow(int row)
    {
        return zombiesByRow.ContainsKey(row) && zombiesByRow[row].Count > 0;
    }

    // Lấy số lượng zombie trong row
    public int GetZombieCountInRow(int row)
    {
        return zombiesByRow.ContainsKey(row) ? zombiesByRow[row].Count : 0;
    }
}