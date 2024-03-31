using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ConveyorBelt : Floor
{
    [SerializeField]
    private Direction beltDirection;
    [SerializeField]
    private bool isActive = true;
    [SerializeField]
    private SpriteRenderer arrowIndicator;
    [SerializeField]
    private float reattemptTime = .5f;
    // After how long will belt try to move the box on it again
    private Coroutine beltRunning;
    private Coroutine arroeIndicating;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            EnableBelt();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            DisableBelt();
        }
    }

    private void Start()
    {
        if(isActive)
            EnableBelt();
        // TODO: will be replaced when art asset is present
        switch (beltDirection)
        {
            case Direction.N:
                break;
            case Direction.S:
                transform.localEulerAngles = new Vector3(0, 180, 0);
                break;
            case Direction.E:
                transform.localEulerAngles = new Vector3(0, 90, 0);
                break;
            case Direction.W:
                transform.localEulerAngles = new Vector3(0, 270, 0);
                break;
        }
    }

    // Everytime an object enters the belt, its automatically sent to the targeting direction
    // 1. Keep listening to information of item entering belt area
    // 2. Once event triggered, run Box.Move(), if it returns false, loop till its possible; if it returns true, continue waiting for the next box

    public void MoveBox(Box box)
    {
        if (!isActive)
        {
            return;
        }
        // In case box refuse to move, try every 
        /*if (!box.Move(beltDirection))
        {
            //StartCoroutine(ReattemptMoveBox(box));
        } else
        {
           
        }*/
        box.Move(beltDirection);
        // TODO: will be replaced with other methods
        EventBus.Broadcast(EventTypes.ClearPlayerInteractBox);
        EventBus.Broadcast<bool>(EventTypes.DisableInteraction, false);
    }

    public void EnableBelt()
    {
        isActive = true;
        arrowIndicator.color = new Color(0.09f, 0.68f, 0.09f);
        beltRunning =  StartCoroutine(ActivateArrowEffectCor());
        arroeIndicating = StartCoroutine(RunningBeltCor());
        /*
        Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
        Box box = GridSystem.Instance.GetBoxAbove(gridPos);
        if (box != null)
        {
            MoveBox(box);
        }*/
    }
    
    public void DisableBelt()
    {
        isActive = false;
        StopCoroutine(arroeIndicating);
        arrowIndicator.DOFade(1, 3f);
        arrowIndicator.color = new Color(0.5f, 0.11f, 0.08f);
        StopCoroutine(beltRunning);
    }

    private IEnumerator ActivateArrowEffectCor()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1.5f);
        while (true)
        {
            arrowIndicator.DOFade(0.3f, 1.5f);
            yield return waitTime;
            arrowIndicator.DOFade(1f, 1.5f);
            yield return waitTime;
        }
    }
    private IEnumerator RunningBeltCor()
    {
        WaitForSeconds reattemptWait = new WaitForSeconds(reattemptTime);
        Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
        while (true)
        {
            Debug.Log("Check Has Box");
            Box box = GridSystem.Instance.GetBoxAbove(gridPos);
            if (box != null)
            {
                MoveBox(box);
                yield return reattemptWait;
            }
            else
            {
                yield return reattemptWait;
            }
        }
    }

    private IEnumerator ReattemptMoveBox(Box box)
    {
        do
        {
            yield return new WaitForSeconds(reattemptTime);
        }
        while (!box.Move(beltDirection));
    }
}
