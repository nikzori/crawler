using System.Text;
using Terminal.Gui.Drawing;
using Attribute = Terminal.Gui.Drawing.Attribute;

public class Creature
{
    public static Rune remains = new('\u223B'); // to be shown on decomposition
    public string name;
    public float health = 10f;
    public int level = 1;
    public int MoveSpeed { get; }
    public Vector2Int pos;

    public Rune Rune
    {
        get
        {
            if (health <= 0)
                return remains;
            else return field;
        }
    }
    public Attribute color = new(Color.White, Color.Black);

    // AI stuff
    // putting this here feels kinda ass so idk
    public int aut = 0;
    public AIState state = AIState.idle;
    public bool isTrackingPlayer = false;
    public Vector2Int lastPlayerPosition;
    public Queue<Vector2Int> currentPath = new();

    public Creature(string name, Vector2Int pos, Rune rune)
    {
        this.name = name;
        this.pos = pos;
        this.Rune = rune;
        this.MoveSpeed = 10;
    }

    // Shorthand for single vectors
    public bool Move(Vector2Int dir) { return MoveTo(this.pos + dir); }

    public bool MoveTo(Vector2Int pos)
    {
        if (!Game.dungeon.GetCurrentFloor().cells.ContainsKey(pos))
        {
            GameWindow.Log("Trying to move creature out of map boundaries; object stays at x: " + this.pos.X + "; y: " + this.pos.Y);
            return false;
        }
        if (!Game.dungeon.GetCurrentFloor().cells[pos].isWalkable || Game.dungeon.GetCurrentFloor().cells[pos].HasCreature())
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

    void OnCreatureDeath() { Game.currentMap.creatures.Remove(this); }


}

public interface IDamageable
{
    int Health { get; }
    void ReceiveDamage(int value);
}
