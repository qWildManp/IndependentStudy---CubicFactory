using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GridBaseMovement : MonoBehaviour
{
    public float moveSpeed;
    public Transform movePoint;
    
    private void Start()
    {
        movePoint.parent = null;
    }

    IEnumerator MoveInstance(Direction dir, int moveID)
    {
        while (Vector3.Distance(transform.position, movePoint.position) > 0.05f)
        {

            yield return new WaitForFixedUpdate();
        }
        EventBus.Broadcast<int>(EventTypes.BoxMove, moveID);
    }

    private void Update()
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
                    if (targetCell.IsAccessible && targetCell.Obj == null)      // Allow to move
                    {
                        // Call ObjectStartMoving before moving, update isMoving status and disconnect with the original cell
                        GridSystem.Instance.ObjectStartMoving(gridPos.x, gridPos.y); 

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
                    if (targetCell.IsAccessible && targetCell.Obj == null)      // Allow to move
                    {
                        // Call ObjectStartMoving before moving, update isMoving status and disconnect with the original cell
                        GridSystem.Instance.ObjectStartMoving(gridPos.x, gridPos.y);
                        movePoint.position += new Vector3(-1 * Input.GetAxisRaw("Vertical"), 0f, 0f);   // Move movePoint
                    }

                }
            }
        }


    }
}
