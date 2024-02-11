using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    public FloorData config;

    // Start is called before the first frame update
    void Start()
    {
        ApplyConfig();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ApplyConfig()
    {
        if (config != null)
        {
            // Applying a property from the config

        }
        // Additional configuration
    }
}
