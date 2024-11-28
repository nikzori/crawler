public class Stair : GameObject, IInteractable
{
    bool singleUse;


    public Stair((int x, int y) pos, bool singleUse = false) : base(pos)
    {
        this.pos = pos;
        this.singleUse = singleUse;
    }

    public void Interact()
    {
        
    }
}

public interface IInteractable
{
    abstract void Interact();
}