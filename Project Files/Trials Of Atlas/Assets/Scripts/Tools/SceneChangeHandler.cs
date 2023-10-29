using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeHandler
{
    public delegate void SceneReload();
    public SceneReload OnReload;

    public static SceneChangeHandler Singleton;
}
