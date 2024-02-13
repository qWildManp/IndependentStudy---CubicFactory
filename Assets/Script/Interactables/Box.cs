using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactable
{
    private static int moveIdentifier = 0;
    private int lastMoveIdentifier = -1;
    public bool isMoving { get; private set; } // This variable is only for player moving box, belt movement is seperate

    private void Update()
    {
        if (itemID == 1)
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
    public void Move(Direction dir)
    {
        if (isMoving)
            return;
        lastMoveIdentifier = moveIdentifier;
        Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
        GridSystem.Instance.ObjectStartMoving(gridPos.x, gridPos.y, dir, moveIdentifier++);
        isMoving = true;
        EventBus.AddListener<int>(EventTypes.BoxMove, StopMovement);
    }

    private void StopMovement(int i)
    {
        if (i == lastMoveIdentifier)
        {
            isMoving = false;
            EventBus.RemoveListener<int>(EventTypes.BoxMove, StopMovement);
            lastMoveIdentifier = -1;
        }
    }
}
