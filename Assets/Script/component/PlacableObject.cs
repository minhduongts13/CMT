
public abstract class PlacableObject
{
    private float damage;
    private float cost;
    private string name;
    private UnityEngine.Vector2 position;
    private string state;
    private float attackSpeed;
    private string owner;
    private float spawnCooldown;
    private bool isAvailable;

    public abstract void Attack();
    public abstract void Spawn();
    public abstract void Skill();
    public abstract void Special(); 
}