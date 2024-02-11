using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public ObjectData config;   // Scriptable Object reference
    public bool isMoving = false;

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
