using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
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
    protected bool canElectrified;
    [SerializeField] protected bool isElectrified; // Property to check if this unit have electricty

    private BoxCollider boxCollider;

    [SerializeField]private GameObject electrifyEffect;
    [SerializeField]private GameObject blockingCollider;

    private void Start()
    {
        try
        {
            electrifyEffect = transform.Find("Electrify").gameObject;
            blockingCollider = transform.Find("Collider").gameObject;
        }
        catch
        {
            Debug.LogWarning("Either effect or collider not detected");
        }
        
        if (!isElectrified)
        {
            StopElectrify();
        }
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

        if (isElectrified)
        {
            Electrify();
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

    public void SetFilledUp(bool canElectrify)
    {
        isHole = false;
        isAccessible = true;
        canElectrified = canElectrify;
        if (canElectrified)
        {
            try
            {
                blockingCollider = transform.Find("Collider").gameObject;
                electrifyEffect = transform.Find("Electrify").gameObject;
            }
            catch
            {
                Debug.LogWarning("Either effect or collider not detected");
            }
        }
        boxCollider.center = Vector3.zero;
    }

    public bool GetCanCharge()
    {
        return isCharger;
    }

    public virtual bool Electrify()
    {
        if (canElectrified)
        {
            isElectrified = true;
            // Enable visual effects
            if (electrifyEffect != null)
                electrifyEffect.SetActive(true);
            // TODO: Disable player from walking into
            if (blockingCollider != null)
                blockingCollider.SetActive(true);

            return true;
        }
        return false;
    }
    public virtual bool ForceElectrify()
    {
        
        Debug.Log(name + "ForceActive");
            isElectrified = true;
            // Enable visual effects
            if (electrifyEffect != null)
                electrifyEffect.SetActive(true);
            if (blockingCollider != null)
                blockingCollider.SetActive(true);
            return true;
    }
    public virtual void StopElectrify()
    {
        Debug.Log(name + " Deactive");
        if (electrifyEffect != null)
            electrifyEffect.SetActive(false);
        if (blockingCollider != null)
            blockingCollider.SetActive(false);
        isElectrified = false;
    }

    public bool GetIsElectrified()
    {
        return isElectrified;
    }



}
