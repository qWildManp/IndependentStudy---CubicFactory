using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactable
{
    public BoxID itemID;
    protected static int moveIdentifier = 0;
    protected int lastMoveIdentifier = -1;
    protected bool stopIsCaptured = false;
    protected bool isDisabled = false;
    public bool isMoving { get; private set; } // This variable is only for player moving box, belt movement is seperate

    private void Awake()
    {
        EventBus.AddListener<int>(EventTypes.BoxMove, StopMovement);
    }

    private void OnDestroy()
    {
        EventBus.RemoveListener<int>(EventTypes.BoxMove, StopMovement);
    }

    private void Update()
    {
        if (!isDisabled && itemID == BoxID.Debug)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            if (x != 0)
            {
                Move(x == 1 ? Direction.E : Direction.W);
            } else if (y != 0)
            {
                Move(y == 1 ? Direction.N : Direction.S);
            }
        }
    }

    /// <summary>
    /// Player resulted box movements
    /// </summary>
    /// <param name="dir"></param>
    public virtual bool Move(Direction dir)
    {
        Debug.Log("Move");
        if (isMoving || isDisabled)
            return false;
        lastMoveIdentifier = moveIdentifier;
        Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
        //Debug.Log(moveIdentifier);
        if (GridSystem.Instance.ObjectStartMoving(gridPos.x, gridPos.y, dir, moveIdentifier++))
        {
            isMoving = true;
            return true;
        }
        return false;
        
    }

    protected virtual void StopMovement(int i)
    {
        if (i == lastMoveIdentifier)
        {
            stopIsCaptured = true;
            isMoving = false;
            //EventBus.RemoveListener<int>(EventTypes.BoxMove, StopMovement);
            lastMoveIdentifier = -1;
        }
    }

    // Box disappear after falling into pit
    public IEnumerator BoxFallInPit()
    {
        isDisabled = true;
        float elapsed = 0;
        while (elapsed < .5f)
        {
            transform.position -= 2 * Time.fixedDeltaTime * Vector3.up;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // TODO: will be replaced with other methods
        EventBus.Broadcast(EventTypes.ClearPlayerInteractBox);
        EventBus.Broadcast<bool>(EventTypes.DisableInteraction, false);
        Destroy(gameObject);
    }
}
