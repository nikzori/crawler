public class Stair : GameObject, IInteractable
{
    bool singleUse;
    StairDirection direction = StairDirection.Down;


    public Stair((int x, int y) pos, StairDirection direction, bool singleUse = false) : base(pos)
    {
        this.pos = pos;
        this.singleUse = singleUse;
        this.direction = direction;
    }

    public void Interact()
    {
        Game.Descend();
    }
}

public interface IInteractable
{
    abstract void Interact();
}
public enum StairDirection : byte { Down, Up };