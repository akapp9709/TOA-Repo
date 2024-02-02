using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContentScoreManager
{
    private List<RoomVariant> roomVariants = new List<RoomVariant>();
    public static int RunsCompleted;
    public static float maxRoomValue, roomCount, curveOffset, curveWidth;
    public static List<RoomVariant> PrepareValues(List<GameObject> content)
    {
        var manager = new ContentScoreManager();
        List<RoomVariant> result = new List<RoomVariant>();
        
        foreach (var room in content)
        {
            var roomObj = room.GetComponent<PF_Room>();
            var length = roomObj.Variants.Count;

            for (int i = 0; i < length; i++)
            {
                var obj = new RoomVariant();
                obj.room = room;
                obj.formationIndex = i;
                roomObj.Variants[i].CalculateRoomValue();
                obj.roomValue = roomObj.Variants[i].formationValue;
                result.Add(obj);
            }
        }

        manager.roomVariants = result;
        manager.SortRooms();
        manager.CalculateBiases();
        manager.PrepareVariantLine();

        LineLength = manager.roomVariants[^1].shiftedBias;

        return result;
    }

    private List<RoomVariant> SortRooms()
    {
        for (int i = 0; i < roomVariants.Count - 1; i++)
        {
            for (int j = i + 1; j < roomVariants.Count; j++)
            {
                var x = roomVariants[i].roomValue;
                var y = roomVariants[j].roomValue;

                if (x > y)
                {
                    (roomVariants[i], roomVariants[j]) = (roomVariants[j], roomVariants[i]);
                }
            }
        }
        maxRoomValue = roomVariants[^1].roomValue;
        return roomVariants;
    }

    private void CalculateBiases()
    {
        var runNum = RunsCompleted;

        foreach (var room in roomVariants)
        {
            room.roomBias =
                Mathf.Exp(-(Mathf.Pow(((room.roomValue / maxRoomValue) - (runNum / roomCount) - curveOffset), 2) /
                            (2 * Mathf.Pow(curveWidth, 2) / 100)));

            if (room.roomBias < 0.01)
            {
                room.roomBias = 0;
            }
        }
    }

    private void PrepareVariantLine()
    {
        roomVariants[0].shiftedBias = roomVariants[0].roomBias;
        for (int i = 1; i < roomVariants.Count; i++)
        {
            roomVariants[i].shiftedBias += roomVariants[i - 1].shiftedBias + roomVariants[i].roomBias;
            // if (roomVariants[i].roomBias != 0)
            // {
            //     availableRooms++;
            // }
        }
    }

    public static float LineLength;
}
