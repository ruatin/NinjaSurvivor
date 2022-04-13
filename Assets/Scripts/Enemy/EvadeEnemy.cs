using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeEnemy : Enemy
{
    private float evadeRange = 2f;
    
    protected override void Move(Vector3 targetDir)
    {
        targetDir = new Vector3(startPos.x, startPos.y *-1,0);

        var randX = 0;
        if (startPos.x > 0)
        {
            randX = 5;
        }
        else
        {
            randX = -5;
        }
        
        if (Vector3.Distance(transform.position, player.transform.position) <= evadeRange)
        {
            var a = (transform.position - player.transform.position) + new Vector3(randX,0,0);
            targetDir = targetDir + a;
        }
        

        base.Move(targetDir);

        if (Vector3.Distance(transform.position, targetDir) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }
}
