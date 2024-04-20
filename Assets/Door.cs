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
    protected void Awake()
    {
        doorAniamtor = GetComponent<Animator>();
    }
    
    public override void Activate()
    {
        if(!isOpen)
            doorAniamtor.Play("DoorOpen");
        isOpen = true;
        activated = true;
        powerIndicator.material = hasPowerMat;
        
    }

    public override void Deactivate()
    {
        if(isOpen)
            doorAniamtor.Play("DoorClose");
        isOpen = false;
        activated = false;
        powerIndicator.material = noPowerMat;
       
    }
    
    
}
