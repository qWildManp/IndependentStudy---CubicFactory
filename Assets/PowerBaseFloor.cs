using System;
using System.Collections;
using System.Collections.Generic;
using Script.Objects;
using UnityEngine;

public class PowerBaseFloor : Floor
{
    // Start is called before the first frame update
    public bool charged;
    public ControllerObjectBase[] controlledObjects;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Battery>())// battery is on
        {
            charged = true;
            foreach (var controlled in controlledObjects)
            {
                controlled.Activate();
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Battery>())// battery is on
        {
            charged = false;
            foreach (var controlled in controlledObjects)
            {
                controlled.Deactivate();
            }
        }
    }
}
