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
    public virtual bool Move(Direction dir)// GRID SYSTEM Move
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
    
    public virtual bool Move(Vector2 pushDir)// Player Interact Move //complicated shit
    {
        if (isMoving || isDisabled)
            return false;
        lastMoveIdentifier = moveIdentifier;
        Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
        //Debug.Log(moveIdentifier);
        RaycastHit hit;
        Ray castingRay = new Ray(transform.position + new Vector3(0,1,0),new Vector3(pushDir.x,0,pushDir.y));
        if (Physics.Raycast(castingRay, out hit, maxDistance: GridSystem.Instance.cellSize)) // if there is no space to move
        {
            Debug.Log("Hit" +  hit.transform.name);
            return false;
        }

        Direction converted_dir = ComputeDirBasedOnVector(pushDir);
        if (GridSystem.Instance.ObjectStartMoving(gridPos.x, gridPos.y, converted_dir, moveIdentifier++))
        {
            isMoving = true;
            return true;
        }
        return false;
        
    }

    protected virtual void StopMovement(int i)
    {
        //Debug.Log(name + " Get stop move");
        if (i == lastMoveIdentifier)
        {
            stopIsCaptured = true;
            isMoving = false;
            lastMoveIdentifier = -1;
        }
    }
    private Direction ComputeDirBasedOnVector(Vector2 dir)
    {
        if (dir.x != 0 && Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x < 0)
                return Direction.N;
                
            return Direction.S;
        }
        else if (dir.y != 0 && Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y < 0)
                return Direction.W;
                
            return Direction.E;
        }
        else
        {
            return Direction.None;
        }
    }
}
