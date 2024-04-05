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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void Activate()
    {
        activated = true;
        doorAniamtor.Play("DoorOpen");
    }

    public override void Deactivate()
    {
        activated = false;
        doorAniamtor.Play("DoorClose");
    }
    
    
}
