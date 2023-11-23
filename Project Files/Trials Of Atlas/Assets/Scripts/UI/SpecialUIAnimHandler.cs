using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.UI;


public class SpecialUIAnimHandler : MonoBehaviour
{

    private Animator _anim;
    private PlayerInputCLass _input;
    private MultiSpecialHandler _handler;
    public Color ready, notReady;
    public List<Image> images;
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _input = FindObjectOfType<PlayerManager>().inputHandler;
        _handler = FindObjectOfType<MultiSpecialHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        _anim.SetBool("isPrimed", _input.specialAttackPrimed);

        SetImageColor(images[0], _handler.sp1);
        SetImageColor(images[1], _handler.sp2);
        SetImageColor(images[2], _handler.sp3);
    }

    private void SetImageColor(Image img, bool cond)
    {
        if (cond)
        {
            img.color = ready;
        }
        else if (!cond)
        {
            img.color = notReady;
        }
    }
}
