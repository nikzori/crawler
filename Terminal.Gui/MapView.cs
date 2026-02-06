using System.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.Input;

public class MapView : FrameView
{
    Creature Player = Game.Player;
    int pX, pY; // player position on the screen
    int mX, mY;
    public MapView()
    {
        // viewport size needs to be an odd number to put player in the center
        Width = 31;
        Height = 31;
        Game.Log("Map viewport width: " + Viewport.Width.ToString() + "; height: " + Viewport.Height.ToString());
        Visible = true;
        pX = Viewport.Width / 2;
        pY = Viewport.Height / 2; // center the player on the screen

        SetFocus();
    }
    protected override bool OnClearingViewport() { return true; }
    #region Line of Sight
    protected override bool OnDrawingContent(DrawContext? context)
    {
        try
        {
            bool[,] vision = FOV.Shadowcast(Game.CurrentMap, Player.Pos, Player.SightRadius);
            mX = Player.Pos.X - pX;
            mY = Player.Pos.Y - pY;

            Rune c;
            Vector2Int currentPos;
            for (int tx = 0; tx < Viewport.Width; tx++)
            {
                for (int ty = 0; ty < Viewport.Height; ty++)
                {
                    currentPos = new(mX, mY);

                    if (Game.CurrentMap.cells.ContainsKey(currentPos))
                    {
                        Cell cell = Game.CurrentMap.cells[currentPos];
                        if (vision[currentPos.X, currentPos.Y])
                        {
                            if (cell.HasCreature())
                            {
                                c = new('@');
                                SetAttribute(new Terminal.Gui.Drawing.Attribute(Terminal.Gui.Drawing.StandardColor.Green, Terminal.Gui.Drawing.StandardColor.Black));
                            }
                            else
                            {
                                if (cell.Type == CellType.Wall)
                                {
                                    c = Dungeon.WALL;
                                    SetAttribute(Dungeon.WALL_COLOR);
                                }
                                else
                                {
                                    c = Dungeon.FLOOR;
                                    SetAttribute(Dungeon.FLOOR_COLOR);
                                }
                            }
                        }
                        else
                        {
                            if (cell.isRevealed)
                            {
                                if (cell.Type == CellType.Wall)
                                    c = Dungeon.WALL;
                                else c = Dungeon.FLOOR;
                                SetAttribute(Dungeon.REVEALED_COLOR);
                            }
                            else
                            {
                                c = new(Game.CurrentMap.background[new(mX + 15, mY + 15)]);
                                SetAttribute(Dungeon.OBSCURED_COLOR);
                            }
                        }
                    }
                    else
                    {
                        c = new(Game.CurrentMap.background[new(mX + 15, mY + 15)]);
                        SetAttribute(Dungeon.OBSCURED_COLOR);
                    }

                    AddRune(tx, Viewport.Height - ty, c);

                    mY++;
                }

                mX++;
                mY = Player.Pos.Y - pY;
            }

            /*
            //upper-left visible map cell coordinates 
            mX = Player.Pos.X - pX;
            mY = Player.Pos.Y - pY;

            Vector2Int currentPos = new(mX, mY);
            Rune c;
            for (int tx = 0; tx < Viewport.Width; tx++)
            {
                for (int ty = 0; ty < Viewport.Height; ty++)
                {
                    if (Game.CurrentMap.cells.ContainsKey(currentPos))
                    {
                        Cell cell = Game.CurrentMap.cells[currentPos];
                        c = GetRune(cell);
                        if (cell.Type == CellType.Wall)
                            SetAttribute(Dungeon.WALL_COLOR);
                        else SetAttribute(Dungeon.FLOOR_COLOR);
                        //Game.CurrentMap.cells[currentPos].SetRevealed(true);

                        if (Math.Abs(tx - pX) < Player.SightRadius && Math.Abs(ty - pY) < Player.SightRadius)
                        {
                            // very unnatural (and probably very inefficient) LOS made with Bresenham's algorythm, 
                            // but hey, it works
                            if (Dungeon.CanSeeTile(Player.Pos, currentPos))
                            {
                                cell.isRevealed = true;
                                if (cell.Type == CellType.Wall)
                                    SetAttribute(Dungeon.WALL_COLOR);
                                else SetAttribute(Dungeon.FLOOR_COLOR);
                            }
                            else if (Game.CurrentMap.cells[currentPos].isRevealed)
                            {
                                SetAttribute(Dungeon.REVEALED_COLOR);
                            }
                            else
                            {
                                c = new Rune(Game.CurrentMap.background[new(mX + 15, mY + 15)]);
                                SetAttribute(Dungeon.OBSCURED_COLOR);
                            }
                        }
                        else if (Game.CurrentMap.cells[currentPos].isRevealed)
                        {
                            SetAttribute(Dungeon.REVEALED_COLOR);
                        }
                        else
                        {
                            c = new(Game.CurrentMap.background[new(mX + 15, mY + 15)]);
                            SetAttribute(Dungeon.OBSCURED_COLOR);
                        }
                    }
                    else
                    {
                        c = new(Game.CurrentMap.background[new(mX + pX, mY + pY)]);
                        SetAttribute(Dungeon.OBSCURED_COLOR);
                    }
                    AddRune(tx, Viewport.Height - ty, c);

                    mY++;
                    currentPos = new(mX, mY);
                }
                mX++;
                mY = Player.Pos.Y - pY;
                currentPos = new(mX, mY);
            }
            */
            return true;
        }
        catch (Exception e)
        {
            Game.Log(e.Message);
            File.WriteAllText(AppContext.BaseDirectory, e.Message);
            return true;
        }
    }
    #endregion
    public Rune GetRune(Cell cell)
    {
        if (cell.creature != null)
            return new('g');
        else
        {
            if (cell.Type == CellType.Floor)
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
            PlayerController.CellInteract(new(-1, -1));
        }
        if (key == Key.D2)
        {
            processed = true;
            PlayerController.CellInteract(new(0, -1));
        }
        if (key == Key.D3)
        {
            processed = true;
            PlayerController.CellInteract(new(1, -1));
        }
        if (key == Key.D4)
        {
            processed = true;
            PlayerController.CellInteract(new(-1, 0));
        }
        if (key == Key.D5)
        {
            processed = true;
            //interact mode?
            Game.Update(10);
            Game.Log("Key Registered");
        }
        if (key == Key.D6)
        {
            processed = true;
            PlayerController.CellInteract(new(1, 0));
        }
        if (key == Key.D7)
        {
            processed = true;
            PlayerController.CellInteract(new(-1, 1));
        }
        if (key == Key.D8)
        {
            processed = true;
            PlayerController.CellInteract(new(0, 1));
        }
        if (key == Key.D9)
        {
            processed = true;
            PlayerController.CellInteract(new(1, 1));
        }
        if (processed)
            SetNeedsDraw();
        key.Handled = processed; // unclear if I need to do this, but just in case 
        return processed;
    }
}
