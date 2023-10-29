using System.Collections;
using System.Collections.Generic;
using AJK;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class LightControl : MonoBehaviour
{
    public LayerMask obstacleLayer;
    private PlayerManager _manger;
    private Light lightScript;
    public float range = 50f;
    public float minRange = 20f;
    private HDAdditionalLightData lightData;
    public int maxIntensity = 1000;
    // Start is called before the first frame update
    void Start()
    {
        _manger = PlayerManager.Singleton;
        lightScript = GetComponent<Light>();
        lightData = GetComponent<HDAdditionalLightData>();
    }

    // Update is called once per frame
    void Update()
    {
        bool lightActive = (Vector3.Distance(transform.position, _manger.transform.position) <= range && LineOfSight())
                                        || Vector3.Distance(transform.position, _manger.transform.position) <= minRange;
        lightScript.enabled = lightActive;

        if (lightActive)
        {
            lightData.intensity = Mathf.Lerp(0, maxIntensity, NormalizeValue(Vector3.Distance(transform.position, _manger.transform.position)));
        }
    }

    private bool LineOfSight()
    {
        Vector3 dir = _manger.transform.position - transform.position;
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform == _manger.transform)
            {
                return true;
            }
        }

        return false;
    }

    private float NormalizeValue(float value)
    {
        if (minRange == range)
            return 0f;
        else
        {
            return 1f - Mathf.Clamp01(value - minRange) / (range - minRange);
        }
    }
}
