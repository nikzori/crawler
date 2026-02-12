public static class PlayerController
{
    public static Creature Player = Game.Player;
    public static void CellInteract(Vector2Int pos)
    {
        Vector2Int tmp = Player.Pos + pos;
        if (Game.CurrentMap.cells.ContainsKey(tmp))
        {
            if (Game.CurrentMap.cells[tmp].IsWalkable)
            {
                if (Game.CurrentMap.cells[tmp].creature != null)
                    Game.CurrentMap.cells[tmp].creature?.ReceiveDamage(999); // perish lmao
                else Player.Move(pos);

                Game.Update(5);
            }
        }
    }

}
