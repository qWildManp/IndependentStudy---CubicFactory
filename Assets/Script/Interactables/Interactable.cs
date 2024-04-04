using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string itemName;
    protected static int moveIdentifier = 0;
    protected int lastMoveIdentifier = -1;
    protected bool stopIsCaptured = false;
    protected bool isDisabled = false;
    public bool isMoving { get; private set; } // This variable is only for player moving box, belt movement is seperate
    
    private void Awake()
    {
        EventBus.AddListener<int>(EventTypes.ObjectMove, StopMovement);
    }

    private void OnDestroy()
    {
        Vector2Int objectGridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
        GridSystem.Instance.ClearCellObjectData(objectGridPos.x, objectGridPos.y);
        EventBus.RemoveListener<int>(EventTypes.ObjectMove, StopMovement);
    }
    /// <summary>
    /// Player resulted box movements
    /// </summary>
    /// <param name="dir"></param>
    public virtual bool Move(Direction dir)
    {
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
        Debug.Log(name + " Get stop move");
        if (i == lastMoveIdentifier)
        {
            stopIsCaptured = true;
            isMoving = false;
            lastMoveIdentifier = -1;
        }
    }
}
