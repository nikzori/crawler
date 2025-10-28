// Ripped from here: https://habr.com/ru/articles/513158/
// Unironically go run it through google translate and do it step-by-step if you're a surface-level dumbass like I am. 



internal readonly struct PathNode : IComparable<PathNode>
{
    public Vector2Int Position { get; }
    public readonly double TraverseDistance { get; }
    public readonly double EstimatedTotalCost { get; }

    public PathNode(Vector2Int Position, Vector2Int target, double TraverseDistance)
    {
        this.Position = Position;
        this.TraverseDistance = TraverseDistance;
        double heuristicDistance = (Position - target).DistanceEstimate();
        this.EstimatedTotalCost = TraverseDistance + heuristicDistance;
    }

    public int CompareTo(PathNode otherNode)
    {
        return EstimatedTotalCost.CompareTo(otherNode.EstimatedTotalCost);
    }
}

internal static class NodeExtensions
{
    private static readonly (Vector2Int position, double travelCost)[] NeighboursTemplate = {
        (new Vector2Int(1, 0), 1),
        (new Vector2Int(0, 1), 1),
        (new Vector2Int(-1, 0), 1),
        (new Vector2Int(0, -1), 1),
        (new Vector2Int(1, 1), Math.Sqrt(2)),
        (new Vector2Int(1, -1), Math.Sqrt(2)),
        (new Vector2Int(-1, 1), Math.Sqrt(2)),
        (new Vector2Int(-1, -1), Math.Sqrt(2))
    };
    public static void Fill(this PathNode[] buffer, PathNode parent, Vector2Int target)
    {
        int i = 0;
        foreach ((Vector2Int position, double cost) in NeighboursTemplate)
        {
            Vector2Int nodePosition = position + parent.Position;
            double traverseDistance = parent.TraverseDistance + cost;
            buffer[i++] = new PathNode(nodePosition, target, traverseDistance);
        }
    }
}

public class Path
{
    private const int MaxNeighbours = 8;
    private readonly PathNode[] neighbours = new PathNode[MaxNeighbours];

    private readonly int maxSteps;
    private readonly IBinaryHeap<Vector2Int, PathNode> frontier;
    private readonly HashSet<Vector2Int> ignoredPositions;
    private readonly List<Vector2Int> output;
    private readonly IDictionary<Vector2Int, Vector2Int> links;

    public Path(int maxSteps = int.MaxValue, int initialCapacity = 10)
    {
        if (maxSteps <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxSteps));
        if (initialCapacity < 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity));

        this.maxSteps = maxSteps;
        frontier = new BinaryHeap<Vector2Int, PathNode>(a => a.Position, initialCapacity);
        ignoredPositions = new HashSet<Vector2Int>(initialCapacity);
        output = new List<Vector2Int>(initialCapacity);
        links = new Dictionary<Vector2Int, Vector2Int>(initialCapacity);
    }

    public bool Calculate(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles,
            ref Queue<Vector2Int> path)
    {
        if (!GenerateNodes(start, target, obstacles))
        {
            path.Clear();
            return false;
        }

        output.Clear();
        output.Add(target);

        while (links.TryGetValue(target, out target))
            output.Add(target);
        for (int i = output.Count - 2; i >= 0; i--) // `Count - 2` to ignore NPC's starting position
        {
            path.Enqueue(output[i]);
        }


        return true;
    }

    public IReadOnlyCollection<Vector2Int> Calculate(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
    {
        if (!GenerateNodes(start, target, obstacles))
            return Array.Empty<Vector2Int>();

        output.Clear();
        output.Add(target);

        while (links.TryGetValue(target, out target))
            output.Add(target);
        return output.AsReadOnly();
    }

    private bool GenerateNodes(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
    {
        frontier.Clear();
        ignoredPositions.Clear();
        links.Clear();

        // starting point
        frontier.Enqueue(new PathNode(start, target, 0));
        ignoredPositions.UnionWith(obstacles);
        var step = 0;

        while (frontier.Count > 0 && step++ <= maxSteps)
        {
            // get last node in binary heap
            PathNode current = frontier.Dequeue();
            ignoredPositions.Add(current.Position);

            if (current.Position.Equals(target)) return true;

            // haven't reached the target; get neighbours for this node, repeat
            GenerateFrontierNodes(current, target);
        }

        // no nodes left, didn't find the target; target position unreachable
        return false;
    }

    // gets neighbours for a node, but with extra filters
    private void GenerateFrontierNodes(PathNode parent, Vector2Int target)
    {
        neighbours.Fill(parent, target);
        foreach (var neighbour in neighbours)
        {
            // check for obstacles and already calculated nodes
            if (ignoredPositions.Contains(neighbour.Position))
                continue;

            // Node is not present in the queue
            if (!frontier.TryGet(neighbour.Position, out PathNode existingNode))
            {
                frontier.Enqueue(neighbour);
                links[neighbour.Position] = parent.Position;
            }

            // Node is present in queue and new optimal path is detected
            else if (neighbour.TraverseDistance < existingNode.TraverseDistance)
            {
                frontier.Modify(neighbour);
                links[neighbour.Position] = parent.Position;
            }
        }

    }

    // using binary heap instead of a List to speed up the node sorting (log(n) speed, baby)

    internal class BinaryHeap<TKey, T> : IBinaryHeap<TKey, T>
        where TKey : IEquatable<TKey>
        where T : IComparable<T>
    {
        private readonly IDictionary<TKey, int> map;
        private readonly IList<T> collection;
        private readonly Func<T, TKey> lookupFunc;

        public BinaryHeap(Func<T, TKey> lookupFunc, int capacity)
        {
            this.lookupFunc = lookupFunc;
            collection = new List<T>(capacity);
            map = new Dictionary<TKey, int>(capacity);
        }

        public int Count => collection.Count;

        public void Clear()
        {
            collection.Clear();
            map.Clear();
        }

        public void Enqueue(T item)
        {
            collection.Add(item);
            int i = collection.Count - 1;
            map[lookupFunc(item)] = i;

            // Sort nodes from bottom to top
            while (i > 0)
            {
                int j = (i - 1) / 2;
                if (collection[i].CompareTo(collection[j]) > 0)
                    break;

                Swap(i, j);
                i = j;
            }
        }
        private void Swap(int i, int j)
        {
            T temp = collection[i];
            collection[i] = collection[j];
            collection[j] = temp;
            map[lookupFunc(collection[i])] = i;
            map[lookupFunc(collection[j])] = j;
        }

        public T Dequeue()
        {
            if (collection.Count == 0) return default;

            T result = collection.First();
            RemoveRoot();
            map.Remove(lookupFunc(result));
            return result;
        }

        private void RemoveRoot()
        {
            collection[0] = collection.Last();
            map[lookupFunc(collection[0])] = 0;
            collection.RemoveAt(collection.Count - 1);

            // sort nodes
            var i = 0;
            while (true)
            {
                int largest = LargestIndex(i);
                if (largest == i)
                    return;

                Swap(i, largest);
                i = largest;
            }
        }
        private int LargestIndex(int i)
        {
            int leftIndex = 2 * i + 1;
            int rightIndex = 2 * i + 2;
            int largest = i;

            if (leftIndex < collection.Count && collection[leftIndex].CompareTo(collection[largest]) > 0)
                largest = leftIndex;
            if (rightIndex < collection.Count && collection[rightIndex].CompareTo(collection[largest]) > 0)
                largest = rightIndex;

            return largest;

        }

        public bool TryGet(TKey key, out T value)
        {
            if (!map.TryGetValue(key, out int index))
            {
                value = default;
                return false;
            }

            value = collection[index];
            return true;
        }

        public void Modify(T value)
        {
            if (!map.TryGetValue(lookupFunc(value), out int index))
                throw new KeyNotFoundException(nameof(value));

            collection[index] = value;

        }
    }


    internal interface IBinaryHeap<in TKey, T>
    {
        void Enqueue(T item);
        T Dequeue();
        void Clear();
        bool TryGet(TKey key, out T value);
        void Modify(T value);
        int Count { get; }

    }
}



