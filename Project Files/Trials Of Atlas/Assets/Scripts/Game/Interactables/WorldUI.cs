
using AJK;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class WorldUI : InteractibleBehaivor
{
    public CinemachineVirtualCamera vCamera;
    private PlayerInputCLass _input;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        _input = FindObjectOfType<PlayerManager>().inputHandler;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Interact()
    {
        base.Interact();

        FindObjectOfType<CameraSwitchHandler>().ChangeToCamera(vCamera);
    }
}
