
public abstract class Plant : PlacableObject
{
    private float attackRange;

    public abstract void Fuse(Plant otherPlant);
    public abstract void Idle();
}