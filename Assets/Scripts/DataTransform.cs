using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataTransform
{
    public static List<SpawnGroupTable> SpawnGroupLoad(string fileName)
    {
        List<SpawnGroupTable> spawnGroupTables = new List<SpawnGroupTable>();
        
        var fileData = Resources.Load(fileName) as TextAsset;

        if (fileData == null)
        {
            Debug.Log("File is null");
            return null;
        }

        var lines = fileData.text.Split('\n');

        for (int i = 2; i < lines.Length; i++)
        {
            if (lines[i] == "")
            {
                break;
            }
            
            var ward = lines[i].Split(',');
            SpawnGroupTable spawnGroupTable = new SpawnGroupTable();
            spawnGroupTable.id = int.Parse(ward[0]);
            spawnGroupTable.groupNum = int.Parse(ward[1]);
            spawnGroupTable.monsterId = ward[2];
            spawnGroupTable.spawnCount = int.Parse(ward[3]);
            spawnGroupTable.areaType = (AreaType)Enum.Parse(typeof(AreaType),ward[4]);
            
            spawnGroupTables.Add(spawnGroupTable);
        }
        
        return spawnGroupTables;
    }

    public static List<WaveTable> WaveDataLoad(string fileName)
    {
        List<WaveTable> result = new List<WaveTable>();
        
        var fileData = Resources.Load(fileName) as TextAsset;

        if (fileData == null)
        {
            Debug.Log("File is null");
            return null;
        }

        var lines = fileData.text.Split('\n');

        for (int i = 2; i < lines.Length; i++)
        {
            if (lines[i] == "")
            {
                break;
            }
            
            var ward = lines[i].Split(',');
            WaveTable waveTable = new WaveTable {id = int.Parse(ward[0]), spawnGroup = new List<int>()};

            for (int j = 1; j < ward.Length; j++)
            {
                int temp;
                if(ward[j].EndsWith("\""))
                {
                    temp = int.Parse(ward[j].TrimEnd('\"'));
                    waveTable.spawnGroup.Add(temp);
                    break;
                }
                
                temp = int.Parse(ward[j].TrimStart('\"'));
                waveTable.spawnGroup.Add(temp);
            }

            waveTable.spawnDelay = float.Parse(ward[ward.Length - 1]);

            result.Add(waveTable);
        }

        return result;
    }
    
    public static List<MonsterTable> MonsterDataLoad(string fileName)
    {
        List<MonsterTable> monsterTables = new List<MonsterTable>();
        
        var fileData = Resources.Load(fileName) as TextAsset;

        if (fileData == null)
        {
            Debug.Log("File is null");
            return null;
        }

        var lines = fileData.text.Split('\n');

        for (int i = 2; i < lines.Length; i++)
        {
            if (lines[i] == "")
            {
                break;
            }
            
            var ward = lines[i].Split(',');
            MonsterTable monsterTable = new MonsterTable();
            monsterTable.id = ward[0];
            monsterTable.monsterType = (MonsterType)Enum.Parse(typeof(MonsterType),ward[1]);
            monsterTable.maxHp = int.Parse(ward[2]);
            monsterTable.damage = int.Parse(ward[3]);
            monsterTable.attackSpeed = float.Parse(ward[4]);
            monsterTable.moveSpeed = float.Parse(ward[5]);
            monsterTable.monsterSize = float.Parse(ward[6]);
            monsterTable.monsterGold = int.Parse(ward[7]);
            monsterTable.monsterScore = int.Parse(ward[8]);
            monsterTable.monsterRank = int.Parse(ward[9]);

            monsterTables.Add(monsterTable);
        }
        
        return monsterTables;
    }
}
