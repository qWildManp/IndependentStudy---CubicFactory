using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CamRootRotation : MonoBehaviour
{
    // Start is called before the first frame update
    private bool canRotate = true;
    public bool isRotating = false;
    public Transform followObj;
    public float rootOffset;
    void Awake()
    {
        EventBus.AddListener(EventTypes.CamRootClockWiseRotate,ClockWiseRotation);
        EventBus.AddListener(EventTypes.CamRootCounterClockWiseRotate,CounterClockWiseRotation);
    }

    private void OnDestroy()
    {
        EventBus.RemoveListener(EventTypes.CamRootClockWiseRotate,ClockWiseRotation);
        EventBus.RemoveListener(EventTypes.CamRootCounterClockWiseRotate,CounterClockWiseRotation);
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

        transform.position = followObj.position + new Vector3(0, rootOffset, 0);
        //transform.DOMove(followObj.position + new Vector3(0,rootOffset,0), 0.5f);
    }

    void ClockWiseRotation()
    {
        canRotate = false;
        isRotating = true;
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 90, 0), 1.5f).OnComplete(()=>
        {
            canRotate = true;
            isRotating = false;
        });
    }

    void CounterClockWiseRotation()
    {
        canRotate = false;
        isRotating = true;
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, -90, 0), 1.5f).OnComplete(()=>
        {
            canRotate = true;
            isRotating = false;
        });
    }

    void CamRootCustomRotation (Vector3 angle,float duration)
    {
        canRotate = false;
        transform.DORotate(angle, duration).OnComplete(()=>canRotate = true);
    }
}
