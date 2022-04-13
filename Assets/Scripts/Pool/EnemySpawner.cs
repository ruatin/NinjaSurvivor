using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class WaveTable
{
    public int id;
    [SerializeField]
    public List<int> spawnGroup;
    public float spawnDelay;
}

[Serializable]
public class SpawnGroupTable
{
    public int id;
    public int groupNum;
    public string monsterId;
    public int spawnCount;
    public AreaType areaType;
}

[Serializable]
public class MonsterTable
{
    public string id;
    public MonsterType monsterType;
    public int maxHp;
    public int damage;
    public float attackSpeed;
    public float moveSpeed;
    public float monsterSize;
    public int monsterGold;
    public int monsterScore;
    public int monsterRank;
}

public enum AreaType
{
    Upper,
    Bottom
}

public class EnemySpawner : MonoBehaviour
{
    Queue<GameObject> enemyObjectPool = new Queue<GameObject>();

    public static EnemySpawner instance;
    
    [Header("Enemy 프리팹")]
    [SerializeField]
    private GameObject enemyPrefab;
    
    [Header("오브젝트 풀 양")]
    [SerializeField]
    private int poolAmount = 100;
    
    [SerializeField]
    private Collider2D _topArea;
    [SerializeField]
    private Collider2D _downArea;
    
    List<WaveTable> waveTable = new List<WaveTable>();
    List<SpawnGroupTable> spawnGroupTable = new List<SpawnGroupTable>();
    List<MonsterTable> monsterTable = new List<MonsterTable>();
    
    private const float waveDelayTime = 4.8f; 
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        
        waveTable = DataTransform.WaveDataLoad("Wave_Table");
        spawnGroupTable = DataTransform.SpawnGroupLoad("SpawnGroup_Table");
        monsterTable = DataTransform.MonsterDataLoad("Monster_Table");

        Initialize();
    }

    public void UpdateWaveData(int waveCount)
    {
        if (GameManager.instance.GameState == GameState.GameOver)
        {
            return;
        }

        var wave = waveTable.Find(x => x.id == waveCount);

        if (wave == null)
        {
            var lastWave = waveTable[waveTable.Count - 1].id;
            var waveRandom = Random.Range(lastWave - 9, lastWave + 1);
            wave = waveTable.Find(x => x.id == waveRandom);
        }
        
        StartCoroutine(DelaySpawnMonster(wave));
    }
    
    private IEnumerator DelaySpawnMonster(WaveTable wave)
    {
        foreach (var groupNum in wave.spawnGroup) 
        {
           var groupTables = spawnGroupTable.FindAll(x => x.groupNum == groupNum);
           foreach (var group in groupTables)
           {
               var monster = monsterTable.Find(x => x.id == group.monsterId);
               SpawnMonster(group.spawnCount,monster.monsterType,monster.maxHp,monster.damage,monster.attackSpeed,monster.moveSpeed,monster.monsterSize
                   ,monster.monsterGold,monster.monsterScore,monster.monsterRank,group.areaType);
           } 
           //웨이브내 몬스터 생성간 딜레이
           yield return new WaitForSeconds(wave.spawnDelay/1000f);
        }
        
        //웨이브간 딜레이
        yield return new WaitForSeconds(waveDelayTime);

        GameManager.instance.waveCount++;
        UpdateWaveData(GameManager.instance.waveCount);
    }

    private void SpawnMonster(int spawnCount,MonsterType _monsterType, int _maxHp ,int _damage,float _attackSpeed , float _moveSpeed ,float _monsterSize ,int _gold,int _score,int _rank , AreaType _areaType)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // 웨이브에 따른 강화 수준 조절함
            var plusWave = GameManager.instance.waveCount;
            var enemy = GetObject(_monsterType).GetComponent<Enemy>();
            enemy.monsterType = _monsterType;
            enemy.maxHp = _maxHp + (int)Mathf.Round(2f * Mathf.Pow(plusWave/1.2f,1.2f));
            enemy.normalAttackDamage = _damage + (int)Mathf.Round(0.2f * Mathf.Pow(plusWave / 1f, 1.2f));
            enemy.normalAttackSpeed = _attackSpeed;
            enemy.moveSpeed = _moveSpeed;
            enemy.monsterSize = _monsterSize;
            enemy.monsterGold = _gold + (int)Mathf.Round(1.5f * Mathf.Pow(plusWave / 1f, 1.3f));
            enemy.monsterScore = _score;
            enemy.monsterRank = _rank;
            enemy.transform.position = ReturnRandomPos(_areaType);
            enemy.gameObject.SetActive(true);
        }
    }

    public void StopAllSpawn()
    {
        StopAllCoroutines();
    }

    private void Initialize()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            enemyObjectPool.Enqueue(CreateNewObject(MonsterType.Boss));
        }
    }
    
    private GameObject CreateNewObject(MonsterType type)
    {
        var newObj = Instantiate(enemyPrefab, transform, true);
        newObj.gameObject.SetActive(false);
        return newObj;
    }
    
    private GameObject GetObject(MonsterType type)
    {
        if (enemyObjectPool.Count > 0)
        {
            var obj = enemyObjectPool.Dequeue();
            obj.transform.SetParent(null);
            return obj;
        }
        else
        {
            var newObj = CreateNewObject(type);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }


    public void ReturnObject(GameObject obj,MonsterType type)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);
        enemyObjectPool.Enqueue(obj);
    }


    
    private Vector3 ReturnRandomPos(AreaType areaType)
    {
        Collider2D col = new BoxCollider2D();
        if (areaType == AreaType.Upper)
        {
            col = _topArea;
        }
        else if(areaType == AreaType.Bottom)
        {
            col = _downArea;
        }
        
        Vector3 top = col.gameObject.transform.position;
        top.z = 0;
        var bounds = col.bounds;
        float rangeX = bounds.size.x;
        float rangeY = bounds.size.y;

        rangeX = Random.Range((rangeX / 2) * -1, rangeX / 2);
        rangeY = Random.Range((rangeY / 2) * -1, rangeY / 2);
        Vector3 randomPos = new Vector3(rangeX ,rangeY , 0  );
        Vector3 resultPos = randomPos + top;
        return resultPos;
    }
}
