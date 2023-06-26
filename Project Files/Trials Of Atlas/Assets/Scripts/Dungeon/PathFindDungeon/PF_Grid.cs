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
}
