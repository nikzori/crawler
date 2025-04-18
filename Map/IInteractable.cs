public class Stairs : GameObject, IInteractable
{
    bool singleUse;
    StairDirection direction = StairDirection.Down;
    Stairs? linkedStairs;

    public Stairs((int x, int y) pos, StairDirection direction, Stairs linkedStairs = null, bool singleUse = false) : base(pos)
    {
        this.pos = pos;
        this.singleUse = singleUse;
        this.direction = direction;
        this.linkedStairs = linkedStairs;
        if (direction == StairDirection.Up)
            this.rune = new('<');
        else this.rune = new('>');

        UI.Interact += OnInteract;
        
    }

    public void OnInteract(object? sender, InteractEventArgs e)
    {
        if (e.pos != this.pos)
            return;

        UI.Log("Stairs activated");
        try
        {
            if (direction == StairDirection.Down)
                Game.Descend(linkedStairs.pos);
            else Game.Ascend(linkedStairs.pos);
        }
        catch { UI.Log("These stairs are not connected to stairs on a level below. This is a bug."); }
    }

    public void SetLinkedStairs (Stairs linkedStairs)
    {
        this.linkedStairs = linkedStairs;
    }
}

public interface IInteractable
{
    abstract void OnInteract(object? sender, InteractEventArgs e);
}
public enum StairDirection : byte { Down, Up };