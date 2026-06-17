
// should probably be refactored into a struct with creatures and objects being tracked separately
public class Cell
{
    public CellType Type { get; set; }
    public bool IsTransparent = true;
    public bool IsWalkable = true;

    public bool IsRevealed = false;

    public Creature? creature;
    public Dictionary<Item, int>? Items;
    public Cell(CellType type, bool isTransparent, bool isWalkable)
    {
        this.Type = type;
        this.IsTransparent = isTransparent;
        this.IsWalkable = isWalkable;
    }
    public Cell(CellType type)
    {
        this.Type = type;
        switch (Type)
        {
            case CellType.Floor:
                this.IsTransparent = true;
                this.IsWalkable = true;
                break;
            case CellType.Wall:
                this.IsTransparent = false;
                this.IsWalkable = false;
                break;
            default: goto case CellType.Floor;
        }
    }
    public void AddCreature(Creature creature)
    {
        this.creature = creature;
    }
    public void RemoveCreature()
    {
        creature = null;
    }

    public bool HasCreature() { return !(creature is null); }

    public void Set(CellType type, bool isWalkable, bool isTransparent)
    {
        this.Type = type;
        this.IsWalkable = isWalkable;
        this.IsTransparent = isTransparent;
    }
    public void SetToWall() { this.Set(CellType.Wall, false, false); }
    public void SetToFloor() { this.Set(CellType.Floor, true, true); }
    public void SetRevealed(bool value) { this.IsRevealed = value; }
    public void AddItems(Dictionary<Item, int> _items) 
    {
        if (Items is null)
            Items = new();

        foreach (KeyValuePair<Item, int> kvp in _items) 
        {
            if (kvp.Value <= 0)
                continue;

            if (Items.ContainsKey(kvp.Key))
                Items[kvp.Key] += kvp.Value;
            else Items.Add(kvp.Key, kvp.Value);
        }
    }
}
public enum CellType { Floor, Wall, StairsUp, StairsDown, Custom }

public class InteractableCell : Cell, IInteractable 
{
  public Action<object> OnInteract {get;}
  public InteractableCell(Action<object> customFunction, CellType cellType = CellType.Custom) : base(cellType, true, true)
  {
    this.Type = cellType; 
    OnInteract = customFunction;
  }

}

public interface IInteractable
{
  public Action<object> OnInteract {get;}
}
