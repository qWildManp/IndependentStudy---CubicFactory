using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

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

        // Find all objects tagged as "Floor" or "Object"
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");

        // Load floors
        foreach (var floor in floors)
        {
            Vector2Int gridPos = WorldToGridPosition(floor.transform.position);
            if (IsWithinGrid(gridPos))
            {
                gridArray[gridPos.x, gridPos.y].Floor = floor;
                if (!floor.GetComponent<FloorController>().config.isAccessible)
                {
                    gridArray[gridPos.x, gridPos.y].IsAccessible = false;
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

    // Update is called once per frame
    void Update()
    {

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
        int x = Mathf.FloorToInt((worldPosition.x + (cellSize / 2)) / cellSize) - 1;
        int y = Mathf.FloorToInt((worldPosition.z + (cellSize / 2)) / cellSize) - 1;
        return new Vector2Int(x, y);
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
                if (cell != null && cell.IsAccessible)
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
        GridCell cell = GetCell(DirectionToPosition(row, column, direction).x, DirectionToPosition(row, column, direction).y); 
        return cell != null ? cell : null;
    }


    // Move Begin Update
    // Update isMoving status and disconnect the object with the original cell
    public void ObjectStartMoving(int row, int column)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            cell.Obj.GetComponent<ObjectController>().isMoving = true;
            // Debug.Log(cell.Obj.GetComponent<ObjectController>().isMoving);
            cell.Obj = null;
        }
    }


    // Move End Update
    // Update isMoving status and attach the object to the new cell
    public void ObjectEndMoving(GameObject obj, int row, int column)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            cell.Obj = obj;
            obj.GetComponent<ObjectController>().isMoving = false;
        };
    }




}
