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
    private void Awake()
    {
        doorAniamtor = GetComponent<Animator>();
    }
    
    public override void Activate()
    {
        isOpen = true;
        activated = true;
        doorAniamtor.Play("DoorOpen");
    }

    public override void Deactivate()
    {
        isOpen = false;
        activated = false;
        doorAniamtor.Play("DoorClose");
    }
    
    
}
