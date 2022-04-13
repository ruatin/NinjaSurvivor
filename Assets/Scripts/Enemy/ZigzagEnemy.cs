using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigzagEnemy : Enemy
{
    private float currentTime;
    private float delayTime = 1f;
    private float changeSpeed = 3f;
    private float dir = 1f;


    public override void OnDisable()
    {
        base.OnDisable();
        currentTime = 0f;
        dir = 1f;
    }
    

    protected override void Move(Vector3 targetDir)
    {
        targetDir = new Vector3(startPos.x, startPos.y *-1,0);
        
        currentTime += Time.deltaTime;
        if (currentTime >= delayTime)
        {
            currentTime = 0f;
            dir *= -1f;
        }
        
        targetDir += new Vector3(dir*changeSpeed, 0,0);
        base.Move(targetDir);

        if (Vector3.Distance(transform.position, targetDir) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }
}
