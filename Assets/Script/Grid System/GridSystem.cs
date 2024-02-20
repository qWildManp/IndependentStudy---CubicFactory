using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }


    public Vector3 gridOrigin;
    public FloorDatabaseSO floorDatabase;
    public ObjectsDatabaseSO objectDatabase;
    public int rows = 10;
    public int columns = 10;
    public float cellSize = 1.0f; // Size of each grid cell

    public GridCell[,] gridArray;

    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridArray = new GridCell[rows, columns];

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
                if (!floor.GetComponent<Floor>().GetIsAccessable())
                {
                    gridArray[gridPos.x, gridPos.y].Floor.GetComponent<Floor>().SetAccessability(false);
                }
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
        return new Vector3(gridPos.x * cellSize + 0.5f, 0, gridPos.x * cellSize + 0.5f);
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
        return cell.Obj.GetComponent<Box>();
    }

    // Move Begin Update
    // Update isMoving status and disconnect the object with the original cell
    public bool ObjectStartMoving(int row, int column, Direction dir, int i)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            Vector2Int target = DirectionToPosition(row, column, dir);
            Debug.Log(target);
            GridCell neighbor = GetCell(target.x, target.y);
            if (neighbor != null && (neighbor.Floor == null || neighbor.Floor.GetComponent<Floor>().GetIsAccessable())
                   && neighbor.Obj == null)
            {
                GridBaseMovement.Instance.MoveItem(cell.Obj, dir, i);
                cell.Obj = null;
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
            cell.Obj = obj;
        };
    }




}
