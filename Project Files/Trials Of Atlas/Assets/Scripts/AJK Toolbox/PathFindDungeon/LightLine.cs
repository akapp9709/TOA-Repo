using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLine
    {
        public Vector2Int StartPos, EndPos;
        public static float LightCount;
        public List<Vector3> Positions = new List<Vector3>();

        public enum LineDirection
        {
            SouthToNorth,
            WestToEast,
            NorthToSouth,
            EasttoWest
        }

        public LineDirection Direction;

        public LightLine(Vector2Int startPos, Vector2Int endPos, LineDirection direction)
        {
            StartPos = startPos;
            EndPos = endPos;
            Direction = direction;
        }

        public int NumberOfLights()
        {
            var dist = Vector2Int.Distance(StartPos, EndPos);
            var num = Mathf.RoundToInt(dist * LightCount);
            return num;
        }

        public float LightSpacing()
        {
            var dist = Vector2Int.Distance(StartPos, EndPos);
            return dist / NumberOfLights();
        }

        public Vector3 Midpoint
        {
            get
            {
                Vector3 vec = new Vector3();

                var p1 = new Vector3(StartPos.x, 0f, StartPos.y);
                var p2 = new Vector3(EndPos.x, 0f, EndPos.y);

                vec = (p1 + p2) / 2f;

                return vec;
            }
        }

        public float LineLength
        {
            get
            {
                return Vector2.Distance(StartPos, EndPos);
            }
        }
    }
