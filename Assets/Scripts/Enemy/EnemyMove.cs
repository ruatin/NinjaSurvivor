using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviour
{
    private Player player;
    private Vector3 startPos;
    
    
    #region 공용 파라미터
    private float currentTime;
    #endregion
    
    #region GuidedEnmey 파라미터
    private float guideRange = 3.0f;
    private float guideDelayTime = 1.5f;
    private float slowSpeedPercent = 70f;
    private bool outSideMove = false;
    private bool isTraceStart;
    #endregion

    #region ZigzagEnemy 파라미터
    private float ZigzagDelayTime = 1f;
    private float changeSpeed = 3f;
    private float dir = 1f;
    #endregion

    #region TeleportEnemy 파라미터
    private float teleportDelayTime = 0.8f;
    private float plusTeleport;
    private float plusPreviewPos;
    public GameObject previewObj;
    #endregion

    #region Evade 파라미터
    private float evadeRange = 2f;
    #endregion



    private Vector3 lastDir;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    private void OnEnable()
    {
        startPos = transform.position;
        
        //teleport
        plusPreviewPos = Random.Range(0.01f, 0.1f);
    }

    private void OnDisable()
    {
        currentTime = 0f;
        outSideMove = false;
        isTraceStart = false;
        
        //teleport
        plusTeleport = 0f;
        previewObj.SetActive(false);
    }

    public void Move(Vector3 target , EnemyState enemyState ,float moveSpeed ,Rigidbody2D rigid ,MonsterType monsterType)
    {
        if (enemyState == EnemyState.Dead)
        {
            return;
        }

        switch (monsterType)
        {
            case MonsterType.Boss:
                BossTypeMove(target, moveSpeed, rigid);
                break;
            case MonsterType.Straight:
                StraightMove(target, moveSpeed, rigid, monsterType);
                break;
            case MonsterType.Guided:
                GuidedMove(target, moveSpeed, rigid, monsterType);
                break;
            case MonsterType.Zigzag:
                ZigzagMove(target, moveSpeed, rigid, monsterType);
                break;
            case MonsterType.Teleport:
                TeleportMove(target, monsterType);
                break;
            case MonsterType.Evade:
                EvadeMove(target, moveSpeed, rigid, monsterType);
                break;
        }

    }

    private void BossTypeMove(Vector3 target , float moveSpeed ,Rigidbody2D rigid)
    {
        target = player.transform.position;
        var moveDir = target - transform.position;
        moveDir.Normalize();

        rigid.velocity = moveDir * moveSpeed;
    }
    
    private void StraightMove(Vector3 target , float moveSpeed ,Rigidbody2D rigid,MonsterType monsterType)
    {
        target = new Vector3(startPos.x, startPos.y *-1,0);
        
        var moveDir = target - transform.position;
        moveDir.Normalize();

        rigid.velocity = moveDir * moveSpeed;
        
        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }
    
    private void GuidedMove(Vector3 target, float moveSpeed, Rigidbody2D rigid, MonsterType monsterType)
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= guideRange)
        {
            isTraceStart = true;
        }
        else
        {
            target = new Vector3(startPos.x, startPos.y *-1,0);
            outSideMove = true;
        }
        
        
        if (isTraceStart)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= guideDelayTime)
            {
                outSideMove = true;
                target = new Vector3(transform.position.x, startPos.y *-1,0);
            }
            else
            {
                target = player.transform.position;
                moveSpeed  *= slowSpeedPercent/100f;
            }
        }

        
        var moveDir = target - transform.position;
        moveDir.Normalize();
        rigid.velocity = moveDir * moveSpeed;

        if (outSideMove && Vector3.Distance(transform.position, target) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }

    private void ZigzagMove(Vector3 target, float moveSpeed, Rigidbody2D rigid, MonsterType monsterType)
    {
        target = new Vector3(startPos.x, startPos.y *-1,0);
        
        currentTime += Time.deltaTime;
        if (currentTime >= ZigzagDelayTime)
        {
            currentTime = 0f;
            dir *= -1f;
        }
        
        target += new Vector3(dir*changeSpeed, 0,0);

        var moveDir = target - transform.position;
        moveDir.Normalize();
        rigid.velocity = moveDir * moveSpeed;

        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }   
    }

    private void TeleportMove(Vector3 target, MonsterType monsterType)
    {
        target = new Vector3(startPos.x, startPos.y *-1,0);

        currentTime += Time.deltaTime;
        if (currentTime >= teleportDelayTime && !previewObj.activeSelf)
        {
            previewObj.transform.position = Vector3.Lerp(transform.position,target,plusTeleport + plusPreviewPos);
            previewObj.SetActive(true);
        }
        
        if (currentTime >= teleportDelayTime+1f)
        {
            previewObj.SetActive(false);
            currentTime = 0f;
            plusTeleport += plusPreviewPos;
            transform.position = Vector3.Lerp(transform.position,target,plusTeleport);
        }


        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }

    private void EvadeMove(Vector3 target, float moveSpeed, Rigidbody2D rigid, MonsterType monsterType)
    {
        target = new Vector3(startPos.x, startPos.y *-1,0);

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
            target = target + a;
        }
        
        var moveDir = target - transform.position;
        moveDir.Normalize();
        rigid.velocity = moveDir * moveSpeed;

        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            EnemySpawner.instance.ReturnObject(gameObject,monsterType);
        }
    }
}
