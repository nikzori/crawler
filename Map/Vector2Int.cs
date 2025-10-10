public struct Vector2Int : IEquatable<Vector2Int>
{
    public static readonly double Sqr2 = Math.Sqrt(2); // for diagonals

    public int X { get; }
    public int Y { get; }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Estimated distance without obstacles
    /// </summary>
    public double DistanceEstimate()
    {
        int linearSteps = Math.Abs(Math.Abs(Y) - Math.Abs(X));
        int diagonalSteps = Math.Max(Math.Abs(Y), Math.Abs(X)) - linearSteps;
        return linearSteps + Sqr2 * diagonalSteps;
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        => new Vector2Int(a.X + b.X, a.Y + b.Y);
    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        => new Vector2Int(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(Vector2Int a, Vector2Int b)
        => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector2Int a, Vector2Int b)
        => !(a == b);

    public bool Equals(Vector2Int other)
        => X == other.X && Y == other.Y;

    public override bool Equals(object? obj)
    {
        if (!(obj is Vector2Int))
            return false;

        var other = (Vector2Int)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public override string ToString()
        => $"({X}, {Y})";
}
