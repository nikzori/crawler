public class Creature : IDamageable
{
    const int BASE_MOVESPEED = 10;
    const int BASE_HEALTH = 3;

    public string Name;
    public float Health { get; set; }
    public float MoveSpeed { get; }
    public float Damage { get; }
    public int SightRadius { get; set; }
    public Vector2Int Pos { get; set; }

    public List<Item> inventory = new();


    public Creature(string name, Vector2Int pos)
    {
        this.Name = name;
        this.Pos = pos;
        this.MoveSpeed = BASE_MOVESPEED;
    }

    // Shorthand for single vectors
    public bool Move(Vector2Int dir) { return MoveTo(this.Pos + dir); }

    public bool MoveTo(Vector2Int pos)
    {
        if (!Game.dungeon.GetCurrentFloor().cells.ContainsKey(pos))
        {
            Game.Log("Trying to move creature out of map boundaries; object stays at x: " + this.Pos.X + "; y: " + this.Pos.Y);
            return false;
        }
        if (!Game.dungeon.GetCurrentFloor().cells[pos].IsWalkable || Game.dungeon.GetCurrentFloor().cells[pos].HasCreature())
            return false;
        Game.currentMap.cells[this.Pos].RemoveCreature();
        this.Pos = pos;
        Game.currentMap.cells[this.Pos].AddCreature(this);
        return true;
    }

    public void ReceiveDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            OnCreatureDeath();
    }

    void OnCreatureDeath() { Game.currentMap.creatures.Remove(this); }


}

public interface IDamageable
{
    float Health { get; set; }
    void ReceiveDamage(float damage);
}
