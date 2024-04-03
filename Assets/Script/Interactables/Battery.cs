using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Box
{
    public Transform BatteryIndicator;
    public float maxIndicatorScaleY = 1.8f;
    private float charge = 1; // The current amount of electricity inside battery, will decay overtime if connected to items
    [SerializeField]
    private float dischargePerSecond = .05f; // Battery amount -change per second
    [SerializeField]
    private float chargePerSecond = .2f; // Battery amount +change per second
    private bool isDischarging = false;
    private bool isCharging = false;

    private void Start()
    {
        BatteryIndicator.localScale = BatteryIndicator.localScale - new Vector3(0, BatteryIndicator.localScale.y, 0);
        StartCoroutine(BatteryBehavior());
    }

    IEnumerator BatteryBehavior()
    {
        while (true)
        {
            if (isDischarging)
            {
                charge -= dischargePerSecond * Time.fixedDeltaTime;
            }
            else if (isCharging)
            {
                charge += chargePerSecond * Time.fixedDeltaTime;
            }
            
            if (charge > 1)
            {
                charge = 1;
            } 
            else if (charge < 0)
            {
                // automatically disconnect from electric wiring if electricity used up
                charge = 0;
                isDischarging = false;
            }

            BatteryIndicator.localScale = new Vector3(BatteryIndicator.localScale.x, charge * maxIndicatorScaleY, BatteryIndicator.localScale.z);
            yield return new WaitForFixedUpdate();
        }
    }

    public override bool Move(Direction dir)
    {
        if (base.Move(dir))
        {
            isDischarging = false;
            isCharging = false;
            return true;
        } else
        {
            return false;
        }
    }

    protected override void StopMovement(int i)
    {
        base.StopMovement(i);
        if (stopIsCaptured)
        {
            Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
            Floor floor = GridSystem.Instance.GetFloorBelow(gridPos);
            if (floor != null && floor.GetCanCharge())
            {
                isCharging = true;
            }
            stopIsCaptured = false;

            Electrify();
        }
    }

    // Everytime battery is moved, detect again
    private void Electrify()
    {
        if (charge > 0)
        {
            Vector2Int pos = GridSystem.Instance.WorldToGridPosition(transform.position);
            GridSystem.Instance.CheckForElectricityRoute(pos);
        }
    }
}
