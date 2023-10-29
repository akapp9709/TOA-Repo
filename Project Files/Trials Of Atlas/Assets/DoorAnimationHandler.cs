using UnityEngine;

public class DoorAnimationHandler : MonoBehaviour
{
    private bool roomIsClear = false;
    private void Start()
    {
        GetComponentInParent<RoomBehavior>().PlayerEntry += CloseDoorAfterSeconds;
        GetComponentInParent<RoomBehavior>().RoomClear += OpenDoorDelay;
    }

    private void CloseDoorAfterSeconds()
    {
        Invoke("CloseDoor", 1.5f);
    }

    private void OpenDoorDelay()
    {
        Invoke("OpenDoor", 1f);
    }

    private void OpenDoor()
    {
        GetComponent<Animator>().SetBool("isOpen", true);
        roomIsClear = true;
    }

    private void CloseDoor()
    {
        if (roomIsClear)
        {
            return;
        }
        GetComponent<Animator>().SetBool("isOpen", false);
    }
}
