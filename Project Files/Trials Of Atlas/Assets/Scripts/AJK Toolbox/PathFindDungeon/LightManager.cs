using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private List<LightLine> _lines = new List<LightLine>();
    private PF_Grid<GridBuilder.CellType> _grid;
    private Vector2Int maxSize;
    [SerializeField] private float LightDensity = 1;
    [SerializeField] private GameObject LightPrefab;

    private void OnDrawGizmos() 
    {
        foreach( var line in _lines)
        {
            Gizmos.color = Color.yellow;

            var offset = Vector3.up * 4.5f;
            var pos1 = new Vector3(line.StartPos.x, 0f, line.StartPos.y) + offset;
            var pos2 = new Vector3(line.EndPos.x, 0f, line.EndPos.y) + offset;

            Gizmos.DrawSphere(pos1, 0.5f);
            Gizmos.DrawSphere(pos2, 0.5f);
            Gizmos.DrawLine(pos1, pos2);
            Handles.Label((pos1+pos2)/2f, "\t" + line.directString);

            foreach (var pos in line.Positions)
            {
                Gizmos.DrawCube(pos + offset, Vector3.one*0.2f);
            }
        }
    }

    public void PlaceLights(PF_Grid<GridBuilder.CellType> grid)
    {
        _grid = grid;
        maxSize = _grid.Size;
        LightLine.LightCount = LightDensity;

        MarkLineWest();
        MarkLineEast();
        MarkLineNorth();
        MarkLineSouth();

        MarkLightPositions();
        GenerateLights();

        Debug.Log($"{_lines.Count} Lines marked");
    }

    private void GenerateLights()
    {
        foreach(var line in _lines)
        {
            foreach(var pos in line.Positions)
            {
                int rot = 0;

                switch(line.Direction)
                {
                    case LightLine.LineDirection.WestToEast:
                        rot = 3;
                        
                        break;
                    case LightLine.LineDirection.EasttoWest:
                        rot = 1;
                        
                        break;
                    case LightLine.LineDirection.NorthToSouth:
                        rot = 2;
                        break;
                    case LightLine.LineDirection.SouthToNorth:
                        rot = 0;
                        break;
                }

                Instantiate(LightPrefab, pos, Quaternion.identity * Quaternion.Euler(0f, rot*90f, 0f), transform);
            }
        }
    }

    private void MarkLightPositions()
    {
        foreach (var line in _lines)
        {
            for (var i = 0.5f; i < line.NumberOfLights(); i++)
            {
                Vector3 pos;
                switch (line.Direction)
                {
                    case LightLine.LineDirection.NorthToSouth:
                    case LightLine.LineDirection.SouthToNorth:
                        pos = new Vector3(line.StartPos.x, 0f, line.StartPos.y + line.LightSpacing() * i);
                        line.Positions.Add(pos);
                        break;
                    case LightLine.LineDirection.WestToEast:
                    case LightLine.LineDirection.EasttoWest:
                        pos = new Vector3(line.StartPos.x + line.LightSpacing() * i, 0f, line.StartPos.y);
                        line.Positions.Add(pos);
                        break;

                }
            }
        }
    }

    private void MarkLineWest()
    {
        Vector2Int pos1 = Vector2Int.zero, pos2 = Vector2Int.zero;

        for (int x = 0; x < maxSize.x; x++)
        {
            for (int y = 0; y < maxSize.y; y++)
            {
                if (pos1 == Vector2Int.zero 
                    && ((_grid[x, y] is GridBuilder.CellType.CornerOutSW or GridBuilder.CellType.Room 
                    && _grid[x, y + 1] == GridBuilder.CellType.WallWest)
                    || (_grid[x, y] == GridBuilder.CellType.Hallway 
                    && _grid[x - 1, y] is GridBuilder.CellType.WallNorth or GridBuilder.CellType.CornerOutNW)))
                {
                    pos1 = new Vector2Int(x, y);
                }
                else if (_grid[x, y] == GridBuilder.CellType.CornerOutNW 
                    || ((_grid[x, y] is GridBuilder.CellType.Room or GridBuilder.CellType.CornerOutNW 
                    && _grid[x, y - 1] == GridBuilder.CellType.WallWest)
                    || (_grid[x, y] == GridBuilder.CellType.Hallway 
                    && _grid[x - 1, y] is GridBuilder.CellType.WallSouth or GridBuilder.CellType.CornerOutSW)))
                {
                    pos2 = new Vector2Int(x, y);
                }

                if (pos1 != Vector2Int.zero && pos2 != Vector2Int.zero)
                {
                    _lines.Add(new LightLine(pos1, pos2, LightLine.LineDirection.SouthToNorth));
                    pos1 = Vector2Int.zero;
                    pos2 = Vector2Int.zero;
                }
            }

            pos1 = Vector2Int.zero;
            pos2 = Vector2Int.zero;
        }
    }

    //Working
    private void MarkLineEast()
    {
        Vector2Int pos1 = Vector2Int.zero, pos2 = Vector2Int.zero;

        for (int x = 0; x < maxSize.x; x++)
        {
            for (int y = 0; y < maxSize.y; y++)
            {
                if (pos1 == Vector2Int.zero 
                    && ((_grid[x, y] is GridBuilder.CellType.CornerOutSE or GridBuilder.CellType.Room 
                    && _grid[x, y + 1] == GridBuilder.CellType.WallEast)
                    || (_grid[x, y] == GridBuilder.CellType.Hallway 
                    && _grid[x + 1, y] is GridBuilder.CellType.WallNorth or GridBuilder.CellType.CornerOutNE)))
                {
                    pos1 = new Vector2Int(x, y);
                } 
                else if (pos1 != Vector2Int.zero 
                        && ((_grid[x, y] is GridBuilder.CellType.Room or GridBuilder.CellType.CornerOutNE 
                        && _grid[x, y - 1] is GridBuilder.CellType.WallEast or GridBuilder.CellType.Hallway)
                        || (_grid[x, y] == GridBuilder.CellType.Hallway 
                        && (_grid[x + 1, y] is GridBuilder.CellType.WallSouth or GridBuilder.CellType.CornerOutSE))))
                {
                    pos2 = new Vector2Int(x, y);
                }

                if (pos1 != Vector2Int.zero && pos2 != Vector2Int.zero)
                {
                    _lines.Add(new LightLine(pos1, pos2, LightLine.LineDirection.NorthToSouth));
                    pos1 = Vector2Int.zero;
                    pos2 = Vector2Int.zero;
                }
            }

            pos1 = Vector2Int.zero;
            pos2 = Vector2Int.zero;
        }
    }

    //Working
    private void MarkLineNorth()
    {
        Vector2Int pos1 = Vector2Int.zero, pos2 = Vector2Int.zero;

        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                if(_grid[x,y] == GridBuilder.CellType.None)
                    continue;


                if (pos1 == Vector2Int.zero 
                    && ((_grid[x, y] is GridBuilder.CellType.CornerOutSW or GridBuilder.CellType.Room 
                    && _grid[x + 1, y] == GridBuilder.CellType.WallSouth)
                    || (_grid[x, y] == GridBuilder.CellType.Hallway 
                    && _grid[x, y - 1] is GridBuilder.CellType.WallEast or GridBuilder.CellType.CornerOutSE)))
                {
                    pos1 = new Vector2Int(x, y);
                }
                else if (pos1 != Vector2Int.zero 
                    && ((_grid[x, y] is GridBuilder.CellType.CornerOutSE or GridBuilder.CellType.Room 
                    && _grid[x - 1, y] is GridBuilder.CellType.WallSouth or GridBuilder.CellType.Hallway)
                    || (_grid[x, y] == GridBuilder.CellType.Hallway 
                    && (_grid[x, y - 1] is GridBuilder.CellType.WallWest or GridBuilder.CellType.CornerOutSW))))
                {
                    pos2 = new Vector2Int(x, y);
                }

                if (pos1 != Vector2Int.zero && pos2 != Vector2Int.zero)
                {
                    _lines.Add(new LightLine(pos1, pos2, LightLine.LineDirection.WestToEast));
                    pos1 = Vector2Int.zero;
                    pos2 = Vector2Int.zero;
                }
            }

            pos1 = Vector2Int.zero;
            pos2 = Vector2Int.zero;
        }
    }
    
    private void MarkLineSouth()
    {
        Vector2Int pos1 = Vector2Int.zero, pos2 = Vector2Int.zero;

        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                if (pos1 == Vector2Int.zero && ((_grid[x, y] is GridBuilder.CellType.CornerOutNW or GridBuilder.CellType.Room && _grid[x + 1, y] == GridBuilder.CellType.WallNorth)
                                            || (_grid[x, y] == GridBuilder.CellType.Hallway && _grid[x, y + 1] is GridBuilder.CellType.WallEast or GridBuilder.CellType.CornerOutNE)))
                {
                    pos1 = new Vector2Int(x, y);
                }
                else if (pos1 != Vector2Int.zero && ((_grid[x, y] is GridBuilder.CellType.CornerOutNE or GridBuilder.CellType.Room && _grid[x - 1, y] == GridBuilder.CellType.WallNorth)
                                                    || (_grid[x, y] == GridBuilder.CellType.Hallway && (_grid[x, y + 1] is GridBuilder.CellType.WallWest or GridBuilder.CellType.CornerOutNW))))
                {
                    pos2 = new Vector2Int(x, y);
                }

                if (pos1 != Vector2Int.zero && pos2 != Vector2Int.zero)
                {
                    _lines.Add(new LightLine(pos1, pos2, LightLine.LineDirection.EasttoWest));
                    pos1 = Vector2Int.zero;
                    pos2 = Vector2Int.zero;
                }
            }

            pos1 = Vector2Int.zero;
            pos2 = Vector2Int.zero;
        }
    }
}
