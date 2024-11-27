using Terminal.Gui;

public class Inventory : View
{
  public void Init()
  {
    Label test = new("This is a test entry");
    SetNeedsDisplay();
  }

  public override bool OnKeyDown(KeyEvent keyEvent)
  {
    bool keyProcessed = false;
    switch (keyEvent.Key)
    {
      case (Key.Esc):
        keyProcessed = true;
        this.Enabled = false;
        Game.OpenMain();
        break;
      default:
        break;
    }
    return true;
  }
}

