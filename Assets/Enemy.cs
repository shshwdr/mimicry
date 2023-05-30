using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : AffectableItem
{
    public string enemyType;

    public Transform patrolPointsParent;

    private List<Vector3> patrolPoints = new List<Vector3>();

    private int patrolId = 0;

    private bool patrolRevert = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        foreach (Transform point in patrolPointsParent)
        {
            patrolPoints.Add(point.position);
        }

        transform.position = patrolPoints[0];
        
        base.Start();
    }

    Vector3 nextPosition()
    {
        var nextPatrolId = patrolId + (patrolRevert ? -1 : 1);

        var nextP = patrolPoints[nextPatrolId];
        var dir = (nextP - transform.position).normalized;
        var nextMove = transform.position + dir;
        if ((nextMove - nextP).magnitude <= 0.3f)
        {
            patrolId = nextPatrolId;
            if (patrolId == patrolPoints.Count - 1 || patrolId == 0)
            {
                patrolRevert = !patrolRevert;
            }
        }
        return nextMove;
    }
    public override void enemyMove()
    {
        base.enemyMove();

        var nextP =  nextPosition();
        pos = GridManager.PositionToIndexPair(nextP);
        
        transform.DOMove(GridManager.indexToPosition(pos), moveTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
