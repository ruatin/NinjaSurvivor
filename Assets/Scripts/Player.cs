using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Dead
    }

    public PlayerState playerState;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health > maxHealth)
            {
                _health = maxHealth;
            }
            healthImage.fillAmount = (float) _health / maxHealth;
        } 
    }
    private int _health;

    public int maxHealth;
    public int defence;
    public float normalAttackRange;
    public int normalAttackDamage;

    public float normalAttackDelayTime;
    public float moveSpeed;
    
    public int satelliteAttackDamage;
    public float satelliteAttackSpeed;
    public float satelliteAttackRadius;
    public int satelliteAttackCount;

    public float multipleShotCount;
    public float multipleShotPercent;

    public int recoveryAmount;
    
    private float normalAttackSpeed = 10f;
    private float recoveryCoolTime = 3f;
    private float recoveryCurrentTime;

    public Image healthImage;
    public SpriteRenderer playerEffectRenderer;
    private Animator _playerAnim;
    private Rigidbody2D _rigid;
    private SpriteRenderer _spriteRenderer;

    private bool _isNormalAttackAble = true;

    private bool isNoDamaged;
    private float noDamageCoolTime = 3f;
    private float noDamageCurrentTime;
    
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<Animator>();
        _health = maxHealth;
    }

    private void Start()
    {
        AdManager.instance.adContinueGameEvent += ContinueGame;
    }

    private void Update()
    {
        HpRecoveryTimer();
        NoDamageTimer();
        NormalAttack();

        SatelliteAttack();
        UpdateSatellitePos();;
    }

    public void FixedUpdate()
    {
        InputMoveDir();
    }

    public VirtualJoystick _joystick;

    private void InputMoveDir()
    {
        Vector3 inputDir = Vector3.zero;
        
        if (Application.isEditor)
        {
            inputDir.x = Input.GetAxisRaw("Horizontal");
            inputDir.y= Input.GetAxisRaw("Vertical");
        }
        else
        {
            inputDir = _joystick.InputControlVector();
        }
        inputDir = _joystick.InputControlVector();

        Move(inputDir);
    }

    public void Damaged(int amountDamage)
    {
        if (playerState == PlayerState.Dead || isNoDamaged)
        {
            return;
        }

        var damage = amountDamage - defence;
        if (damage <= 0)
        {
            damage = 0;
        }
        
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            playerState = PlayerState.Dead;
            GameManager.instance.SetGameOver();
        }
        else
        {
            //데미지 받는 사운드
            AudioManager.instance.PlayDamagedPlayer();

            if (!isDamagedTime)
            {
                StartCoroutine(ColorRepeat());
            }
            //연출
        }
    }

    private bool isDamagedTime;
    private IEnumerator ColorRepeat()
    {
        isDamagedTime = true;
        int countTime = 0;
        while (countTime < 5)
        {
            if (countTime % 2 == 0)
            {
                _spriteRenderer.color = new Color32(255,255,255,30);
                playerEffectRenderer.color = new Color32(255,255,255,30);
            }
            else
            {
                _spriteRenderer.color = new Color32(220,100,20,180);
                playerEffectRenderer.color = new Color32(220,100,20,180);
            }
            
            yield return new WaitForSeconds(0.2f);

            countTime++;
        }
        
        _spriteRenderer.color = Color.white;
        playerEffectRenderer.color = Color.white;
        isDamagedTime = false;
    }
    


    private void Move(Vector3 moveDir)
    {
        if (playerState == PlayerState.Dead)
        {
            _rigid.velocity = Vector2.zero;
            return;
        }
        
        playerState = moveDir == Vector3.zero ? PlayerState.Idle : PlayerState.Move;

        if (moveDir == Vector3.zero)
        {
            playerState = PlayerState.Idle;
            _playerAnim.speed = 0;
            playerEffectRenderer.enabled = false;
        }
        else
        {
            playerState = PlayerState.Move;
            _playerAnim.speed = 1;
            playerEffectRenderer.enabled = true;
        }
        
        if (moveDir.x < 0)
        {
            _spriteRenderer.flipX = true;
            playerEffectRenderer.flipX = true;
        }
        else if(moveDir.x > 0)
        {
            _spriteRenderer.flipX = false;
            playerEffectRenderer.flipX = false;
        }
        
        _rigid.velocity = moveDir * moveSpeed;
    }

    private void NormalAttack()
    {
        if (playerState == PlayerState.Dead || !_isNormalAttackAble)
        {
            return;
        }

        var enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        var enemy = Physics2D.OverlapCircleAll(transform.position, normalAttackRange, enemyLayer);
        Transform targetTr = null;

        var multipleOn = false;
        if (enemy.Length > 0)
        {
            if (multipleShotCount > 0 && enemy.Length > 1 && RandomResult(multipleShotPercent))
            {
                multipleOn = true;
            }
            else
            {
                float maxDist = 100f;
                foreach (var e in enemy)
                {
                    var dist = Vector3.Distance(e.transform.position, transform.position);
                    if (dist < maxDist)
                    {
                        maxDist = dist;
                        targetTr = e.transform;
                    }
                }
            }
        }
        else
        {
            return;
        }

        _isNormalAttackAble = false;
        StartCoroutine(NormalAttackDelayTime(normalAttackDelayTime));

        if (multipleOn)
        {
            float maxDist = 100f;
            foreach (var e in enemy)
            {
                var dist = Vector3.Distance(e.transform.position, transform.position);
                if (dist < maxDist)
                {
                    maxDist = dist;
                    targetTr = e.transform;
                }
            }
            
            var target1 = targetTr.position - transform.position;
            target1.Normalize();
                
            var multipleBull = BulletPool.GetObject();
            multipleBull.transform.position = transform.position;
            multipleBull.bulletDamage = normalAttackDamage;
            multipleBull.bulletSpeed = normalAttackSpeed;
            multipleBull.targetDir = targetTr;
            
            var count = multipleShotCount;
            foreach (var e in enemy)
            {
                if (count <= 0)
                {
                    break;
                }
                var target = e.transform.position - transform.position;
                target.Normalize();
                
                var multipleBullet = BulletPool.GetObject();
                multipleBullet.transform.position = transform.position;
                multipleBullet.bulletDamage = normalAttackDamage;
                multipleBullet.bulletSpeed = normalAttackSpeed;
                multipleBullet.targetDir = e.transform;
                count--;
            }
        }
        else
        {
            var targetDir = targetTr.transform.position - transform.position;
            targetDir.Normalize();
        
            var bullet = BulletPool.GetObject();
            bullet.transform.position = transform.position;
            bullet.bulletDamage = normalAttackDamage;
            bullet.bulletSpeed = normalAttackSpeed;
            bullet.targetDir = targetTr.transform;
        }


    }


    public GameObject satelliteObject;

    private List<Satellite> _satellites = new List<Satellite>();
    
    //위성공격
    private void SatelliteAttack()
    {
        if (playerState == PlayerState.Dead || satelliteAttackCount <= _satellites.Count)
        {
            return;
        }

        var diffCount = satelliteAttackCount - _satellites.Count;
        
        for (int i = 0; i < diffCount; i++)
        {
            var satellite = Instantiate(satelliteObject,transform.position,Quaternion.identity,satelliteRoot.transform).GetComponent<Satellite>();
            _satellites.Add(satellite);
        }

        UpdateSatelliteInfo();
    }

    //위성공격 업데이트는 위성 새로 생성할때
    //위성공격 업그레이드 버튼 눌렀을때
    //위성을 돌리는게 필요한데 위성 개수를 증가시킬때마다 
    //즉 업데이트를 할때마다 그럴때마다
    
    private void UpdateSatelliteInfo()
    {
        var theta = 360f / _satellites.Count;
        for (int i = 0; i < _satellites.Count; i++)
        {
            _satellites[i].damage = satelliteAttackDamage;
            _satellites[i].transform.position = transform.position + satelliteAttackRadius * new Vector3(Mathf.Cos(theta * Mathf.Deg2Rad * i ),Mathf.Sin(theta * Mathf.Deg2Rad * i ),0);
        }
    }

    public GameObject satelliteRoot;
    private void UpdateSatellitePos()
    {
        if (_satellites.Count <= 0)
        {
            return;
        }
        
        var velocity = Time.deltaTime * satelliteAttackSpeed;
        satelliteRoot.transform.Rotate(0,0,velocity);
    }
    
    IEnumerator NormalAttackDelayTime(float sec)
    {
        var time = 1 / sec;
        yield return new WaitForSeconds(time);
        _isNormalAttackAble = true;
    }

    public float GetUpgradeCurrentValue(int upgradeNum ,int upgradeValue)
    {
        // 전반적인 업그레이드 영향 값 조절함
        var result = 0f;
        switch (upgradeNum)
        {
            case (int)UpgradeNum.MaxHealth:
                result = maxHealth + upgradeValue *10;
                maxHealth = (int)result;
                break;
            case (int)UpgradeNum.AttackRange:
                result = normalAttackRange + upgradeValue *0.1f;
                normalAttackRange = result;
                break;
            case (int)UpgradeNum.AttackDamage:
                result = normalAttackDamage + (5 * upgradeValue);
                normalAttackDamage = (int)result;
                break;
            case (int)UpgradeNum.AttackDelayTime:
                result = 1f + 0.1f*upgradeValue;
                normalAttackDelayTime = result;
                break;
            case (int)UpgradeNum.MoveSpeed:
                result = moveSpeed + 0.01f * upgradeValue;
                moveSpeed = result;
                break;
            case (int)UpgradeNum.SatelliteAttackDamage:
                //result = satelliteAttackDamage + (int)(satelliteAttackDamage * Mathf.Pow(1.05f,upgradeValue));
                result = satelliteAttackDamage + (5 * upgradeValue);
                satelliteAttackDamage = (int) result;
                UpdateSatelliteInfo();
                break;
            case (int)UpgradeNum.SatelliteAttackSpeed:
                result = 150f + 0.5f * upgradeValue;
                satelliteAttackSpeed = result;
                break;
            case (int)UpgradeNum.SatelliteAttackRadius:
                result = 1 + 0.2f * upgradeValue;
                satelliteAttackRadius = result;
                UpdateSatelliteInfo();
                break;
            case (int)UpgradeNum.SatelliteAttackCount:
                result = upgradeValue;
                satelliteAttackCount = upgradeValue;
                break;
            case (int)UpgradeNum.MultipleShotCount:
                result = upgradeValue;
                multipleShotCount = result;
                break;
            case (int)UpgradeNum.Defence:
                result = upgradeValue;
                defence = (int)result;
                break;
            case (int)UpgradeNum.MultipleShotPercent:
                result = 5f + (5 * upgradeValue);
                multipleShotPercent = result;
                break;
            case (int)UpgradeNum.RecoveryAmount:
                result = upgradeValue;
                recoveryAmount = (int)result;
                break;
        }                                                                               
        return result;
    }
    
    
    //확률에 따른 결과 도출
    private bool RandomResult(float percentage)
    {
        if (percentage < 0.0000001f)
        {
            percentage = 0.0000001f;
        }

        percentage = percentage / 100;

        bool success = false;
        int RandAccuracy = 10000000;
        float randHitRange = percentage * RandAccuracy;
        int rand = UnityEngine.Random.Range(1, RandAccuracy+1);
        if (rand <= randHitRange)
        {
            success = true;
        }
        return success;
    }

    private void HpRecoveryTimer()
    {
        if (playerState == PlayerState.Dead || recoveryAmount <= 0)
        {
            return;
        }
        
        recoveryCurrentTime += Time.deltaTime;
        if (recoveryCurrentTime > recoveryCoolTime)
        {
            recoveryCurrentTime = 0f;
            Health += recoveryAmount;
        }
    }
    
    private void NoDamageTimer()
    {
        if (playerState == PlayerState.Dead || !isNoDamaged)
        {
            return;
        }
        
        noDamageCurrentTime += Time.deltaTime;
        if (noDamageCurrentTime > noDamageCoolTime)
        {
            noDamageCurrentTime = 0f;
            isNoDamaged = false;
        }
    }
    
    private void ContinueGame(bool isSuccess)
    {
        if (!isSuccess)
        {
            return;
        }
        playerState = PlayerState.Idle;
        Health = maxHealth;
        isNoDamaged = true;
        
        var enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        var enemy = Physics2D.OverlapCircleAll(transform.position, 10f, enemyLayer);
        foreach (var e in enemy)
        {
            var en = e.GetComponent<Enemy>();
            en.Damaged(en.maxHp);

        }
    }
}
