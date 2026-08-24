public static class PlayerController
{
    public static Creature Player = Game.Player;
    public static void CellInteract(Vector2Int pos)
    {
        Vector2Int tmp = Player.Pos + pos;
        if (Game.CurrentMap.cells.ContainsKey(tmp))
        {
            Cell tCell = Game.CurrentMap.cells[tmp];
            if (Game.CurrentMap.cells[tmp].IsWalkable)
            {
                if (Game.CurrentMap.cells[tmp].creature != null)
                    Attack(tCell.creature);
                else Player.Move(pos);

                Game.Update(5);
            }
        }
    }
    public static void Attack(Creature target)
    {
        Game.Log("");
    }

}
