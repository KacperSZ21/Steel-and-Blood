using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogReveal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.RegisterVisionSource(transform);
        }
    }

    void OnEnable() // Unit is created
    {
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.RegisterVisionSource(transform);
        }
    }

    void OnDisable() // Unit dies or destroy
    {
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.UnregisterVisionSource(transform);
        }
    }
}
