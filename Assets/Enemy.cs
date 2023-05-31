using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : AffectableItem
{
    public string enemyType;

    public Transform patrolPointsParent;

    private List<Vector3> patrolPoints = new List<Vector3>();


    private int patrolId = 0;

    private bool patrolRevert = false;

    private EnemyInfo info;

    // Start is called before the first frame update
    protected override void Start()
    {
        info = CSVDataManager.Instance.enemyInfo(enemyType);
        foreach (Transform point in patrolPointsParent)
        {
            if (point.gameObject.activeInHierarchy)
            {
                patrolPoints.Add(point.position);
            }
        }

        transform.position = patrolPoints[0];
        renderer.sprite = Resources.Load<Sprite>("enemies/" + enemyType);
        base.Start();
        updateIndicator();
    }

    void updateIndicator()
    {
        var nextPatrolId = patrolId + (patrolRevert ? -1 : 1);
        var nextP = patrolPoints[nextPatrolId];
        var dir = (nextP - transform.position).normalized;

        if (dir.x > 0.9f)
        {
            indicator.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (dir.x < -0.9f)
        {
            indicator.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (dir.y < -0.9f)
        {
            indicator.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        else if (dir.y > 0.9f)
        {
            indicator.transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }

    public bool canSeeChest(bool checkPreviousDir)
    {
        var nextPatrolId = patrolId + (patrolRevert ? -1 : 1);

        var nextP = patrolPoints[nextPatrolId];
        var dir = lastDir; //(nextP - transform.position).normalized;

        var normalizedDir = new Vector2Int((int)math.round(dir.x), (int)math.round(dir.y));

        if (checkPreviousDir)
        {
            
            if (GridManager.Instance.hasChestOnDir(pos, normalizedDir))
            {
                return true;
            }
        }

        dir = (nextP - transform.position).normalized;

        normalizedDir = new Vector2Int((int)math.round(dir.x), (int)math.round(dir.y));


        if (GridManager.Instance.hasChestOnDir(pos, normalizedDir))
        {
            return true;
        }

        return false;
    }

    public IEnumerator catchMimicry()
    {
        transform.DOMove(GridManager.indexToPosition( GridManager.Instance.player.pos), moveTime);
        yield return new WaitForSeconds(moveTime);
        //show dialogue
        var react = CSVDataManager.Instance.enemyReactToItem(enemyType, Inventory.Instance.selectedItem);
        dialoguePanel.SetActive(true);
        dialogueLabel.text = enemyType + " spotted you!";
        yield return WaitForAnyKey();
        LevelManager.Instance.RestartLevel();
    }

    private Vector3 lastDir;

    Vector3 moveToNextPosition()
    {
        var nextPatrolId = patrolId + (patrolRevert ? -1 : 1);

        var nextP = patrolPoints[nextPatrolId];
        lastDir = (nextP - transform.position).normalized;
        var nextMove = transform.position + lastDir;

        // var nextPos = GridManager.PositionToIndex(nextMove);
        // if (GridManager.Instance.player.pos == nextPos)
        // {
        //     return transform.position;
        // }
        
        if ((nextMove - nextP).magnitude <= 0.3f)
        {
            patrolId = nextPatrolId;
            if (patrolId == patrolPoints.Count - 1 || patrolId == 0)
            {
                patrolRevert = !patrolRevert;
            }
        }

        updateIndicator();
        return nextMove;
    }

    void destroy()
    {
        GridManager.Instance.RemoveAffectable(this);
        Destroy(gameObject);
    }

    public override void enemyMove()
    {
        base.enemyMove();

        var nextP = moveToNextPosition();
        pos = GridManager.PositionToIndexPair(nextP);

        transform.DOMove(GridManager.indexToPosition(pos), moveTime);
    }

    private IEnumerator WaitForAnyKey()
    {
        while (!Input.anyKeyDown)
        {
            yield return null; // Wait until next frame
        }

        Debug.Log("A key or mouse click has been detected");
    }

    private bool isMovingBack = false;
    private Vector2Int backPos;
    public override IEnumerator enemyChestCheck()
    {
        if (canSeeChest(true) || pos == GridManager.Instance.player.pos)
        {
            //move to chest
            backPos = pos;
            pos = GridManager.Instance.player.pos;
            transform.DOMove(GridManager.Instance.player.transform.position, moveTime);
            yield return new WaitForSeconds(moveTime);
            //show dialogue
            var react = CSVDataManager.Instance.enemyReactToItem(enemyType, Inventory.Instance.selectedItem);
            dialoguePanel.SetActive(true);
            if (react != null)
            {
                dialogueLabel.text = react.words;
                yield return WaitForAnyKey();
                destroy();
                Inventory.Instance.removeItem(Inventory.Instance.selectedItem);
                Inventory.Instance.selectedItem = "";

                Inventory.Instance.addItem(info.drop);
            }
            else
            {
                dialogueLabel.text = info.disappoint;

                yield return WaitForAnyKey();
                dialoguePanel.SetActive(false);
                if (backPos != GridManager.Instance.player.pos)
                {
                    
                    var nextPatrolId = patrolId + (patrolRevert ? -1 : 1);
                    patrolId = nextPatrolId;
                    patrolRevert = !patrolRevert;
                    updateIndicator();
                }
                
                
                //transform.DOMove(GridManager.indexToPosition(pos), moveTime);
                //yield return new WaitForSeconds(moveTime);
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        dialogueLabel.text = info.words;
    }

    // Update is called once per frame
    void Update()
    {
    }
}