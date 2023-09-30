using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightDecalHandler : MonoBehaviour
{
    [SerializeField] private DecalProjector projector;
    public void Initialize(Vector3 dimensions)
    {
        projector.size = dimensions;
    }
}
