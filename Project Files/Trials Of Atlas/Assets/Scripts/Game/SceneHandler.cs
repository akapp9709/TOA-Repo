using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void RestartDungeon()
    {
        SceneManager.LoadScene(1);
    }

    public static void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }
}
