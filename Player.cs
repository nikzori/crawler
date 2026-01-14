public static class PlayerController
{
    public static Creature Player = Game.Player;
    public static void CellInteract(Vector2Int pos)
    {
        Vector2Int tmp = Player.Pos + pos;
        if (Game.currentMap.cells.ContainsKey(tmp))
        {
            if (Game.currentMap.cells[tmp].creature != null)
                Game.currentMap.cells[tmp].creature?.ReceiveDamage(Player.Damage);
            else if (Game.currentMap.cells[tmp].IsWalkable)
                Player.Move(pos);
        }
    }

}
