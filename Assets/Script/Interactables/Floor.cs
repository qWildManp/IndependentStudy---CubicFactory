using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Interactable
{
    [SerializeField]
    private bool isAccessible; // Decide whether player or box can step on the block
    private bool isElectrified;

    private void Start()
    {
        // TODO: Temporary solution for setting properties
        if (itemID == 5)
        {
            isElectrified = true;
        }
    }

    public bool GetIsAccessable()
    {
        return isAccessible;
    }

    public void SetAccessability(bool a)
    {
        isAccessible = a;
    }

    public bool GetCanCharge()
    {
        return isElectrified;
    }
}
