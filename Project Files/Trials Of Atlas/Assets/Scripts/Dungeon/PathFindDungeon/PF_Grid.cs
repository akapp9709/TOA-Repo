using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Grid<T>
{
    private T[] _data;
    
    public Vector2Int Size { get; private set; }
    public Vector2Int Offset { get; set; }

    public PF_Grid(Vector2Int size, Vector2Int offset)
    {
        Size = size;
        Offset = offset;

        _data = new T[size.x * size.y];
    }

    public int GetIndex(Vector2Int pos)
    {
        return pos.x + (Size.x * pos.y);
    }

    public bool InBounds(Vector2Int pos)
    {
        return new RectInt(Offset, Size).Contains(pos + Offset);
    }
    
    public T this[int x, int y] {
        get {
            return this[new Vector2Int(x, y)];
        }
        set {
            this[new Vector2Int(x, y)] = value;
        }
    }
    
    public T this[Vector2Int pos] {
        get {
            pos += Offset;
            return _data[GetIndex(pos)];
        }
        set {
            pos += Offset;
            _data[GetIndex(pos)] = value;
        }
    }
}
