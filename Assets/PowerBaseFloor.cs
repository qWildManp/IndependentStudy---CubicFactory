using System;
using System.Collections;
using System.Collections.Generic;
using Script.Objects;
using UnityEngine;

public class PowerBaseFloor : Floor
{
    public ControllerObjectBase[] controlledObjects;
    public bool useConductivity;
    public Floor EndWire;
    public Floor[] connectedWires;
    private void OnTriggerEnter(Collider other)
    {
        if (!useConductivity && other.GetComponent<Battery>())
        {
            if (connectedWires.Length > 0)// for each connected wire, make it electrify
            {
               foreach (var wire in connectedWires)
               {
                   wire.ForceElectrify();
               } 
            }
            
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
            if (connectedWires.Length > 0)// for each connected wire, make it stop electrify
            {
                foreach (var wire in connectedWires)
                {
                    wire.StopElectrify();
                } 
            }
            foreach (var controlled in controlledObjects)
            {
                controlled.Deactivate();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Battery>())
        {
            Debug.Log("Battery On");
            //Electrify();
            if (useConductivity&&EndWire && EndWire.GetIsElectrified())
            {
                if (controlledObjects.Length > 0)
                {
                   foreach (var controlled in controlledObjects)
                   {
                       controlled.Activate();
                   } 
                }
                    
            }
        }
    }
}
