using Terminal.Gui;

public class Inventory : View
{
  public void Init()
  {
    Label test = new("This is a test entry"){ X = 1, Y = 1, Width = 30, Height = 3 };
    this.Add(test);
    SetNeedsDisplay();
  }

  public override bool ProcessHotKey(KeyEvent keyEvent)
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
}

