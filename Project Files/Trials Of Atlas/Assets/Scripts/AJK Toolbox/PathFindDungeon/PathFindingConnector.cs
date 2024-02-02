using System.Collections;
using System.Collections.Generic;
using Graphs;
using UnityEngine;

public class PathFindingConnector : ConnecterBuilder
{
    private PF_Grid<GridBuilder.CellType> layout;
    public int hallRadius;
    public override void BuildConnections(GridBuilder grid, HashSet<Prim.Edge> edges)
    {
        base.BuildConnections(grid);

        var aStar = new PF_Pathfinder(grid.maxSize);
        layout = grid.grid;

        foreach (var edge in edges)
        {
            var startRoom = (edge.U as Vertex<RoomSpace>).Item;
            var endRoom = (edge.V as Vertex<RoomSpace>).Item;

            var startPos = startRoom.GetClosestEntrance(Vector2Int.FloorToInt(startRoom.bounds.center));
            var endPos = endRoom.GetClosestEntrance(startPos);

            var path = aStar.FindPath(startPos, endPos, cost);

            if(path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];
                    if (layout[current] is GridBuilder.CellType.None or GridBuilder.CellType.Buffer)
                    {
                        layout[current] = GridBuilder.CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];
                        var delta = current - prev;
                    }
                }

                foreach (var pos in path)
                {
                    for (int i = 1; i < hallRadius; i++)
                    {
                        foreach(var offset in surroundingPositions)
                        {
                            var curPos = layout[pos + (offset*i)];
                            if(curPos != GridBuilder.CellType.Room && layout.InBounds(pos + (offset*i)))
                            {
                                layout[pos + offset*i] = GridBuilder.CellType.Hallway;
                            }
                        }
                    }
                }
            }
        }
    }

    public PF_Pathfinder.PathCost cost(PF_Pathfinder.Node a, PF_Pathfinder.Node b) => GetCost(a, b);

    public PF_Pathfinder.PathCost GetCost(PF_Pathfinder.Node a, PF_Pathfinder.Node b)
    {
        var cost = new PF_Pathfinder.PathCost();

                // cost.cost += Vector2Int.Distance(b.Position, maxSize/2) * 0.2f;

                switch (layout[b.Position])
                {
                    case GridBuilder.CellType.None:
                        cost.cost += 9;
                        break;
                    case GridBuilder.CellType.Buffer:
                        cost.cost += 25;
                        break;
                    case GridBuilder.CellType.Hallway:
                        cost.cost += 1;
                        break;
                    // case GridBuilder.CellType.SoftBuffer:
                    //     cost.cost += 30;
                    //     break;
                    case GridBuilder.CellType.Room:
                        cost.cost += 100;
                        break;
                    default:
                        break;
                }

                cost.traversable = true;

                return cost;
    }

    private List<Vector2Int> surroundingPositions = new List<Vector2Int> {
        new Vector2Int(1,1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(1,-1),
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
    };
}
