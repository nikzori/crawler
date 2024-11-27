/** 
 * <summary>
 * A game object class to track positions on the map.
 * This is mainly for entities, but some other things are viable too, e.g. trigger boxes, pointer for aiming, etc.
 * hence nullable Entity.
 * </summary>
 */

using Terminal.Gui;
public class GameObject
{
    public Entity? entity;
    public (int x, int y) pos;

    public Rune rune; // separate rune for game object


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

    public void Move(int x, int y)
    {
        int xt = pos.x + x;
        int yt = pos.y + y;

        if (Game.map.cells[xt, yt].IsWall() && entity != null) //restrict movement only for entities
            return;

        Game.map.cells[pos.x, pos.y].RemoveGameObject(this);
        Game.map.cells[xt, yt].AddGameObject(this);
        pos = (xt, yt);
    }
}
