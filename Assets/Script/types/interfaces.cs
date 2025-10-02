
public interface IMovable
{
    // private float speed;
    public void MoveTo(int x, int y);
    public void SetSpeed(float speed);
    public float GetCurrentSpeed();
    public bool CheckCollision();
}

public interface IDamageable
{
    public void TakeDamage(int amount);
    public void GetCurrentHp();
    public void Destroyed();
}