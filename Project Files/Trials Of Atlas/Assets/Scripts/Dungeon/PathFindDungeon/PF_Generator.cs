using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Generator : MonoBehaviour
{
    public enum CellType
    {
        None,
        Room,
        Hallway
    }
    
    [SerializeField] private List<GameObject> roomPrefabs;
    private PF_Grid<CellType> _grid;
    
    // Start is called before the first frame update
    void Start()
    {
        PlaceRooms();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaceRooms()
    {
        
    }
}
