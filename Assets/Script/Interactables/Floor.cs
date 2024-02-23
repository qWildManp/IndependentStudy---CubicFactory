using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Interactable
{
    [SerializeField]
    private bool isAccessible; // false=box&player cannot step onto
    private bool isElectrified;
    private bool isHole; // Box can be pushed into holes to make it normal floor

    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        // TODO: Temporary solution for setting properties
        if (itemID == 5)
        {
            isElectrified = true;
        }
        if (itemID == 3)
        {
            isHole = true;
            isAccessible = false;
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

    public bool GetIsHole()
    {
        return isHole;
    }

    public void SetFilledUp()
    {
        isHole = false;
        isAccessible = true;
        boxCollider.center = Vector3.zero;
    }

    public bool GetCanCharge()
    {
        return isElectrified;
    }
}
