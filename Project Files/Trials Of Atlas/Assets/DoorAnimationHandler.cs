using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationHandler : MonoBehaviour
{
    private void Start()
    {
        GetComponentInParent<RoomBehavior>().PlayerEntry += CloseDoorAfterSeconds;
        GetComponentInParent<RoomBehavior>().RoomClear += OpenDoor;

    }

    private void CloseDoorAfterSeconds()
    {
        Invoke("CloseDoor", 1.5f);
    }

    private void OpenDoor()
    {
        GetComponent<Animator>().SetBool("isOpen", true);
    }

    private void CloseDoor()
    {
        GetComponent<Animator>().SetBool("isOpen", false);
    }
}
