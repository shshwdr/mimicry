using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stair : Singleton<Stair>
{
    public GameObject animatedFlag;
    Vector3 targetTilePosition;
    Vector3Int targetTileInd;

    private Vector2Int pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void init()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            // Do stuff per position
            TileBase currentTile = tilemap.GetTile(position);
            //Debug.Log("position " + position+" "+currentTile);
            if (currentTile)
            {
                targetTileInd = position;
                targetTilePosition = GridManager.indexToPosition(position);
                GridManager.Instance.AddGrid(position.x, position.y, "Stair");
                pos = new Vector2Int(position.x, position.y);
            }

        }
    }
}
