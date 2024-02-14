using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Interactable
{
    [SerializeField]
    private bool isAccessible; // Decide whether player or box can step on the block

    public bool GetIsAccessable()
    {
        return isAccessible;
    }

    public void SetAccessability(bool a)
    {
        isAccessible = a;
    }
}
