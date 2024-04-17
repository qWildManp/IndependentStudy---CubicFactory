using System;
using System.Collections;
using System.Collections.Generic;
using Script.Objects;
using UnityEngine;

public class Door : ControllerObjectBase
{
    public bool isOpen;

    private Animator doorAniamtor;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        doorAniamtor = GetComponent<Animator>();
    }
    
    public override void Activate()
    {
        isOpen = true;
        activated = true;
        powerIndicator.material = hasPowerMat;
        doorAniamtor.Play("DoorOpen");
    }

    public override void Deactivate()
    {
        isOpen = false;
        activated = false;
        powerIndicator.material = noPowerMat;
        doorAniamtor.Play("DoorClose");
    }
    
    
}
