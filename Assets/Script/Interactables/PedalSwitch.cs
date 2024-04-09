using Script.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedalSwitch : Floor
{
    public ControllerObjectBase[] controlledObjects;
    private void OnTriggerEnter(Collider other)
    {
        if (isElectrified && other.gameObject.CompareTag("Player"))
        {
            foreach (var controlled in controlledObjects)
            {
                controlled.Activate();
            }
        }
    }
}
