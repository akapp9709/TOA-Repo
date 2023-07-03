using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueRaja;

public class PF_Pathfinder
{
    public class Node
    {
        public Vector2Int Position { get; private set; }
        public Node Previous { get; set; }
        public float Cost { get; set; }

        public Node(Vector2Int pos)
        {
            Position = pos;
        }
    }

    public struct PathCost
    {
        public bool traversable;
        public float cost;
    }

    private static readonly Vector2Int[] neighbours =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private PF_Grid<Node> _grid;
    private SimplePriorityQueue<Node, float> _queue;
    private HashSet<Node> _closed;
    private Stack<Vector2Int> _stack;

    public PF_Pathfinder(Vector2Int size)
    {
        _grid = new PF_Grid<Node>(size, Vector2Int.zero);
        _queue = new SimplePriorityQueue<Node, float>();
        _closed = new HashSet<Node>();
        _stack = new Stack<Vector2Int>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                _grid[i, j] = new Node(new Vector2Int(i, j));
            }
        }
    }

    private void ResetNodes()
    {
        var size = _grid.Size;

        for (var i = 0; i < size.x; i++)
        {
            for (var j = 0; j < size.y; j++)
            {
                var node = _grid[i, j];
                node.Previous = null;
                node.Cost = float.PositiveInfinity;
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, Func<Node, Node, PathCost> costFunction)
    {
        ResetNodes();
        _queue.Clear();
        _closed.Clear();

        _queue = new SimplePriorityQueue<Node, float>();
        _closed = new HashSet<Node>();

        _grid[start].Cost = 0;
        _queue.Enqueue(_grid[start], 0);

        while (_queue.Count > 0)
        {
            var node = _queue.Dequeue();
            _closed.Add(node);

            if (node.Position == end)
            {
                return ReconstructPath(node);
            }

            foreach (var offset in neighbours)
            {
                if (!_grid.InBounds(node.Position + offset))
                    continue;
                var neighbour = _grid[node.Position + offset];
                if (_closed.Contains(neighbour))
                    continue;

                var pathCost = costFunction(node, neighbour);
                if(!pathCost.traversable)
                    continue;

                var newCost = node.Cost + pathCost.cost;

                if (newCost < neighbour.Cost)
                {
                    neighbour.Previous = node;
                    neighbour.Cost = newCost;

                    if (_queue.TryGetPriority(node, out float existingPriority))
                    {
                        _queue.UpdatePriority(node, newCost);
                    }
                    else
                    {
                        _queue.Enqueue(neighbour, neighbour.Cost);
                    }
                }
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(Node node)
    {
        var result = new List<Vector2Int>();

        while (node != null)
        {
            _stack.Push(node.Position);
            node = node.Previous;
        }

        while (_stack.Count > 0)
        {
            result.Add(_stack.Pop());
        }

        return result;
    }
}
