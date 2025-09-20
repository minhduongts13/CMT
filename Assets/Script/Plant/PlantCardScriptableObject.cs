using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Cards/Plant Card", fileName = "NewPlantCard")]
public class PlantCardScriptableObject : ScriptableObject
{
    public Sprite plantIcon;
    public int cost;
    public float cooldown;
    public float health;
    public float damage;
    public float range;
    public GameObject bullet;
    public float bulletSpeed;
    public float fireRate;
    public LayerMask zombieLayer;
    public GameObject plantPrefab;
}
