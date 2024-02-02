using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnecterBuilder : MonoBehaviour
{
    public virtual void BuildConnections(GridBuilder grid)
    {

    }

    public virtual void BuildConnections(GridBuilder grid, HashSet<Prim.Edge> edges)
    {

    }
}
