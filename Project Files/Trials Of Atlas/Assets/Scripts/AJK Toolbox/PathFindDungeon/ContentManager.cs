using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
    public List<GameObject> Content = new List<GameObject>();
    public virtual List<GameObject> ChooseContent(){return null;}
}
