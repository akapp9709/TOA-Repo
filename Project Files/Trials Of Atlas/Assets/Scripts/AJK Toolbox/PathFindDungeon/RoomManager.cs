using System.Collections.Generic;
using UnityEngine;

public class RoomManager : ContentManager
{
    public int numberOfRooms = 10;
    

    public override List<GameObject> ChooseContent()
    {
        List<GameObject> result = new List<GameObject>();

        for(int i = 0; i < numberOfRooms; i++)
        {
            var x = Random.Range(0, Content.Count);
            result.Add(Content[x]);
        }

        return result;
    }
}
