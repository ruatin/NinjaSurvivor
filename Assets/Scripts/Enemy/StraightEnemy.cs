using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightEnemy : Enemy
{
    protected override void Move(Vector3 targetDir)
    {
        targetDir = new Vector3(startPos.x, startPos.y *-1,0);
        base.Move(targetDir);

        if (Vector3.Distance(transform.position, targetDir) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }
}
