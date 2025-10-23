using System.Text;
using Terminal.Gui.Drawing;
using Attribute = Terminal.Gui.Drawing.Attribute;
public class Player : Creature
{
    public int sightRadius = 10;

    public List<Item> inventory = new();

    public Player(string name, Vector2Int pos, Rune rune) : base(name, pos, rune)
    {
        color = new Attribute(Color.Black, Color.White);
    }

    public void TileInteract(Vector2Int pos)
    {
        Vector2Int tmp = this.pos + pos;
        if (Game.currentMap.cells.ContainsKey(tmp))
        {
            if (Game.currentMap.cells[tmp].creature != null)
                Game.currentMap.cells[tmp].creature.ReceiveDamage(10f);
            else if (Game.currentMap.cells[tmp].IsWalkable())
                this.Move(pos);


            //Game.Update(10);
        }
    }

}
