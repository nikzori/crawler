/** 
 * <summary>
 * A game object class to track positions on the map.
 * This is mainly for entities, but some other things are viable too, e.g. trigger boxes, pointer for aiming, etc.
 * hence nullable Entity.
 * </summary>
 */
public class Creature
{
    public string name;
    public int health = 10;
    public int level = 1;
    public (int x, int y) pos;

    Rune _rune;
    public Rune rune { get; set; } // gonna need this for status effects and such

    public int aut = 0;
    public Creature((int x, int y) pos)
    {
        this.pos = pos;
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
            UI.Log("Trying to move GameObject out of map boundaries; object stays at x: " + pos.x + "; y: " + pos.y);
            return false;
        }
        if (!Game.dungeon.GetCurrentFloor().cells[x, y].isWalkable || Game.dungeon.GetCurrentFloor().cells[x, y].HasCreature())
            return false;
        Game.dungeon.GetCurrentFloor().cells[pos.x, pos.y].RemoveCreature(this);
        Game.dungeon.GetCurrentFloor().cells[x, y].AddCreature(this);
        pos = (x, y);
        return true;
    }
}
