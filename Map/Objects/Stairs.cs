using System.Text;

public class Stairs : IInteractable
{
    public StairsDirection Direction { get; }
    public Rune Rune { get; }
    public Stairs(StairsDirection direction)
    {
        this.Direction = direction;
        if (direction == StairsDirection.Up)
            Rune = new('<');
        else Rune = new('>');
    }

    public void OnInteract(object? sender, Vector2Int pos)
    {
        Random rng = new();
        Vector2Int newPos;
        int newFloorIndex;
        if (Direction == StairsDirection.Up)
            newFloorIndex = Game.dungeon.currentFloor - 1;
        else newFloorIndex = Game.dungeon.currentFloor + 1;

        while (true)
        {
            Map otherMap = Game.dungeon.floors[newFloorIndex];
            newPos = new(rng.Next(1, otherMap.size.X), rng.Next(1, otherMap.size.Y));
            if (otherMap.cells.ContainsKey(newPos) && otherMap.cells[newPos].IsWalkable)
                break;
        }
        Game.ChangeFloor(newFloorIndex, newPos);
    }
}

public enum StairsDirection { Up, Down };
