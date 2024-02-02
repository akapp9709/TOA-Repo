using System.Collections.Generic;
using UnityEngine;

public class RoomManager : ContentManager
{
    public int numberOfRooms = 10;
    [Header("Bias Curve Settings")]
    public float curveOffset;
    public float curveWidth;
    public List<RoomVariant> variants = new List<RoomVariant>();
    public int[] indices;

    public override List<GameObject> ChooseContent()
    {
        ContentScoreManager.curveWidth = curveWidth;
        ContentScoreManager.curveOffset = curveOffset;
        ContentScoreManager.roomCount = numberOfRooms;

        variants = ContentScoreManager.PrepareValues(Content);
        indices = new int[numberOfRooms];
        

        List<GameObject> result = new List<GameObject>();

        int ind = 0;

        for(int i = 0; i < numberOfRooms; i++)
        {
            var x = Random.Range(0, ContentScoreManager.LineLength);
            for (int j = 0; j < variants.Count; j++)
            {
                if (x > variants[j].shiftedBias)
                {
                    continue;
                }

                ind = j;
                break;
            }

            result.Add(variants[ind].room);
            indices[i] = ind;
        }
        Debug.Log($"{variants.Count} Variants Available     {indices.Length} chosen: {string.Join(", ", indices)}");
        return result;
    }
}
