public class Stair : GameObject, IInteractable
{
    bool singleUse;
    StairDirection direction = StairDirection.Down;

    public Stair((int x, int y) pos, StairDirection direction, bool singleUse = false) : base(pos)
    {
        this.pos = pos;
        this.singleUse = singleUse;
        this.direction = direction;
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
        if (direction == StairDirection.Down)
            Game.Descend();
        else Game.Ascend();
    }
}

public interface IInteractable
{
    abstract void OnInteract(object? sender, InteractEventArgs e);
}
public enum StairDirection : byte { Down, Up };