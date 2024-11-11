using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
public class AffectableManager : Singleton<AffectableManager>
{
    static public float moveTime =0.2f;
    public int movedCount = 0;

    public TileBase trapTile;
    public TileBase maskTile;
    // Start is called before the first frame update
    void Start()
    {
        //Camera.main.transform.position = new Vector3(0,  LevelManager.Instance.currentLevelInfo.cameraAdjust * 0.32f, -10);
        //LevelManager.Instance.resetLevel();

    }
    public bool canAction = true;
    public KeyCode keyDown = KeyCode.None;
    // public void UseSpace()
    // {
    //     if(canAction && !finishedLevel)
    //     {
    //         canAction = false;
    //         keyDown = KeyCode.Space;
    //         recordActionMessages();
    //     }
    // }
    //
    // public void UseArrow(KeyCode key)
    // {
    //     if (canAction && !finishedLevel)
    //     {
    //         canAction = false;
    //         keyDown = key;
    //         recordActionMessages();
    //     }
    // }

    // public bool AutoPathAction(KeyCode key)
    // {
    //     keyDown = key;
    //     return recordActionMessages();
    // }


    IEnumerator  recordActionMessages()
    {
        movedCount++;
        var affectedItems = new List<AffectableItem>();

        foreach (var item in GridManager.Instance.affectableItems)
        {
            if (item is Mimicry)
            {
                affectedItems.Add(item);
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Space) || keyDown == KeyCode.Space)
        {
            //action(affectedItems);
            // yield return StartCoroutine( );
        }
        else
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || keyDown ==KeyCode.D)
            {
                yield return StartCoroutine(moveAllItems(affectedItems, Direction.right));

            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || keyDown == KeyCode.A)
            {
                yield return StartCoroutine(moveAllItems(affectedItems, Direction.left));
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || keyDown == KeyCode.S)
            {
                yield return StartCoroutine(moveAllItems(affectedItems, Direction.down));
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || keyDown == KeyCode.W)
            {
                yield return StartCoroutine(moveAllItems(affectedItems, Direction.up));
            }
        }

            yield return StartCoroutine(updateEnemiesMove());
            
            
                
        keyDown = KeyCode.None;

        canAction = true;
        
        Inventory.Instance.addItem("stone");
    }
    IEnumerator moveAllItems(List<AffectableItem> affectedItems, Direction dir)
    {
        List<AffectableItem> moved = new List<AffectableItem>();
        bool willMove = false;
        
        var vec = GridManager. directionToVector2Int(dir);
        affectedItems.Sort(delegate (AffectableItem a, AffectableItem b)
        {
            if (vec.x == 1)
            {
                if(b.pos.x == a.pos.x)
                {
                    return b.pos.y.CompareTo(a.pos.y); //still try to follow an order
                }
                else
                {
                    return b.pos.x.CompareTo(a.pos.x); //have to follow this order
                }
            }
            else if (vec.x == -1)
            {
                if (b.pos.x == a.pos.x)
                {
                    return b.pos.y.CompareTo(a.pos.y);
                }
                else
                {
                    return a.pos.x.CompareTo(b.pos.x);
                }
            }
            else if (vec.y == 1)
            {
                if (b.pos.y == a.pos.y)
                {
                    return b.pos.x.CompareTo(a.pos.x);
                }
                else
                {
                    return b.pos.y.CompareTo(a.pos.y);
                }

            }
            else
            {

                if (b.pos.y == a.pos.y)
                {
                    return b.pos.x.CompareTo(a.pos.x);
                }
                else
                {
                    return a.pos.y.CompareTo(b.pos.y);
                }
            }
        });
        
        
        
        foreach (var item in affectedItems)
        {
            yield return StartCoroutine(move(item, dir));
            
        }

    }
    IEnumerator move(AffectableItem item, Direction dir)
    {
        // if (!item.canMove)
        // {
        //     return;
        // }
        var pos = item.pos;
        GridStats res = null;

        
        res = GridManager.Instance.MoveDirection(pos.x, pos.y, dir);


        if (res != null)
        {
            SFXManager.Instance.PlayerSFX(SFXManager.Instance.boxMove);
            yield return StartCoroutine(item.MoveEnumerator(new Vector2Int(res.X,res.Y)));
            // if (res.type == "Window" && !(item is Dog))
            // {
            //     return;
            // }
            //
            // item.RecordMoveCommand(new Vector2Int(res.X,res.Y));
            //
            // // if (res.type == "Target")
            // // {
            // //     RecordFinishLevelCommand();
            // // }
            // // else 
            if (res.type == "Stair")
            {
                SFXManager.Instance.PlayerSFX(SFXManager.Instance.finishLevel);
                
                CanvasMenu.Instance.gameEnd.SetActive(true);
            }
            // else if (res.type == "HumanMask" && item is Human human3 && !human3.isProtected)
            // {
            //     item.RecordCollectMask(res);
            // }
        }

        //item.updateAffectVisual();
    }
    
    
    bool finishedLevel = false;
    // Update is called once per frame
    void Update()
    {
        if (canAction &&!finishedLevel&& (Input.GetKey(KeyCode.Space)|| Input.GetKey(KeyCode.D)|| Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)))
        {
            canAction = false;
            StartCoroutine(recordActionMessages());
            //StartCoroutine(actionCoroutine());
        }

        
    }

    

    IEnumerator updateEnemiesMove()
    {
        foreach (var item in GridManager.Instance.affectableItems)
        {
            if (!item.isDead)
            {
                item.enemyMove();
            }
        }
        
        yield return new WaitForSeconds(moveTime);
        
        
        foreach (var item in GridManager.Instance.affectableItems.ToArray())
        {
            if (!item.isDead)
            {
                yield return StartCoroutine(item.enemyChestCheck());
            }
        }
    }
    
}
