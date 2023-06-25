using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class WFC_DungeonGenerator : MonoBehaviour
{
    public enum Direction
    {
        North,
        South,
        East,
        West,
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast
    }

    [Serializable]
    public class RoomClass
    {
        public GameObject prefab;
        public int maximumAmount;
    }

    [Header("Dungeon")] 
    public int dungeonLength;
    
    [Header("Rooms")] 
    [SerializeField] private WFC_Room startingRoom;
    [SerializeField] private List<RoomClass> rooms;

    [Header("Misc")] 
    public int maxIterations;
    public int maxRetries;


    private GameObject _currentRoom;
    public List<GameObject> _placedRooms = new List<GameObject>();

    private Vector3 _currentPos, _currentRot;
    private GameObject _currentObj;

    private List<Vector3> _problemPositions = new List<Vector3>();
    
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.green;
        if (_problemPositions.Count < 1)
            return;
        foreach (var x in _problemPositions)
        {
            Gizmos.DrawSphere(x, 0.2f);
            Handles.Label(x + new Vector3(0, 2f, 0), "Collision Here");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int iterations = 0;
        var room = startingRoom;
        _currentRoom = room.gameObject;

        _currentPos = room.exitPosition;
        _currentRot = room.exitForward;

        for (int i = 0; i < dungeonLength; i++)
        {
            var tries = 0;
            var success = false;
            while (!success && iterations < maxIterations)
            {
                
                var val = Random.Range(0, rooms.Count);
                var prohibitDir = _currentRoom.GetComponent<WFC_Room>().unavailableDirections;
                var prohititType = _currentRoom.GetComponent<WFC_Room>().prohibitedRooms;
                
                var roomDir = rooms[val].prefab.GetComponent<WFC_Room>().exitDirection;
                var roomType = rooms[val].prefab.GetComponent<WFC_Room>().roomType;
                
                if (tries == maxRetries)
                {
                    Destroy(_currentObj);
                    _currentObj = _placedRooms[^2];
                    tries = 0;
                }
                
                if (rooms[val].maximumAmount > 0 && !prohititType.Contains(roomType) && !prohibitDir.Contains(roomDir))
                {
                    success = true;
                    _currentRoom = rooms[val].prefab;
                    _currentObj = Instantiate(_currentRoom.gameObject, _currentPos,Quaternion.Euler(_currentRot), this.transform) as GameObject;
                    room = _currentObj.GetComponent<WFC_Room>();

                    if (!room.CheckForOverlap())
                    {
                        Debug.Log("trying again");
                        // _problemPositions.Add(_currentPos);
                        Destroy(_currentObj);
                        success = false;
                        tries++;
                        continue;
                    }
                    rooms[val].maximumAmount -= 1;
                    _currentPos = room.exitPosition;
                    _currentRot = room.exitForward;
                    _placedRooms.Add(_currentObj);
                }

                if (success != true)
                {
                    iterations++;
                }
            }
        }

        foreach (var x in _placedRooms)
        {
            if (!x.GetComponent<WFC_Room>().CheckForOverlap())
            {
                _problemPositions.Add(x.GetComponent<WFC_Room>().roomCentre.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
