using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public PlayerManager _player;
    private PlayerInputCLass _input;
    [SerializeField] private RectTransform buttonGroup;
    [SerializeField] private GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        _input = _player.inputHandler;
        _input.OpenMenu = OpenMenu;
        _input.Back = Back;
    }

    void OnEnable()
    {
        if (_input == null)
        {
            return;
        }

        Debug.Log("Enabling menu");
        _input.Back = Back;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OpenMenu()
    {
        menu.SetActive(true);
        _input.EnableUIInput();
        _input.DisableMainInput();
        _input.DisableMainPlayerActions();
        menu.GetComponentInChildren<Button>().Select();
    }

    public void Back()
    {
        menu.SetActive(false);
        _input.DisableUIInput();
        _input.EnableMainInput();
        _input.EnableMainPlayerActions();
    }

    public void OpenPage(GameObject page)
    {
        page.SetActive(true);
    }
}
