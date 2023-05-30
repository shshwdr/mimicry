using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Wall : Singleton<Wall>
{
    // Start is called before the first frame update
    void Start()
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

            }
            else
            {
                GridManager.Instance. AddGrid(position.x, position.y,"Ground");
            }

        }
    }
}