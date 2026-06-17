using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
public class DebugInfo : View 
{
    Label position = new();
    Creature Player = Game.Player;
    public DebugInfo()
    {
        Width = Dim.Auto();
        Height = Dim.Auto();
        position = new()
        {
            Y = Pos.Top(this) + 2,
            Width = 15,
            Height = 3
        };
        this.Add(position);
    }

    void UpdatePos()
    {
        position.Text = "X: " + Player.Pos.X + "\nY: " + Player.Pos.Y;
        SetNeedsDraw();
    }
}
