using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Straight,
    Guided,
    Boss,
    Evade,
    Zigzag,
    Teleport,
}
public enum EnemyState
{
    Idle,
    Move,
    Attack,
    Dead
}

public class Enemy : MonoBehaviour
{

    
    [Header("몬스터 상태")]
    public EnemyState enemyState;
    
    [Header("몬스터 능력치")]
    public int maxHp;
    public int normalAttackDamage;
    public float normalAttackSpeed;
    public float moveSpeed;
    public float monsterSize;
    public int monsterGold;
    public int monsterScore;
    public int monsterRank;
    public MonsterType monsterType;

    protected Player player;
    public Vector3 targetDir;
    protected Vector3 startPos;
    
    private int _currentHp;
    private bool _isEnterPlayer = false;
    private bool _attackEnable = true;
    private Rigidbody2D _rigid;
    private Animator _anim;
    private EnemyMove _enemyMove;
    private static readonly int Type = Animator.StringToHash("Type");


    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _enemyMove = GetComponent<EnemyMove>();
        player = GameManager.instance.player;
    }

    protected virtual void OnEnable()
    {
        switch (monsterType)
        {
            case MonsterType.Straight:
                _anim.SetFloat(Type,0f);
                break;
            case MonsterType.Guided:
                _anim.SetFloat(Type,1f);
                break;
            case MonsterType.Boss:
                _anim.SetFloat(Type,2f);
                break;
            case MonsterType.Evade:
                _anim.SetFloat(Type,3f);
                break;
            case MonsterType.Zigzag:
                _anim.SetFloat(Type,4f);
                break;
            case MonsterType.Teleport:
                _anim.SetFloat(Type,5f);
                _rigid.isKinematic = true;
                break;
        }

        enemyState = EnemyState.Idle;
        _isEnterPlayer = false;
        startPos = transform.position;
        _currentHp = maxHp;
        transform.localScale = monsterSize * Vector3.one;
    }

    public virtual void OnDisable()
    {
        _rigid.isKinematic = false;
    }



    private void Update()
    {
        if (enemyState == EnemyState.Dead)
        {
            return;
        }

        if (_isEnterPlayer)
        {
            enemyState = EnemyState.Attack;
            Attack();
        }
        else
        {
            enemyState = EnemyState.Move;
            _enemyMove.Move(targetDir,enemyState,moveSpeed,_rigid,monsterType);
//            Move(targetDir);
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _isEnterPlayer = true;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _isEnterPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _isEnterPlayer = false;
        }
    }

    protected virtual void Move(Vector3 target)
    {
        if (enemyState == EnemyState.Dead)
        {
            return;
        }
        
        var myPos = transform.position;
        var moveDir = target - myPos;
        moveDir.Normalize();

        _rigid.velocity = moveDir * moveSpeed;
    }

    private void Attack()
    {
        if (!_attackEnable || enemyState == EnemyState.Dead)
        {
            return;
        }

        player.Damaged(normalAttackDamage);
        
        _attackEnable = false;
        StartCoroutine(WaitForTime(normalAttackSpeed));
    }

    public void Damaged(int damageAmount)
    {
        if (GameManager.instance.GameState == GameState.GameOver)
        {
            return;
        }
        
        AudioManager.instance.PlayHit();
        
        var hp = _currentHp - damageAmount;
        if (hp <= 0)
        {
            EffectManager.instance.PlayExplosion(transform.position ,  (monsterSize * 0.3f));
            CreateFloatingText(false, damageAmount);
                
            _currentHp = 0;
            enemyState = EnemyState.Dead;
            Dead();
        }
        else
        {
            EffectManager.instance.PlayExplosionHit(transform.position , (monsterSize * 0.2f));
            CreateFloatingText(true, damageAmount);
            
            _currentHp = hp;
        }
    }

    private void CreateFloatingText(bool isDamaged,int damageAmount)
    {
        var floatingText = FloatingTextPool.GetObject();
        if (isDamaged)
        {
            floatingText.damage = damageAmount;
            floatingText.isDamaged = true;
        }
        else
        {
            floatingText.gold = monsterGold;
            floatingText.isDamaged = false;
        }
        
        floatingText.transform.position = transform.position + new Vector3(0,monsterSize/10f,0);
        floatingText.text.fontSize += monsterSize * 15;
        floatingText.gameObject.SetActive(true);
    }

    private void Dead()
    {
        GameManager.instance.Gold += monsterGold;
        GameManager.instance.MonsterScore += monsterScore;
        _rigid.velocity = Vector2.zero;
        EnemySpawner.instance.ReturnObject(gameObject,monsterType);
    }

    IEnumerator WaitForTime(float sec)
    {
        yield return new WaitForSeconds(sec);
        _attackEnable = true;
    }
}
