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
            EventBus.Broadcast<int>(EventTypes.BoxMove, moveID);
        }
    }

    /*private IEnumerator MoveInstance(GameObject obj, Direction dir, int moveID)
    {
        float duration = 0.99f;
        float timeElapsed = 0;
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
        while (timeElapsed < duration)
        {
            obj.transform.position += Time.fixedDeltaTime * targetPath;
            timeElapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(obj.transform.position);
        GridSystem.Instance.ObjectEndMoving(obj, gridPos.x, gridPos.y);
        EventBus.Broadcast<int>(EventTypes.BoxMove, moveID);
    }*/

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
           EventBus.Broadcast<int>(EventTypes.BoxMove, moveID); 
        });
        
    }
    /*private void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime); // Box moving towards movePoint

        Vector2Int gridPos = GridSystem.Instance.WorldToGridPosition(transform.position);   // Current gridPos of the box

        // Call ObjectEndMoving after moving, update isMoving status and attach to the new cell
        if (gameObject.GetComponent<ObjectController>().isMoving && transform.position == movePoint.position)
        {
            GridSystem.Instance.ObjectEndMoving(gameObject , gridPos.x, gridPos.y);     
        }

        


        if (gameObject.GetComponent<ObjectController>().isMoving == false && Vector3.Distance(transform.position,movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                Direction dir = Input.GetAxisRaw("Horizontal") > 0 ? Direction.E : Direction.W;     // Direction Mapping
                GridCell targetCell = GridSystem.Instance.GetSurroundingCell(gridPos.x, gridPos.y, dir); // Get the target cell
                if (targetCell != null)
                {
                    if (targetCell.Floor.GetComponent<Floor>().GetIsAccessable() && targetCell.Obj == null)      // Allow to move
                    {
                        // Call ObjectStartMoving before moving, update isMoving status and disconnect with the original cell
                        //GridSystem.Instance.ObjectStartMoving(gridPos.x, gridPos.y); 

                        movePoint.position += new Vector3(0f, 0f, Input.GetAxisRaw("Horizontal"));  // Move movePoint
                        
                    }
                    
                }
                
            }
            
            if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                Direction dir = Input.GetAxisRaw("Vertical") > 0 ? Direction.N : Direction.S;       // Direction Mapping
                GridCell targetCell = GridSystem.Instance.GetSurroundingCell(gridPos.x, gridPos.y, dir);    // Get the target cell
                if (targetCell != null)
                {
                    if (targetCell.Floor.GetComponent<Floor>().GetIsAccessable() && targetCell.Obj == null)      // Allow to move
                    {
                        // Call ObjectStartMoving before moving, update isMoving status and disconnect with the original cell
                        //GridSystem.Instance.ObjectStartMoving(gridPos.x, gridPos.y);
                        movePoint.position += new Vector3(-1 * Input.GetAxisRaw("Vertical"), 0f, 0f);   // Move movePoint
                    }

                }
            }
        }


    }*/
}
