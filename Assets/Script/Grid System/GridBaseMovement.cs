using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using DG.Tweening;
public class GridBaseMovement : MonoBehaviour
{
    public static GridBaseMovement Instance;

    private void Start()
    {
        Instance = this;
    }

    public void MoveItem(GameObject obj, Direction dir, int moveID)
    {
        if (obj != null)
        {
            //StartCoroutine(MoveInstance(obj, dir, moveID));
            MoveObj(obj,dir,moveID);
        }
        else
        {
            EventBus.Broadcast<int>(EventTypes.ObjectMove, moveID);
        }
    }
    

    private void MoveObj(GameObject obj, Direction dir, int moveID)
    {
        float duration = 0.99f;
        //float timeElapsed = 0;
        Vector3 targetPath = Vector3.zero;
        switch (dir)
        {
            case Direction.N:
                targetPath = Vector3.left;
                break;
            case Direction.S:
                targetPath = Vector3.right;
                break;
            case Direction.E:
                targetPath = Vector3.forward;
                break;
            case Direction.W:
                targetPath = Vector3.back;
                break;
        }

        obj.transform.DOMove(obj.transform.position + targetPath, duration).OnComplete(() =>
        {
           Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(obj.transform.position);
           GridSystem.Instance.ObjectEndMoving(obj, gridPos.x, gridPos.y);
           EventBus.Broadcast<int>(EventTypes.ObjectMove, moveID); 
        });
        
    }
}
