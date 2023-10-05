using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightDecalHandler : MonoBehaviour
{
    [SerializeField] private DecalProjector projector;
    [SerializeField] private HDAdditionalLightData areaLight;

    private float lightWidth, lightHeight;

    private void Start()
    {
        lightWidth = areaLight.shapeWidth;
        lightHeight = areaLight.shapeHeight;
    }

    public void Initialize(Vector3 dimensions, float width)
    {
        projector.size = dimensions;
        projector.transform.rotation *= Quaternion.Euler(new Vector3(0, 90f, 0));
        areaLight.shapeWidth = width;
        // areaLight.intensity *= ((width * lightHeight) / (lightWidth * lightHeight));
    }
    public void Initialize(Vector3 dimensions, Vector3 rotation)
    {
        projector.size = dimensions;
        projector.transform.rotation *= Quaternion.Euler(rotation + new Vector3(0, 90f, 0));
    }

    public void Initialize(Vector3 dimensions, Vector3 rotation, float width)
    {
        projector.size = dimensions;
        projector.transform.rotation *= Quaternion.Euler(rotation + new Vector3(0, 90f, 0));
        areaLight.shapeWidth = width;
    }

    public void Initialize(float width)
    {

    }
}
