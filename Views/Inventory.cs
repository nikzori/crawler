using Terminal.Gui.Views;
using Terminal.Gui.ViewBase;

public class Inventory : View
{
    public void Init()
    {
        Label test = new() { X = 1, Y = 1, Width = 30, Height = 3, Text = "This is a test entry" };
        this.Add(test);
        SetNeedsDraw();
    }

    /*
    public override bool ProcessHotKey(Terminal.Gui.Input.Key keyEvent)
    {
        if (!Visible)
            return false;
        bool keyProcessed = false;
        switch (keyEvent.Key)
        {
            case Key.Esc:
                keyProcessed = true;
                this.Visible = false;
                UI.ShowMain();
                break;
            default:
                UI.Log("Pressed " + keyEvent.Key.ToString() + " in inventory");
                break;
        }
        return keyProcessed;
    }
    */
}

