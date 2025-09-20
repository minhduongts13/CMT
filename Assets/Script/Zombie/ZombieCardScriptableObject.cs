using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Cards/Zombie Card", fileName = "NewZombieCard")]
public class ZombieCardScriptableObject : ScriptableObject
{
    public Sprite zombieIcon;
    public int cost;
    public float cooldown;
    public float health;
    public float damage;
    public float attackRange;
    public float moveSpeed;
    public float attackRate;
    public GameObject zombiePrefab;
}
