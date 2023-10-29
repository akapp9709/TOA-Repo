using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightDecalHandler : MonoBehaviour
{
    [SerializeField] private HDAdditionalLightData areaLight;

    private float lightWidth, lightHeight;

    private void Start()
    {
        lightWidth = areaLight.shapeWidth;
        lightHeight = areaLight.shapeHeight;
    }

    public void Initialize(Vector3 dimensions, float width)
    {
        areaLight.shapeWidth = width;
        // areaLight.intensity *= ((width * lightHeight) / (lightWidth * lightHeight));
    }
    public void Initialize(Vector3 dimensions, Vector3 rotation)
    {

    }

    public void Initialize(Vector3 dimensions, Vector3 rotation, float width)
    {

        areaLight.shapeWidth = width;
    }

    public void Initialize(float width)
    {

    }
}
