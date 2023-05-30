using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    right,
    up,
    left,
    down
}
public class GridManager : Singleton<GridManager>
{
    static float tileSize = 1;
    public enum GridType
    {
        passable,
        nonPassable
    }

    public int Rows = 0;
    public int Columns = 0;
    public float Scale = tileSize;
    public GameObject GridPrefab;
    public Dictionary<Vector2Int, GameObject> GridArray = new Dictionary<Vector2Int, GameObject>();
    public Mimicry player;
    void SetDistance(int fromX, int fromY)
    {
        // reset the current grid to make them all 'unvisited'
        foreach (GameObject obj in GridArray.Values)
        {
            if (obj)
            {
                obj.GetComponent<GridStats>().Visited = -1;
            }
        }

        // set the start position as steps to get there == 0
        GridArray[new Vector2Int(fromX, fromY)].GetComponent<GridStats>().Visited = 0;

        // work out the number of steps to get to each grid position object
        for (int step = 1; step < Rows * Columns; step++)
        {
            foreach (GameObject obj in GridArray.Values)
            {
                if (obj && obj.GetComponent<GridStats>().Visited == step - 1)
                {
                    TestFourDirections(obj.GetComponent<GridStats>().X, obj.GetComponent<GridStats>().Y, step);
                }
            }
        }
    }

    void SetVisited(int x, int y, int step)
    {
        if (GridArray[new Vector2Int(x, y)])
        {
            GridArray[new Vector2Int(x, y)].GetComponent<GridStats>().Visited = step;
        }
    }

    void TestFourDirections(int x, int y, int step)
    {
        if (TestDirection(x, y, -1, Direction.up))
        {
            SetVisited(x, y + 1, step);
        }

        if (TestDirection(x, y, -1, Direction.right))
        {
            SetVisited(x + 1, y, step);
        }

        if (TestDirection(x, y, -1, Direction.down))
        {
            SetVisited(x, y - 1, step);
        }

        if (TestDirection(x, y, -1, Direction.left))
        {
            SetVisited(x - 1, y, step);
        }
    }
    public static Vector3 indexToPosition((int,int) ind)
    {
        return new Vector2(ind.Item1 + 0.5f, ind.Item2+0.5f) * tileSize;
    }
    public static Vector3 indexToPosition(Vector2Int ind)
    {
        return new Vector2(ind.x + 0.5f, ind.y+0.5f) * tileSize;
    }


    public static Vector3 indexToPosition(Vector3 ind)
    {
        return new Vector2(ind.x + 0.5f, ind.y + 0.5f) * tileSize;
    }

    public static Vector3 PositionToIndex((float, float) pos)
    {
        return new Vector2((int)(pos.Item1 /tileSize- 0.5f), (int)(pos.Item2 /tileSize - 0.5f));

    }
    public static Vector3 PositionToIndex(Vector3 pos)
    {
        return new Vector2((int)(pos.x / tileSize - 0.5f), (int)(pos.y / tileSize - 0.5f));

    }
    public static Vector2Int PositionToIndexPair(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x / tileSize - 0.5f), Mathf.RoundToInt(pos.y / tileSize - 0.5f));

    }

    
    public bool hasChestOnDir(Vector2Int pos, Vector2Int direction)
    {
        var i = 0;
        AffectableItem hitItem;
        while (i < 100)
        {
            i++;
            var res = ShootDirection(pos, direction,out hitItem,false, false,true);
            pos += direction;
            if (hitItem && hitItem is Mimicry) {
                return true;
            }

            if (hitItem)
            {
                return false;
            }
            if(res == null)
            {
                return false;
            }
        }
        return false;
    }
    
    public GridStats ShootDirection(Vector2Int pos, Vector2Int direction, out AffectableItem hitItem, bool throughCorpse, bool hitHuman, bool throughWindow)
    {
        hitItem = null;
    
        var target = pos + (direction);
        if (!GridArray.ContainsKey(target))
        {
            return null;
        }
        foreach (var item in affectableItems)
        {
            if (item.pos == target)
            {
                
                hitItem = item;
            }
        }
        var gridStatus = GridArray[target];
        if (gridStatus)
        {
    
            return gridStatus.GetComponent<GridStats>();
        }
        return null;
    }
    static public Direction pairToDirection((int, int) pair)
    {
        var x = 0; var y = 0;

        switch (pair)
        {
            case (0, 1):
                return Direction.up;
            case (1, 0):
                return Direction.right;
            case (0, -1):
                return Direction.down;
            case (-1, 0):
                return Direction.left;



        }
        Debug.LogError("wrong direction " + pair);
        return Direction.right;
    }

    static public Vector2Int directionToVector2Int(Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                return  Vector2Int.up;
                break;
            case Direction.right:
                return  Vector2Int.right;
                break;
            case Direction.down:
                return  Vector2Int.down;
                break;
            case Direction.left:
                return  Vector2Int.left;
                break;
        }

        return Vector2Int.zero;
    } 
    public GridStats MoveDirection(int x, int y, Direction direction)
    {
        var target = new Vector2Int(x, y) + directionToVector2Int(direction);
        if (!GridArray.ContainsKey(target))
        {
            return null;
        }
        foreach(var item in affectableItems)
        {
            // if(item.pos == target)
            // {
            //     return null;
            // }
        }
        var gridStatus = GridArray[target];
        if (gridStatus)
        {
            return gridStatus.GetComponent<GridStats>();
        }
        return null; 
    }
    public bool in3x3Area(Vector2Int pos1,Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) <= 1 && Mathf.Abs(pos1.y - pos2.y) <= 1;
    }
    public bool inCrossArea(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) <= 1;
    }
    public  List<AffectableItem> affectableItems;
    public List<AffectableItem> allItems;
    public void AddAffectable(AffectableItem item)
    {
        affectableItems.Add(item);
        allItems.Add(item);
    }
    public void RemoveAffectable(AffectableItem item)
    {
        affectableItems.Remove(item);
        allItems.Remove(item);
    }

    bool TestDirection(int x, int y, int step, Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                return (y + 1 < Rows && GridArray[new Vector2Int(x, y + 1)] && GridArray[new Vector2Int(x, y + 1)].GetComponent<GridStats>().Visited == step);
            case Direction.right:
                return (x + 1 < Columns && GridArray[new Vector2Int(x + 1, y)] && GridArray[new Vector2Int(x + 1, y)].GetComponent<GridStats>().Visited == step);
            case Direction.down:
                return (y - 1 > -1 && GridArray[new Vector2Int(x, y - 1)] && GridArray[new Vector2Int(x, y - 1)].GetComponent<GridStats>().Visited == step);
            case Direction.left:
                return (x - 1 > -1 && GridArray[new Vector2Int(x - 1, y)] && GridArray[new Vector2Int(x - 1, y)].GetComponent<GridStats>().Visited == step);
            default:
                return false;
        }
    }

    GameObject FindClosest(Transform targetLocation, List<GameObject> list)
    {
        float currentDistance = Scale * Rows * Columns;
        int indexNumber = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < currentDistance)
            {
                currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                indexNumber = i;
            }
        }

        return list[indexNumber];
    }

    List<GameObject> GetPath(int toX, int toY)
    {
        int step;
        int x = toX;
        int y = toY;

        var path = new List<GameObject>();
        var tempList = new List<GameObject>();

        if (GridArray[new Vector2Int(toX, toY)] && GridArray[new Vector2Int(toX, toY)].GetComponent<GridStats>().Visited > 0)
        {
            path.Add(GridArray[new Vector2Int(x, y)]);
            step = GridArray[new Vector2Int(x, y)].GetComponent<GridStats>().Visited - 1;
        }
        else
        {
            print("Can't reach the desired location.");
            return null;
        }

        for (int i = step; step > -1; step--)
        {
            if (TestDirection(x, y, step, Direction.up))
            {
                tempList.Add(GridArray[new Vector2Int(x, y + 1)]);
            }
            if (TestDirection(x, y, step, Direction.right))
            {
                tempList.Add(GridArray[new Vector2Int(x + 1, y)]);
            }
            if (TestDirection(x, y, step, Direction.down))
            {
                tempList.Add(GridArray[new Vector2Int(x, y - 1)]);
            }
            if (TestDirection(x, y, step, Direction.left))
            {
                tempList.Add(GridArray[new Vector2Int(x - 1, y)]);
            }

            GameObject tempObj = FindClosest(GridArray[new Vector2Int(toX, toY)].transform, tempList);
            path.Add(tempObj);

            x = tempObj.GetComponent<GridStats>().X;
            y = tempObj.GetComponent<GridStats>().Y;

            tempList.Clear();
        }

        return path;
    }

    List<GameObject> GetPath(int fromX, int fromY, int toX, int toY)
    {
        SetDistance(fromX, fromY);
        return GetPath(toX, toY);
    }

    private void Start()
    {
        InitiateGrids();
       player = GameObject.FindObjectOfType<Mimicry>();
    }

    public void InitiateGrids()
    {
        Wall.Instance.init();
        // Trap.Instance.init();
        // Target.Instance.init();
        // Window.Instance.init();
        // HumanMask.Instance.init();
    }

    Vector3 startPosition = new Vector3(0.5f,0.5f,0);
    public void AddGrid(int i ,int j,string type)
    {
        if (GridArray.ContainsKey(new Vector2Int(i, j)))
            {
            GridArray[new Vector2Int(i, j)].GetComponent<GridStats>().type = type;
            return;
        }
        GameObject obj = Instantiate(GridPrefab, new Vector3(startPosition.x + Scale * i, startPosition.y + Scale * j, startPosition.z), Quaternion.identity);
        obj.name = $"grid-x{i}-y{j}";
        obj.transform.SetParent(gameObject.transform);
        obj.GetComponent<GridStats>().X = i;
        obj.GetComponent<GridStats>().Y = j;

        // add to grid once instantiated
        GridArray[new Vector2Int(i,j)] = obj;
    }
    public void GenerateGrid(int[,] map, Vector3 startPosition)
    {
        // check that we can instantiate a grid object
        if (!GridPrefab)
        {
            print("Missing GridPrefab, please assign.");
            return;
        }

        // assign the rows & columns properties based on our grid map size
        Rows = map.GetLength(1);
        Columns = map.GetLength(0);


        // build our grid map for our level based on the map object values
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                // find out what should be at this position
                int mapObj = map[i, j];
                switch (mapObj)
                {
                    case (int)GridType.passable:
                        // instantiate our grid object & assign position
                        GameObject obj = Instantiate(GridPrefab, new Vector3(startPosition.x + Scale * i, startPosition.y, startPosition.z + Scale * j), Quaternion.identity);
                        obj.name = $"grid-x{i}-y{j}";
                        obj.transform.SetParent(gameObject.transform);
                        obj.GetComponent<GridStats>().X = i;
                        obj.GetComponent<GridStats>().Y = j;

                        // add to grid once instantiated
                        GridArray[new Vector2Int(i, j)] = obj;
                        break;
                    case (int)GridType.nonPassable:
                    default:
                        break;
                }
            }
        }
    }

    public List<GameObject> GetPathToPosition(Transform from, Transform to, int maximumSteps)
    {
        var startX = (int)from.position.x;
        var startY = (int)from.position.z;
        var endX = (int)to.position.x;
        var endY = (int)to.position.y;

        return GetPath(startX, startY, endX, endY);
    }

    public List<GameObject> GetPathToPosition(Transform from, int toX, int toY, int maximumSteps)
    {
        var startX = (int)from.position.x;
        var startY = (int)from.position.z;

        return GetPath(startX, startY, toX, toY);
    }

    public List<GameObject> GetAvailablePositions(Transform currentPosition, int maximumSteps)
    {
        var startX = (int)currentPosition.position.x;
        var startY = (int)currentPosition.position.z;

        SetDistance(startX, startY);

        return null; // todo;
    }
}
