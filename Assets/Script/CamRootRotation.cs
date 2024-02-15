using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CamRootRotation : MonoBehaviour
{
    // Start is called before the first frame update
    private bool canRotate = true;
    void Awake()
    {
        EventBus.AddListener(EventTypes.CamRootClockWiseRotate,ClockWiseRotation);
        EventBus.AddListener(EventTypes.CamRootCounterClockWiseRotate,CounterClockWiseRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(canRotate)
                EventBus.Broadcast(EventTypes.CamRootClockWiseRotate);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(canRotate)
                EventBus.Broadcast(EventTypes.CamRootCounterClockWiseRotate);
        }
    }

    void ClockWiseRotation()
    {
        canRotate = false;
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 90, 0), 1.5f).OnComplete(()=>canRotate = true);
    }

    void CounterClockWiseRotation()
    {
        canRotate = false;
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, -90, 0), 1.5f).OnComplete(()=>canRotate = true);
    }

    void CamRootCustomRotation (Vector3 angle,float duration)
    {
        canRotate = false;
        transform.DORotate(angle, duration).OnComplete(()=>canRotate = true);
    }
}
