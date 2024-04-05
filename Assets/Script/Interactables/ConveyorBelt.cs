using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class ConveyorBelt : Floor
{
    [SerializeField]
    private Direction beltDirection;
    [SerializeField]
    private SpriteRenderer arrowIndicator;
    [SerializeField]
    private float reattemptTime = 2.5f;
    // After how long will belt try to move the box on it again
    private Coroutine beltRunning;
    private Coroutine arroeIndicating;
    

    private void Start()
    {
        // TODO: will be replaced when art asset is present
        if (isElectrified)
        {
            Electrify();
        }
        else
        {
            StopElectrify();
        }
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

    public void ConveyObject(Interactable interactableObject)
    {
        if (!isElectrified)
        {
            return;
        }
        EventBus.Broadcast(EventTypes.ClearPlayerInteractBox);
        interactableObject.Move(beltDirection);
        // TODO: will be replaced with other methods
        EventBus.Broadcast<bool>(EventTypes.DisableInteraction, false);
    }

    public void MovePlayer()//TBD
    {
        if (!isElectrified)
        {
            return;
        }
        
    }

    public override bool Electrify()
    {
        if (canElectrified)
        {
            EnableBelt();
            return true;
        }
        return false;
    }

    public override void StopElectrify()
    {
        DisableBelt();
    }

    public void EnableBelt()
    {
        isElectrified = true;
        arrowIndicator.color = new Color(0.09f, 0.68f, 0.09f);
        arroeIndicating =  StartCoroutine(ActivateArrowEffectCor());
        beltRunning = StartCoroutine(RunningBeltCor());
    }
    
    public void DisableBelt()
    {
        isElectrified = false;
        if(arroeIndicating != null)
            StopCoroutine(arroeIndicating);
        arrowIndicator.DOFade(1, 3f);
        arrowIndicator.color = new Color(0.5f, 0.11f, 0.08f);
        if (beltRunning != null)
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
            //Debug.Log("Check Has Box");
            GameObject aboveObject = GridSystem.Instance.GetObjectAbove(gridPos);
            //Debug.Log("Above Object:"  + aboveObject);
            if (aboveObject)
            {
                Interactable interactableObject;
                ThirdPersonController player;
                aboveObject.TryGetComponent<Interactable>(out interactableObject);
                aboveObject.TryGetComponent<ThirdPersonController>(out player);
                if (interactableObject)
                {
                    ConveyObject(interactableObject);
                }else if (player)
                {
                    Debug.Log("Move Player");
                }
                
            }
            yield return  reattemptWait;
            //Box box = GridSystem.Instance.GetBoxAbove(gridPos);
            
        }
    }
}
