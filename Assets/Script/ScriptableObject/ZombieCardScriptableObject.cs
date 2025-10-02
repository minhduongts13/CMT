using UnityEngine;

[CreateAssetMenu(fileName = "ZombieCard", menuName = "Scriptable Objects/ZombieCard")]
public class ZombieCardScriptableObject : Almanac_Object
{
    [Header("Zombie Specific")]
    [SerializeField] private GameObject _zombiePrefab;

    // Zombie-specific properties
    public GameObject ZombiePrefab => _zombiePrefab;
    public float AttackRange => Range;
    public float AttackRate => ActionRate;
    public float MoveSpeed => Speed;
}