/** 
 * <summary>
 * A game object class to track positions on the map.
 * This is mainly for entities, but some other things are viable too, e.g. trigger boxes, pointer for aiming, etc.
 * hence nullable Entity.
 * </summary>
 */
public class GameObject
{
    public Entity? entity;
    public (int x, int y) pos;

    Rune _rune;
    public Rune rune
    {
        get 
        { 
            if (entity is null)
                return _rune;
            else return entity.rune;
        }
        set { _rune = value; }
    } 


    public GameObject((int x, int y) pos)
    {
        this.pos = pos;
    }
    public GameObject(Entity e)
    {
        pos = (0, 0);
        entity = e;
        rune = e.rune;
    }
    public GameObject((int x, int y) pos, Entity e)
    {
        this.pos = pos;
        entity = e;
        rune = e.rune;
    }

    public bool Move(int x, int y)
    {
        int xt = pos.x + x;
        int yt = pos.y + y;

        return MoveTo(xt, yt);
    }

    public bool MoveTo(int x, int y)
    {
        if (Game.dungeon.GetCurrentFloor().cells.GetLength(0) < x || Game.dungeon.GetCurrentFloor().cells.GetLength(1) < y )
        {
            UI.Log("Trying to move GameObject out of map boundaries; object stays at x: " + pos.x + "; y: " + pos.y);
            return false;
        }
        if (!Game.dungeon.GetCurrentFloor().cells[x,y].isWalkable)
            return false;
        Game.dungeon.GetCurrentFloor().cells[pos.x, pos.y].RemoveGameObject(this);
        Game.dungeon.GetCurrentFloor().cells[x, y].AddGameObject(this);
        pos = (x, y);
        return true;
        
    }
}
