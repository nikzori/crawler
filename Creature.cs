public class Creature : IDamageable
{
    //const int BASE_MOVESPEED = 10;
    const int BASE_HEALTH = 3;
    const int BASE_SIGHTRADIUS = 10;
    const int FACTION_PLAYER = 0;
    const int FACTION_DUNGEON = 1;
    public int Faction { get; set; }

    public string Name;
    public float Health { get; set; }
    public int MaxHealth { get; set; }
    public int SightRadius { get; set; }
    public Vector2Int Pos { get; set; }

    public List<Item> inventory = new();


    public Creature(string name, Vector2Int pos, int maxHealth = BASE_HEALTH, int sightRadius = BASE_SIGHTRADIUS, int faction = FACTION_DUNGEON)
    {
        this.Name = name;
        this.Pos = pos;
        this.MaxHealth = maxHealth;
        this.Health = MaxHealth;
        this.SightRadius = sightRadius;

        this.Faction = faction;
    }

    // Shorthand for normalized vectors
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
        Game.CurrentMap.cells[this.Pos].RemoveCreature();
        this.Pos = pos;
        Game.CurrentMap.cells[this.Pos].AddCreature(this);
        return true;
    }

    public void ReceiveDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            OnCreatureDeath();
    }

    void OnCreatureDeath() { Game.CurrentMap.creatures.Remove(this); Game.CurrentMap.cells[Pos]?.RemoveCreature(); }


}

public interface IDamageable
{
    float Health { get; set; }
    void ReceiveDamage(float damage);
}
