/** 
 * <summary>
 * A game object class to track positions on the map.
 * This is mainly for entities, but some other things are viable too, e.g. trigger boxes, pointer for aiming, etc.
 * hence nullable Entity.
 * </summary>
 */
public class Creature
{
    public static Rune skeleton = new('\u223B');
    public string name;
    public float health = 10f;
    public int level = 1;
    public (int x, int y) pos;

    Rune _rune;
    public Rune rune { get; set; } // gonna need this for status effects and such

    public int aut = 0;

    public Creature(string name, (int x, int y) pos, Rune rune)
    {
        this.name = name;
        this.pos = pos;
        this.rune = rune;
        AI.creatures.Add(this, AIState.idle);
    }

    public bool Move(int x, int y)
    {
        int xt = pos.x + x;
        int yt = pos.y + y;

        return MoveTo(xt, yt);
    }

    public bool MoveTo(int x, int y)
    {
        if (Game.dungeon.GetCurrentFloor().cells.GetLength(0) < x || Game.dungeon.GetCurrentFloor().cells.GetLength(1) < y)
        {
            UI.Log("Trying to move creature out of map boundaries; object stays at x: " + pos.x + "; y: " + pos.y);
            return false;
        }
        if (!Game.dungeon.GetCurrentFloor().cells[x, y].isWalkable || Game.dungeon.GetCurrentFloor().cells[x, y].HasCreature())
            return false;

        Game.dungeon.GetCurrentFloor().cells[pos.x, pos.y].RemoveCreature();
        Game.dungeon.GetCurrentFloor().cells[x, y].AddCreature(this);
        pos = (x, y);
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
        AI.creatures.Remove(this);
        Game.currentMap.cells[pos.x, pos.y].RemoveCreature();
        Game.currentMap.cells[pos.x, pos.y].rune = Creature.skeleton;
    }
}
