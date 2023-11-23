using System.Collections;
using System.Collections.Generic;
using AJK;
using Unity.VisualScripting;
using UnityEngine;

public class UIPage : MonoBehaviour
{

    public List<GameObject> hiddenPages;
    public GameObject parentPage;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        foreach (var pg in hiddenPages)
        {
            pg.SetActive(false);
        }
        var input = FindObjectOfType<PlayerManager>().inputHandler;
        input.Back = LeavePage;
    }

    private void LeavePage()
    {
        gameObject.SetActive(false);
        foreach (var pg in hiddenPages)
        {
            pg.SetActive(true);
        }
        parentPage.SetActive(true);
    }
}
