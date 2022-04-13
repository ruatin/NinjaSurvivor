using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEnemy : Enemy
{
    private float currentTime;
    private float delayTime = 0.8f;
    private float plusTeleport;
    private float plusPreviewPos;
    public GameObject previewObj;


    protected override void OnEnable()
    {
        base.OnEnable();
        plusPreviewPos = Random.Range(0.01f, 0.1f);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        currentTime = 0f;
        plusTeleport = 0f;
        previewObj.SetActive(false);
    }
    

    protected override void Move(Vector3 targetDir)
    {
        if (enemyState == EnemyState.Dead)
        {
            return;
        }
        
        targetDir = new Vector3(startPos.x, startPos.y *-1,0);

        currentTime += Time.deltaTime;
        if (currentTime >= delayTime)
        {
            previewObj.transform.position = Vector3.Lerp(transform.position,targetDir,plusTeleport + plusPreviewPos);
            previewObj.SetActive(true);
        }
        
        if (currentTime >= delayTime+1f)
        {
            previewObj.SetActive(false);
            currentTime = 0f;
            plusTeleport += plusPreviewPos;
            transform.position = Vector3.Lerp(transform.position,targetDir,plusTeleport);
        }


        if (Vector3.Distance(transform.position, targetDir) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }
}
