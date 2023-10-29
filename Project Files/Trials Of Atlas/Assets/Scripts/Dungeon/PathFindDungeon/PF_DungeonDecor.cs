using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
This class handles the dungeon decoration, it uses PF_Grid to determine positions for decoration objects.
*/
public class PF_DungeonDecor
{
    private PF_Grid<PF_Generator.CellType> _grid;
    private float _maxWidth, _density;
    public List<Hall> positions { get; private set; }
    public PF_DungeonDecor(PF_Grid<PF_Generator.CellType> grid, float maxWidth, float decorDensity)
    {
        _grid = grid;
        _maxWidth = maxWidth;
        _density = decorDensity;
        positions = new List<Hall>();

        Debug.Log(_grid.Size);

        MapPositions();
    }

    public struct Hall
    {
        public Vector3 start, end;
        public float width;
    }

    public void MapPositions()
    {
        int maxX = _grid.Size.x;
        int maxY = _grid.Size.y;

        var pos1 = Vector3.zero;
        var pos2 = Vector3.zero;

        float width = 0;

        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                if (pos1 == Vector3.zero && _grid[i, j] == PF_Generator.CellType.WallWest)
                {

                    if (OppositeWallX(new Vector2Int(i, j), _maxWidth, out width))
                    {
                        pos1 = new Vector3(i + width, 0, j);
                    }
                }
                else if (pos1 != Vector3.zero && _grid[i, j] == PF_Generator.CellType.WallWest)
                {
                    if (!OppositeWallX(new Vector2Int(i, j + 1), _maxWidth, out width))
                    {
                        pos2 = new Vector3(i + width, 0, j);
                    }
                }

                if (pos1 != Vector3.zero && pos2 != Vector3.zero)
                {
                    var hall = new Hall();
                    hall.start = pos1;
                    hall.end = pos2;
                    hall.width = width;
                    positions.Add(hall);
                    Debug.Log("Wall Added");
                    pos1 = Vector3.zero;
                    pos2 = Vector3.zero;
                }
            }
            pos1 = Vector3.zero;
            pos2 = Vector3.zero;
        }
    }

    private bool OppositeWallX(Vector2Int startPos, float width, out float outWidth)
    {
        for (int i = 0; i < _maxWidth; i++)
        {
            if (_grid[startPos.x + i, startPos.y] == PF_Generator.CellType.WallEast)
            {
                Debug.DrawLine(
                    new Vector3(startPos.x, 6f, startPos.y),
                    new Vector3(startPos.x + i, 6f, startPos.y),
                    Color.green,
                    60f
                );
                outWidth = i / 2;
                return true;
            }
        }

        outWidth = 0;
        return false;
    }

    public List<Vector3> PlaceOnLineX(float density, Vector3 start, Vector3 end)
    {
        var dist = Vector3.Distance(start, end);
        var numDecor = NumberofDecorations(dist, density);

        if (numDecor < 2)
            return null;

        var list = new List<Vector3>();
        for (int i = 0; i < numDecor; i++)
        {
            var newPos = new Vector3(start.x, start.y, start.z + DecorSpacing(dist, numDecor) * i);
            list.Add(newPos);
        }
        return list;
    }

    public int NumberofDecorations(float dist, float numObj)
    {
        var num = Mathf.RoundToInt(dist * numObj);
        return num;
    }

    public float DecorSpacing(float dist, int numObj)
    {
        return dist / (float)numObj;
    }
}
