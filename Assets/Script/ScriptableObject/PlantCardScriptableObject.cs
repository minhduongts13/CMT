using UnityEngine;

[CreateAssetMenu(fileName = "PlantCard", menuName = "Scriptable Objects/PlantCard")]
public class PlantCardScriptableObject : Almanac_Object
{
    [Header("Plant Specific")]
    [SerializeField] private GameObject _plantPrefab;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private LayerMask _zombieLayer;

    // Plant-specific properties
    public GameObject PlantPrefab => _plantPrefab;
    public GameObject Bullet => _bullet;
    public LayerMask ZombieLayer => _zombieLayer;
    public float AttackRange => Range;
    public float FireRate => ActionRate;
    public float SpeedBullet => Speed;
}