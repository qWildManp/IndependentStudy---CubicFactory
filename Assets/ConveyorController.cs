using System.Collections;
using System.Collections.Generic;
using Script.Objects;
using UnityEngine;

public class ConveyorController : ControllerObjectBase
{
    public ConveyorBelt[] controlledConveyorBelts;
    
    // Start is called before the first frame update

    public override void Activate()
    {
        activated = true;
        if(powerIndicator)
            powerIndicator.material = hasPowerMat;
        foreach (var belt in controlledConveyorBelts)
        {
            belt.EnableBelt();
        }
    }
    public override void Deactivate()
    {
        activated = false;
        if(powerIndicator)
            powerIndicator.material = noPowerMat;
        foreach (var belt in controlledConveyorBelts)
        {
            belt.DisableBelt();
        }
    }
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Activate();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Deactivate();
        }
    }
}
