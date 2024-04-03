using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Interactable
{
    public FloorID itemID;
    [SerializeField]
    [Tooltip("Whether box & player can step onto")]
    private bool isAccessible;
    private bool isCharger;
    private bool isHole; // Box can be pushed into holes to make it normal floor

    // The following variables are used for examining electricity flow
    [SerializeField]
    [Tooltip("Whether this floor will be affected by battery above, or floor with electricity nearby")]
    private bool hasWireProperties;
    public bool isElectrified; // Property to check if this unit have electricty

    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        // TODO: Temporary solution for setting properties
        if (itemID == FloorID.ChargingStation)
        {
            isCharger = true;
        }
        if (itemID == FloorID.Hole)
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
        return isCharger;
    }

    public bool Electrify()
    {
        if (hasWireProperties)
        {
            isElectrified = true;
            return true;
        }
        return false;
    }

    public void StopElectrify()
    {
        isElectrified = false;
    }

    public bool GetIsElectrified()
    {
        return isElectrified;
    }



}
