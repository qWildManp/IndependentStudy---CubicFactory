using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Box
{
    private float charge = 1; // The current amount of electricity inside battery, will decay overtime if connected to items
    [SerializeField]
    private float dischargePerSecond = .05f; // Battery amount change per second
    private int batteryStatus = 0; //1=charge, 0=not in use, -1 = discharge

    private void Start()
    {
        StartCoroutine(BatteryBehavior());
    }

    IEnumerator BatteryBehavior()
    {
        while (true)
        {
            charge = charge + batteryStatus * dischargePerSecond * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Discharge battery overtime if connected to circuit
    /// </summary>
    public void Discharge()
    {
        batteryStatus = -1;
    }

    /// <summary>
    /// Charge battery overtime if connected charger
    /// </summary>
    public void Charge()
    {
        batteryStatus = 1;
    }

    public void NotInUse()
    {
        batteryStatus = 0;
    }


}
