using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    public ConveyorBelt[] controlledConveyorBelts;

    public bool activated;
    // Start is called before the first frame update
    void Start()
    {
        if (activated)
        {
            ActivateBelt();
        }
        Debug.Log(controlledConveyorBelts.Length);
    }

    public void ActivateBelt()
    {
        activated = true;
        foreach (var belt in controlledConveyorBelts)
        {
            belt.EnableBelt();
        }
    }
    public void Deactivate()
    {
        activated = false;
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
            ActivateBelt();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Deactivate();
        }
    }
}
