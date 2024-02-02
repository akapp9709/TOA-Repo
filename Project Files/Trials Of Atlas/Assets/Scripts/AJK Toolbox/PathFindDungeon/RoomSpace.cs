using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpace
    {
        public RectInt bounds;
        public List<Vector2Int> roomEntrances = new List<Vector2Int>();

        public RoomSpace(Vector2Int location, Vector2Int size)
        {
            bounds = new RectInt(location, size);
        }

        public void AddEntrances(List<Transform> positions)
        {
            foreach (var t in positions)
            {
                TranslateAndAddPosition(t.position);
            }
        }

        public void TranslateAndAddPosition(Vector3 v)
        {
            var vec = new Vector2Int((int)v.x, (int)v.z);
            roomEntrances.Add(vec);
        }

        public Vector2Int GetClosestEntrance(Vector2Int pos)
        {
            var vec = roomEntrances[0];

            foreach (var v in roomEntrances)
            {
                if (Vector2Int.Distance(v, pos) <= Vector2Int.Distance(vec, pos))
                {
                    vec = v;
                }
            }

            return vec;
        }
    }
