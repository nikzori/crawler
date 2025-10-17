using System.Text;

public class Creature
{
    public static Rune remains = new('\u223B'); // to be shown on decomposition
    public string name;
    public float health = 10f;
    public int level = 1;
    public Vector2Int pos;

    Rune _rune;
    public Rune rune { get; set; } // gonna need this for status effects and such

    public int aut = 0;

    // AI stuff
    public AIState state = AIState.idle;
    public bool isTrackingPlayer = false;
    public Vector2Int lastPlayerPosition;
    public Queue<Vector2Int>? currentPath;

    public Creature(string name, Vector2Int pos, Rune rune)
    {
        this.name = name;
        this.pos = pos;
        this.rune = rune;
    }

    // Shorthand for single vectors
    public bool Move(Vector2Int dir)
    {
        Vector2Int dPos = this.pos + dir;
        return MoveTo(dPos);
    }

    public bool MoveTo(Vector2Int pos)
    {
        if (!Game.dungeon.GetCurrentFloor().cells.ContainsKey(pos))
        {
            UI.Log("Trying to move creature out of map boundaries; object stays at x: " + this.pos.X + "; y: " + this.pos.Y);
            return false;
        }
        if (!Game.dungeon.GetCurrentFloor().cells[pos].IsWalkable() || Game.dungeon.GetCurrentFloor().cells[pos].HasCreature())
            return false;
        Game.currentMap.cells[this.pos].RemoveCreature();
        this.pos = pos;
        Game.currentMap.cells[this.pos].AddCreature(this);
        return true;
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            OnCreatureDeath();
    }

    void OnCreatureDeath()
    {
        Game.currentMap.creatures.Remove(this);
    }


}
