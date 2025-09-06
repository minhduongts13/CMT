using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject[] mapPrefabs;   // Danh sách prefab map
    private GameObject currentMap;    // Map hiện tại

    // Hàm này được gọi từ button, truyền index vào
    public void LoadMapByIndex(int index)
    {
        if (index < 0 || index >= mapPrefabs.Length)
        {
            Debug.LogWarning("Index map không hợp lệ: " + index);
            return;
        }

        // Xóa map cũ nếu có
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        // Spawn map mới
        currentMap = Instantiate(mapPrefabs[index]);
        currentMap.name = "LoadedMap_" + index;
    }
}
