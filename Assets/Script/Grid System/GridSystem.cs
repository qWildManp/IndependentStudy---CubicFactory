using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }
    public bool finishInit = false;
    public Vector2Int playerGridInfo;
    public Vector3 gridOrigin;
    //public FloorDatabaseSO floorDatabase;
    //public ObjectsDatabaseSO objectDatabase;
    public int rows = 10;
    public int columns = 10;
    public float cellSize = 1.0f; // Size of each grid cell

    public GridCell[,] gridArray;
    private bool[,] boxMovingTarget; // Controls whether the cell in the system is currently null, but will be receiving a box soon. If so, no more boxes should be allowed to move into the place.

    private void Awake()
    {
        finishInit = false;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //InitializeGrid();
    }

    public void UpdatePlayerGridInfo()
    {
        if (finishInit)
        {
            Vector3 playerPos =  GameManager.Instance.player.transform.position;
            Vector2Int newPlayerGridInfo = WorldToGridPosition(playerPos);
            if (newPlayerGridInfo != playerGridInfo)
            {
                GridCell oldPlayerCell =  GetCell(playerGridInfo.x, playerGridInfo.y);
                oldPlayerCell.Obj = null;
            }
            playerGridInfo = newPlayerGridInfo;
            GridCell playerCell = GetCell(playerGridInfo.x, playerGridInfo.y);
            playerCell.Obj = GameManager.Instance.player.gameObject;
        }
    }
    public void InitializeGrid()
    {
        gridArray = new GridCell[rows, columns];
        boxMovingTarget = new bool[rows, columns];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                gridArray[x, y] = new GridCell();
            }
        }

        // Find all objects tagged as "Floor" or "Box"
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Box");

        // Load floors
        foreach (var floor in floors)
        {
            Vector2Int gridPos = WorldToGridPosition(floor.transform.position);
            if (IsWithinGrid(gridPos))
            {
                gridArray[gridPos.x, gridPos.y].Floor = floor;
                /*if (!floor.GetComponent<Floor>().GetIsAccessable())
                {
                    gridArray[gridPos.x, gridPos.y].Floor.GetComponent<Floor>().SetAccessability(false);
                }*/
            }
        }

        // Load objects
        foreach (var obj in objects)
        {
            Vector2Int gridPos = WorldToGridPosition(obj.transform.position);
            if (IsWithinGrid(gridPos))
            {
                gridArray[gridPos.x, gridPos.y].Obj = obj;
            }
          
        }

        finishInit = true;
    }

    public GridCell GetCell(int row, int column)
    {
        if (row >= 0 && row < rows && column >= 0 && column < columns)
        {
            return gridArray[row, column];
        }
        return null; // Out of bounds
    }

    // Map world position to the gridPos
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPosition.z - gridOrigin.z) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize + 0.5f, 0, gridPos.y * cellSize + 0.5f);
    }

    bool IsWithinGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < rows && gridPos.y >= 0 && gridPos.y < columns;
    }

    public Vector2Int DirectionToPosition(int row, int column, Direction direction)
    {
        int targetRow = row;
        int targetColumn = column;

        switch (direction)
        {
            case Direction.N:
                targetRow -= 1;
                break;
            case Direction.E:
                targetColumn += 1;
                break;
            case Direction.S:
                targetRow += 1;
                break;
            case Direction.W:
                targetColumn -= 1;
                break;
            case Direction.NE:
                targetRow -= 1;
                targetColumn += 1;
                break;
            case Direction.SE:
                targetRow += 1;
                targetColumn += 1;
                break;
            case Direction.SW:
                targetRow += 1;
                targetColumn -= 1;
                break;
            case Direction.NW:
                targetRow -= 1;
                targetColumn -= 1;
                break;
        }

        return new Vector2Int(targetRow, targetColumn);
    }

    // Move Request
    // Given a grid pos, return the surrounding info (NW, N, NE, W, E, SW, S, SE)
    public List<GridCell> GetSurroundingCells(int row, int column)
    {
        List<GridCell> surroundingCells = new List<GridCell>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the current cell

                GridCell cell = GetCell(row + x, column + y);
                if (cell != null)
                {
                    surroundingCells.Add(cell);
                }
                else
                {
                    surroundingCells.Add(null);
                }
            }
        }
        return surroundingCells;
    }

    // Given a grid pos and a direction, return the target cell info
    public GridCell GetSurroundingCell(int row, int column, Direction direction)
    {
        Vector2Int result = DirectionToPosition(row, column, direction);
        GridCell cell = GetCell(result.x, result.y); 
        return cell != null ? cell : null;
    }

    public Floor GetFloorBelow(Vector2Int xy)
    {
        GridCell cell = GetCell(xy.x, xy.y);
        return cell.Floor == null? null : cell.Floor.GetComponent<Floor>();
    }

    public Box GetBoxAbove(Vector2Int xy)
    {
        GridCell cell = GetCell(xy.x, xy.y);
        Box foundBox;
        if (cell.Obj)
            cell.Obj.TryGetComponent<Box>(out foundBox);
        else
            return null;
        return foundBox;
    }
    public GameObject GetObjectAbove(Vector2Int xy)
    {
        GridCell cell = GetCell(xy.x, xy.y);
        if (cell.Obj)
            return cell.Obj;
        else
            return null;
    }
    // Move Begin Update
    // Update isMoving status and disconnect the object with the original cell
    public bool ObjectStartMoving(int row, int column, Direction dir, int i)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            Vector2Int target = DirectionToPosition(row, column, dir);
            //Debug.Log(target);
            GridCell neighbor = GetCell(target.x, target.y);
            // Detection is: neighbor cell should be inside boundary, it should have a floor, the floor should be accessible, and there should be no boxes on that position, or moving towards that position
            if (neighbor != null && (neighbor.Floor == null || neighbor.Floor.GetComponent<Floor>().GetIsAccessable() || neighbor.Floor.GetComponent<Floor>().GetIsHole())
                   && neighbor.Obj == null && !boxMovingTarget[target.x, target.y])
            {
                GridBaseMovement.Instance.MoveItem(cell.Obj, dir, i);
                cell.Obj = null;
                boxMovingTarget[target.x, target.y] = true;
                return true;
            }
        }
        return false;
    }


    // Move End Update
    // Update isMoving status and attach the object to the new cell
    public void ObjectEndMoving(GameObject obj, int row, int column)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            boxMovingTarget[row, column] = false;
            if (cell.Floor != null)
            {
                Floor cur = cell.Floor.GetComponent<Floor>();
                if (cur.GetIsHole())
                {
                    cur.SetFilledUp();
                    StartCoroutine(obj.GetComponent<Box>().BoxFallInPit());
                    return;
                }
            }
            cell.Obj = obj;
            // Once everything is registered, send signal to conveyor belts
            if (cell.Floor != null && cell.Floor.GetComponent<Floor>().itemID == FloorID.ConveyorBelt)
            {
                cell.Floor.GetComponent<ConveyorBelt>().ConveyBox(obj.GetComponent<Box>());
            }
        };
    }
    



}
