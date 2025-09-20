
public abstract class Zombie : PlacableObject
{
    public abstract void MergeWith(Zombie other);
    public abstract void Upgrade();
}