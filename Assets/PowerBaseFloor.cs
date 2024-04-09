using System;
using System.Collections;
using System.Collections.Generic;
using Script.Objects;
using UnityEngine;

public class PowerBaseFloor : Floor
{
    public ControllerObjectBase[] controlledObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Battery>())
        {
            foreach (var controlled in controlledObjects)
            {
                controlled.Activate();
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Battery>())
        {
            foreach (var controlled in controlledObjects)
            {
                controlled.Deactivate();
            }
        }
    }
}
