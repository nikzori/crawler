using System.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.Input;

public class MapView : FrameView
{
    int pX, pY; // player position on the screen
    int mX, mY;
    public MapView()
    {
        // viewport size needs to be an odd number to put player in the center
        Width = 31;
        Height = 31;
        GameWindow.Log("Map viewport width: " + Viewport.Width.ToString() + "; height: " + Viewport.Height.ToString());
        Visible = true;
        pX = Viewport.Width / 2;
        pY = Viewport.Height / 2; // center the player on the screen

        SetFocus();
    }
    protected override bool OnClearingViewport() { return true; }
    #region Line of Sight
    protected override bool OnDrawingContent(DrawContext? context)
    {
        //upper-left visible map cell coordinates 
        mX = Game.player.pos.X - pX;
        mY = Game.player.pos.Y - pY;

        Vector2Int currentPos = new(mX, mY);
        Rune c;
        for (int tx = 0; tx < Viewport.Width; tx++)
        {
            for (int ty = 0; ty < Viewport.Height; ty++)
            {
                if (Game.currentMap.cells.ContainsKey(currentPos))
                {
                    Cell cell = Game.currentMap.cells[currentPos];
                    c = GetRune(cell);
                    if (cell.Type == CellType.Wall)                        
                        SetAttribute(Dungeon.WALL_COLOR);
                    else SetAttribute(Dungeon.FLOOR_COLOR);
                    //Game.currentMap.cells[currentPos].SetRevealed(true);

                    if (Math.Abs(tx - pX) < Game.player.sightRadius && Math.Abs(ty - pY) < Game.player.sightRadius)
                    {
                        // very unnatural (and probably very inefficient) LOS made with Bresenham's algorythm, 
                        // but hey, it works
                        if (Dungeon.CanSeeTile(Game.player.pos, currentPos))
                        {
                            cell.isRevealed = true;
                            if (cell.Type == CellType.Wall)                        
                                SetAttribute(Dungeon.WALL_COLOR);
                            else SetAttribute(Dungeon.FLOOR_COLOR);
                        }
                        else if (Game.currentMap.cells[currentPos].isRevealed)
                        {
                            SetAttribute(Dungeon.REVEALED_COLOR);
                        }
                        else
                        {
                            c = new Rune(Game.currentMap.background[new(mX + 15, mY + 15)]);
                            SetAttribute(Dungeon.OBSCURED_COLOR);
                        }
                    }
                    else if (Game.currentMap.cells[currentPos].isRevealed)
                    {
                        SetAttribute(Dungeon.REVEALED_COLOR);
                    }
                    else
                    {
                        c = new(Game.currentMap.background[new(mX + 15, mY + 15)]);
                        SetAttribute(Dungeon.OBSCURED_COLOR);
                    }
                }
                else
                {
                    c = new(Game.currentMap.background[new(mX + pX, mY + pY)]);
                    SetAttribute(Dungeon.OBSCURED_COLOR);
                }
                AddRune(tx, Viewport.Height - ty, c);

                mY++;
                currentPos = new(mX, mY);
            }
            mX++;
            mY = Game.player.pos.Y - pY;
            currentPos = new(mX, mY);
        }
        return true;
    }
    #endregion
    public Rune GetRune(Cell cell)
    {
        if (cell.creature != null)
            return cell.creature.Rune;
        else
        {
            if  (cell.Type == CellType.Floor)
                return Dungeon.FLOOR;
            else if (cell.Type == CellType.Wall)
                return Dungeon.WALL;
            else return new('?');
        }
    }

    protected override bool OnKeyDown(Key key)
    {
        bool processed = false;

        // switch-case doesn't like Key.Parameters for some reason
        // so this will be ugly for now
        if (key == Key.D1)
        {
            processed = true;
            Game.player.TileInteract(new(-1, -1));
        }
        if (key == Key.D2)
        {
            processed = true;
            Game.player.TileInteract(new(0, -1));
        }
        if (key == Key.D3)
        {
            processed = true;
            Game.player.TileInteract(new(1, -1));
        }
        if (key == Key.D4)
        {
            processed = true;
            Game.player.TileInteract(new(-1, 0));
        }
        if (key == Key.D5)
        {
            processed = true;
            //interact mode?
            Game.Update(10);
            GameWindow.Log("Key Registered");
        }
        if (key == Key.D6)
        {
            processed = true;
            Game.player.TileInteract(new(1, 0));
        }
        if (key == Key.D7)
        {
            processed = true;
            Game.player.TileInteract(new(-1, 1));
        }
        if (key == Key.D8)
        {
            processed = true;
            Game.player.TileInteract(new(0, 1));
        }
        if (key == Key.D9)
        {
            processed = true;
            Game.player.TileInteract(new(1, 1));
        }
        if (processed)
            SetNeedsDraw();
        key.Handled = processed; // unclear if I need to do this, but just in case 
        return processed;
    }
}
