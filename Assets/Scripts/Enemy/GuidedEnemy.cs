using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedEnemy : Enemy
{
    private float delayTime = 1.5f;
    private float guideRange = 3.0f;
    private bool outSideMove = false;
    private float currentTime;
    private bool isTraceStart;
    private float slowSpeedPercent = 70f;

    private Vector3 lastDir;
    private float tempSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();
        currentTime = 0f;
        outSideMove = false;
        isTraceStart = false;
        tempSpeed = moveSpeed;
    }

    protected override void Move(Vector3 targetDir)
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= guideRange)
        {
            isTraceStart = true;
        }
        else
        {
            targetDir = new Vector3(startPos.x, startPos.y *-1,0);
            outSideMove = true;
        }
        
        
        if (isTraceStart)
        {
            targetDir = player.transform.position;
            moveSpeed = tempSpeed * (slowSpeedPercent/100f);
            
            currentTime += Time.deltaTime;
            if (currentTime > delayTime)
            {
                moveSpeed = tempSpeed;
                outSideMove = true;
                targetDir = new Vector3(transform.position.x, startPos.y *-1,0);
            }
        }

        
        base.Move(targetDir);

        if (outSideMove && Vector3.Distance(transform.position, targetDir) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, guideRange);
    }
    
}
