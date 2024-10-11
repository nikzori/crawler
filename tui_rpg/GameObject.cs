/** 
 * <summary>
 * A game object class to track positions on the map.
 * This is mainly for entities, but some other things are viable too, e.g. trigger boxes,
 * hence nullable Entity.
 * </summary>
 */
public class GameObject
{
    public Entity? entity;
    public (int x, int y) pos; // public and naked for now, will add get/set or functions if events or rerenders need to fire explicitly

    public GameObject((int x, int y) pos)
    {
        this.pos = pos;
    }
    public GameObject(Entity e)
    {
        pos = (0, 0);
        entity = e;
    }
    public GameObject((int x, int y) pos, Entity e)
    {
        this.pos = pos;
        entity = e;
    }

    public void Move(int x, int y)
    {
        int xt = pos.x + x;
        int yt = pos.y + y;
        if (Game.map.cells[xt, yt].isWall())
            return;

        else
        {
            Game.map.cells[pos.x, pos.y].RemoveGameObject(this);
            Game.map.cells[xt, yt].AddGameObject(this);
            pos = (xt, yt);
        }
    }
}
