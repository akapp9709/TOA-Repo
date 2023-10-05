using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    private PF_Generator _generator;
    public int roomsRemaining, numRooms;
    public List<RoomBehavior> rooms;

    public delegate void ProgressUpdate();
    public ProgressUpdate OnUpdate;

    private void Awake()
    {
        Singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _generator = PF_Generator.Singleton;
        _generator.OnComplete += InitializeGame;
    }

    private void InitializeGame()
    {
        numRooms = _generator.numberOfRooms;
        roomsRemaining = numRooms;

        Debug.Log($"Dungeon started with {numRooms} rooms");
        var arr = GetComponentsInChildren<RoomBehavior>();
        rooms = new List<RoomBehavior>(arr);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateProgress(RoomBehavior obj)
    {
        roomsRemaining--;
        OnUpdate?.Invoke();

        if (roomsRemaining == 0)
        {
            Debug.Log("Dungeon Cleared!");
        }

        rooms.Remove(obj);
    }
}