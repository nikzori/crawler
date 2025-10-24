using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

public class MapView : FrameView
{
    int pX, pY; // player position on the screen
    int mX, mY;
    public MapView()
    {
        // viewport size needs to be an odd number to put player in the center
        Width = 31;
        Height = 31;
        UI.Log("Map viewport width: " + Viewport.Width.ToString() + "; height: " + Viewport.Height.ToString());
        Visible = true;
        pX = Viewport.Width / 2;
        pY = Viewport.Height / 2; // center the player on the screen
    }
    protected override bool OnClearingViewport() { return true; }
    #region Line of Sight
    protected override bool OnDrawingText(DrawContext? context)
    {
        //upper-left visible map cell coordinates 
        mX = Game.player.pos.X - pX;
        mY = Game.player.pos.Y - pY;

        Vector2Int currentPos = new(mX, mY);
        System.Text.Rune c = new('.');
        for (int tx = 0; tx < Viewport.Width; tx++)
        {
            for (int ty = 0; ty < Viewport.Height; ty++)
            {
                if (Game.currentMap.cells.ContainsKey(currentPos))
                {
                    c = Game.currentMap.cells[currentPos].Rune;
                    //Game.currentMap.cells[currentPos].SetRevealed(true);

                    SetAttribute(Game.currentMap.cells[currentPos].Color);
                    if (Math.Abs(tx - pX) < Game.player.sightRadius && Math.Abs(ty - pY) < Game.player.sightRadius)
                    {
                        // very unnatural (and probably very inefficient) LOS made with Bresenham's algorythm, 
                        // but hey, it works
                        if (Dungeon.CanSeeTile(Game.player.pos, currentPos))
                        {
                            c = Game.currentMap.cells[currentPos].Rune;
                            Game.currentMap.cells[currentPos].SetRevealed(true);

                            SetAttribute(Game.currentMap.cells[currentPos].Color);
                        }
                        else if (Game.currentMap.cells[currentPos].isRevealed)
                        {
                            c = Game.currentMap.cells[currentPos].Rune;
                            SetAttribute(Dungeon.REVEALED_COLOR);
                        }
                        else
                        {
                            c = new System.Text.Rune(Game.currentMap.background[new(mX + 15, mY + 15)]);
                            SetAttribute(Dungeon.OBSCURED_COLOR);
                        }
                    }
                    else if (Game.currentMap.cells[currentPos].isRevealed)
                    {
                        c = Game.currentMap.cells[currentPos].Rune;
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

}
