using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrasManager : ContentManager
{
    public int numberOfObjects;
    public int numberOfChests;
    public GameObject chest;
    public override List<GameObject> ChooseContent()
    {
        var result = new List<GameObject>();
        for(int i = 0; i < numberOfObjects; i++)
        {
            if(i < numberOfChests)
                result.Add(chest);
            else
            {
                var x = Random.Range(0, Content.Count);
                result.Add(Content[x]);
            }
        }
        return result;
    }
}
