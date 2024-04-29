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

    [SerializeField]
    private GameObject steelBoxPrefab;

    public GridCell[,] gridArray;


    private bool[,] boxMovingTarget; // Controls whether the cell in the system is currently null, but will be receiving a box soon. If so, no more boxes should be allowed to move into the place.

    private List<Floor> electrifiedFloor;
    private Vector2Int lastBatteryPos = new Vector2Int(-1, -1);

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

    private void Start()
    {
        
    }


    // Start is called before the first frame update
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
        GameObject[] objects = GameObject.FindGameObjectsWithTag("DynamicObject");

        // Load floors
        foreach (var floor in floors)
        {
            Vector2Int gridPos = WorldToGridPosition(floor.transform.position);
            if (IsWithinGrid(gridPos))
            {
                if (gridArray[gridPos.x, gridPos.y].Floor != null)
                {
                    Debug.LogWarning(floor.name+" is overlap with" + gridArray[gridPos.x, gridPos.y].Floor.name);
                }
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

        for(int i = 0; i < rows; i++)//when finish init then check for Route
        {
            for (int j = 0; j < columns; j++)
            {
                if (gridArray[i,j].Obj != null && gridArray[i, j].Obj.GetComponent<Box>() != null && gridArray[i, j].Obj.GetComponent<Box>().itemID == BoxID.Battery)
                {
                    CheckForNewRoute(new Vector2Int(i, j));
                }
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

    public bool ClearCellObjectData(int row, int column)
    {
        if (row >= 0 && row < rows && column >= 0 && column < columns)
        {
            gridArray[row, column].Obj = null;
            if (GetCell(row, column) != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;
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
                // Deal with special case that the floor is hole, fill it up
                if (cur.GetIsHole())
                {
                    bool canE = obj.GetComponent<Box>().GetCanElectrify();
                    Debug.Log(cur.gameObject.name+" Fill inHole");
                    cur.SetFilledUp(canE);
                    if (canE)
                    {
                        if (lastBatteryPos.x != -1)
                        {
                            CheckForElectricityRoute(lastBatteryPos);
                        }
                    }
                    StartCoroutine(obj.GetComponent<Box>().BoxFallInPit());

                    return;
                }
            }
            cell.Obj = obj;
        };
    }

    //Electrify all the floors connected to the battery
    public void CheckForElectricityRoute(Vector2Int pos)
    {
        if (electrifiedFloor == null)
        {
            electrifiedFloor = new();
        } else
        {
            // First stop electricity in all previous floors
            foreach (Floor f in electrifiedFloor)
            {
                f.StopElectrify();
            }
            electrifiedFloor.Clear();
        }
        CheckForNewRoute(pos);
    }

    private void CheckForNewRoute(Vector2Int pos)
    {
        lastBatteryPos = pos;
        if (electrifiedFloor == null)
        {
            electrifiedFloor = new();
        }
        // Loop through all potentials
        Queue<Vector2Int> q = new();
        HashSet<Vector2> visited = new();
        q.Enqueue(pos);
        while (q.Count > 0)
        {
            Vector2Int p = q.Dequeue();
            if (gridArray[p.x, p.y].Floor != null)
            {
                Floor f = gridArray[p.x, p.y].Floor.GetComponent<Floor>();
                if (f.Electrify())
                {
                    electrifiedFloor.Add(f);

                    List<Vector2Int> l = GetNeighbors(p);
                    foreach (Vector2Int l2 in l)
                    {
                        if (!visited.Contains(l2))
                            q.Enqueue((Vector2Int)l2);
                    }
                }
            }
            visited.Add(p);
        }
    }

    // x-1,x+1,y-1,y+1
    public List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbor = new()
        {
            // Right
            new Vector2Int(pos.x + 1, pos.y),
            // Left
            new Vector2Int(pos.x - 1, pos.y),
            // Up
            new Vector2Int(pos.x, pos.y + 1),
            // Down
            new Vector2Int(pos.x, pos.y - 1)
        };
        for (int i = 0; i < neighbor.Count; i++)
        {
            if (neighbor[i].x < 0 || neighbor[i].x >= rows || neighbor[i].y < 0 || neighbor[i].y >= columns)
            {
                neighbor.RemoveAt(i);
                i--;
            }
        }

        return neighbor;
    }

    // Providing a convertion of normal box into steel ones on a given grid position
    public Interactable ConvertBoxType(Vector2Int pos)
    {
        GameObject box = GetObjectAbove(pos);
        if (box != null && box.GetComponent<Box>().itemID == BoxID.Box)
        {
            Destroy(box);
            GameObject newBox = Instantiate(steelBoxPrefab, GridToWorldPosition(pos) + Vector3.up * .5f, Quaternion.identity).gameObject;
            gridArray[pos.x, pos.y].Obj = newBox;
            return newBox.GetComponent<Interactable>();
        }
        // If the above condition is not satisfied, return the original
        return box == null? null : box.GetComponent<Interactable>();
    }
}
