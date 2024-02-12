using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactable
{
    private static int moveIdentifier = 0;
    private int lastMoveIdentifier = 0;
    public bool isMoving { get; private set; }


    public void Move(Direction dir)
    {
        lastMoveIdentifier = moveIdentifier;
        // GridBaseMovement.StartMoving(dir, moveIdentifier ++);
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
