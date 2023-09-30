using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image foreground;

    [HideInInspector] public float fillValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreground.fillAmount = fillValue;
    }

    public void SetValues(float fill)
    {
        fillValue = fill;
    }
}
